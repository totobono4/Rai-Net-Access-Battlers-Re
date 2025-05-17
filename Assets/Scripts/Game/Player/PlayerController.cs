using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour {
    public static PlayerController LocalInstance { get; private set; }

    [SerializeField] private LayerMask tileLayerMask;

    [SerializeField] private NetworkVariable<PlayerTeam> team;

    private Vector3 lastMouseWorldPosition;
    private Tile selectedTile;
    private Card actionCard;
    private List<Tile> actionableTiles;

    public EventHandler<HoverTileChangedArgs> OnHoverTileChanged;
    public class HoverTileChangedArgs : EventArgs {
        public Tile tile;
    }
    public static EventHandler<SelectedTileArgs> OnSelectTile;
    public class SelectedTileArgs : EventArgs {
        public PlayerTeam team;
        public Tile tile;
    }
    public static EventHandler<ActionTileArgs> OnAction;
    public class ActionTileArgs : EventArgs {
        public int actionTokens;
        public Tile tile;
        public Card card;
    }
    public static EventHandler<CancelTileArgs> OnCancelAction;
    public class CancelTileArgs : EventArgs {
        public Tile tile;
        public Card card;
    }

    public static event EventHandler<AnyPlayerSpawnedArgs> OnAnyPlayerSpawned;
    public class AnyPlayerSpawnedArgs : EventArgs {
        public ulong clientId;
    }

    public static EventHandler<TeamChangedArgs> OnTeamChanged;
    public class TeamChangedArgs : EventArgs {
        public PlayerTeam team;
    }

    private NetworkVariable<PlayerState> playerState;

    private enum PlayerState {
        PlacingOnlineCards,
        WaitingForTurn,
        SelectingForAction,
        ThinkingForAction,
        Won,
        Lose
    }

    public static EventHandler OnPlayerStateChanged;

    public static EventHandler<EventArgs> OnActionFinished;
    public class ActionFinishedArgs : EventArgs {
        public int actionCost;
    }

    private NetworkVariable<int> actionTokens;

    private void Awake() {
        playerState = new NetworkVariable<PlayerState>(PlayerState.PlacingOnlineCards);
        playerState.OnValueChanged += PlayerState_OnValueChanged;

        actionTokens = new NetworkVariable<int>(0);
        actionTokens.OnValueChanged += ActionTokens_OnValueChanged;

        selectedTile = null;
        actionCard = null;
        actionableTiles = new List<Tile>();

        team = new NetworkVariable<PlayerTeam>(PlayerTeam.None);
        team.OnValueChanged += Team_OnValueChanged;
    }

    private void Start() {
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }

    private void PlayerState_OnValueChanged(PlayerState previousValue, PlayerState newValue) {
        OnPlayerStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetWon() {
        playerState.Value = PlayerState.Won;
    }

    private void SetLose() {
        playerState.Value = PlayerState.Lose;
    }

    public bool HasWon() {
        return playerState.Value == PlayerState.Won;
    }

    public bool HasLose() {
        return playerState.Value == PlayerState.Lose;
    }

    private void GameManager_OnGameOver(object sender, GameManager.GameOverArgs e) {
        if (e.team == GetTeam()) {
            if (e.hasWon) SetWon();
            else SetLose();
        }
        else {
            if (e.hasWon) SetLose();
            else SetWon();
        }
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        InputSystem.Instance.OnPlayerAction += InputSystem_OnPlayerAction;
        GameManager.Instance.OnPlayerGivePriority += GameManager_OnPlayerGivePriority;

        if (IsOwner) LocalInstance = this;

        OnAnyPlayerSpawned?.Invoke(this, new AnyPlayerSpawnedArgs {
            clientId = OwnerClientId
        });

        if (IsOwner) InitializeServerRpc();
    }

    private void Update() {
        if (!IsOwner) return;

        SendEventHoverTileChanged();
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    private void InitializeServerRpc(ServerRpcParams serverRpcParams = default) {
        team.Value = MultiplayerManager.Instance.GetClientTeamById(serverRpcParams.Receive.SenderClientId);
    }

    private void Team_OnValueChanged(PlayerTeam previous, PlayerTeam current) {
        OnTeamChanged?.Invoke(this, new TeamChangedArgs { team = current });
    }

    public PlayerTeam GetTeam() { return team.Value; }

    private void SendEventHoverTileChanged() {
        Vector3 mouseWorldPosition = InputSystem.Instance.GetMouseWorldPosition();
        if (GameBoard.Instance.GetTile(mouseWorldPosition, out Tile tile)) {
            OnHoverTileChanged?.Invoke(this, new HoverTileChangedArgs {
                tile = tile
            });
            return;
        }
        OnHoverTileChanged?.Invoke(this, new HoverTileChangedArgs {
            tile = null
        });
    }

    private void GameManager_OnPlayerGivePriority(object sender, GameManager.PlayerGivePriorityArgs e) {
        if (e.team == team.Value) playerState.Value = PlayerState.SelectingForAction;
        else playerState.Value = PlayerState.WaitingForTurn;
        actionTokens.Value = e.actionTokens;
    }

    private void ActionTokens_OnValueChanged(int previous, int current) {
        if (!IsServer) return;
        GameManager.Instance.PassPriority(actionTokens.Value);
    }

    private void InputSystem_OnPlayerAction(object sender, InputSystem.PlayerActionEventArgs e) {
        if (!IsOwner) return;

        lastMouseWorldPosition = InputSystem.Instance.GetMouseWorldPosition();

        switch (playerState.Value) {
            default: break;
            case PlayerState.PlacingOnlineCards:
                PlacingOnlineCards(e.playerActionType);
                break;
            case PlayerState.WaitingForTurn:
                WaitingForTurn(e.playerActionType);
                break;
            case PlayerState.SelectingForAction:
                SelectingForAction(e.playerActionType);
                break;
            case PlayerState.ThinkingForAction:
                ThinkingForAction(e.playerActionType);
                break;
        }
    }

    public bool IsPlacingCards() {
        return playerState.Value == PlayerState.PlacingOnlineCards;
    }

    public bool IsWaitingForTurn() {
        return playerState.Value == PlayerState.WaitingForTurn;
    }

    private bool IsSelectingForAction() {
        return playerState.Value == PlayerState.SelectingForAction;
    }

    private bool IsThinkingForAction() {
        return playerState.Value == PlayerState.ThinkingForAction;
    }

    private void PlacingOnlineCards(InputSystem.PlayerActionType playerActionType) {
        if (!GameBoard.Instance.GetTile(lastMouseWorldPosition, out Tile tile)) return;
        if (tile.GetTeam() != team.Value) return;
        if (tile is not StartTile) return;

        switch (playerActionType) {
            default: break;
            case InputSystem.PlayerActionType.Action:
                PlacingCardsServerRpc(tile.GetComponent<NetworkObject>(), OnlineCardState.Link);
                break;
            case InputSystem.PlayerActionType.SecondaryAction:
                PlacingCardsServerRpc(tile.GetComponent<NetworkObject>(), OnlineCardState.Virus);
                break;
        }
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void PlacingCardsServerRpc(NetworkObjectReference tileNetworkReference, OnlineCardState onlineCardState) {
        if (!IsPlacingCards()) return;

        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile tile = tileNetwork.GetComponent<Tile>();

        if (!tile.TryGetComponent(out StartTile startTile)) return;
        (tile as StartTile).TryPlaceOnlineCard(onlineCardState, team.Value);
    }

    private void WaitingForTurn(InputSystem.PlayerActionType playerActionType) {

    }

    private void SelectingForAction(InputSystem.PlayerActionType playerActionType) {
        if (playerActionType != InputSystem.PlayerActionType.Action) return;
        if (!GameBoard.Instance.GetTile(lastMouseWorldPosition, out Tile tile)) return;
        SelectingForActionServerRpc(tile.GetComponent<NetworkObject>());
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void SelectingForActionServerRpc(NetworkObjectReference tileNetworkReference) {
        if (!IsSelectingForAction()) return;

        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile tile = tileNetwork.GetComponent<Tile>();

        tile.OnSelectedValueChanged += Tile_OnSelectedTile;
        OnSelectTile?.Invoke(this, new SelectedTileArgs { tile = tile, team = GetTeam() });
    }

    private void ThinkingForAction(InputSystem.PlayerActionType playerActionType) {
        if (playerActionType != InputSystem.PlayerActionType.Action) return;
        if (!GameBoard.Instance.GetTile(lastMouseWorldPosition, out Tile tile)) return;
        ThinkingForActionServerRpc(tile.GetComponent<NetworkObject>());
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void ThinkingForActionServerRpc(NetworkObjectReference tileNetworkReference) {
        if (!IsThinkingForAction()) return;

        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile tile = tileNetwork.GetComponent<Tile>();

        if (tile.Equals(selectedTile)) {
            foreach (Tile actionable in actionableTiles) actionable.UnsetActionable();
            CancelAction();
            playerState.Value = PlayerState.SelectingForAction;
            return;
        }

        OnAction?.Invoke(this, new ActionTileArgs {
            actionTokens = actionTokens.Value,
            tile = tile,
            card = actionCard
        }); ;
    }

    private void Tile_OnSelectedTile(object sender, Tile.SelectedTileArgs e) {
        if (!IsSelectingForAction()) return;

        e.tile.OnSelectedValueChanged -= Tile_OnSelectedTile;

        if (!e.tile.GetCard(out Card card)) return;
        if (!card.IsUsable()) return;
        if (!e.isSelected) return;

        selectedTile = e.tile;
        playerState.Value = PlayerState.ThinkingForAction;

        actionableTiles = card.GetActionables();
        foreach (Tile actionable in actionableTiles) actionable.SetActionable();

        actionCard = card;
        actionCard.OnActionCallback += Card_OnActionCallback;
    }

    private void Card_OnActionCallback(object sender, Card.ActionCallbackArgs e) {
        if (!IsThinkingForAction()) return;

        actionTokens.Value -= e.tokenCost;

        foreach (Tile actionable in actionableTiles) actionable.UnsetActionable();
        if (e.finished) CancelAction();
        else foreach (Tile actionable in e.card.GetActionables()) actionable.SetActionable();
    }

    private void CancelAction() {
        actionCard.OnActionCallback -= Card_OnActionCallback;
        OnCancelAction?.Invoke(this, new CancelTileArgs { tile = selectedTile, card = actionCard });
        selectedTile = null;
        actionCard = null;
    }
}
