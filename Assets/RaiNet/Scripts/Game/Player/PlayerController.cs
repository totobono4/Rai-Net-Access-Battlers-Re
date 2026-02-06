using System;
using System.Collections.Generic;
using System.Linq;
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
    public static EventHandler<CancelSelectionArgs> OnCancelSelection;
    public class CancelSelectionArgs : EventArgs {
        public Tile tile;
    }
    public static EventHandler<CancelTileArgs> OnCancelAction;
    public class CancelTileArgs : EventArgs {
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
        PlacingOnlineCards = 0,
        WaitingForTurn = 1,
        PlayingTurn = 2,
        Won = 4,
        Lose = 5
    }

    public static EventHandler OnPlayerStateChanged;

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

    public override void OnDestroy() {
        playerState.OnValueChanged -= PlayerState_OnValueChanged;
        actionTokens.OnValueChanged -= ActionTokens_OnValueChanged;
        team.OnValueChanged -= Team_OnValueChanged;
        if (InputSystem.Instance) InputSystem.Instance.OnPlayerAction -= InputSystem_OnPlayerAction;
        if (GameManager.Instance) GameManager.Instance.OnPlayerGivePriority -= GameManager_OnPlayerGivePriority;
        if (IsOwner) LocalInstance = null;
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

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    private void InitializeServerRpc(RpcParams rpcParams = default) {
        team.Value = GameManager.Instance.GetClientTeamById(rpcParams.Receive.SenderClientId);
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
        if (e.team == team.Value) playerState.Value = PlayerState.PlayingTurn;
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
            case PlayerState.PlayingTurn:
                PlayingTurn(e.playerActionType);
                break;
        }
    }

    public bool IsPlacingCards() {
        return playerState.Value == PlayerState.PlacingOnlineCards;
    }

    public bool IsWaitingForTurn() {
        return playerState.Value == PlayerState.WaitingForTurn;
    }

    private bool IsPlayingTurn() {
        return playerState.Value == PlayerState.PlayingTurn;
    }

    private void SetSelectedTile(Tile tile) {
        selectedTile = tile;
    }

    private void UnsetSelectedTile() {
        OnCancelSelection?.Invoke(this, new CancelSelectionArgs {
            tile = selectedTile
        });
        selectedTile = null;
    }

    private void SetActionables(Card card) {
        actionableTiles = card.GetActionables();
        foreach (Tile actionable in actionableTiles) actionable.SetActionable();
    }

    private void UnsetActionables() {
        foreach (Tile actionable in actionableTiles) actionable.UnsetActionable();
        actionableTiles.Clear();
    }

    private void SetActionCard(Card card) {
        actionCard = card;
        actionCard.OnActionCallback += Card_OnActionCallback;
    }

    private void UnsetActionCard() {
        if (!actionCard) return;
        actionCard.OnActionCallback -= Card_OnActionCallback;
        actionCard = null;
    }

    private bool IsTileAcionable(Tile tile) {
        foreach (Tile actionable in actionableTiles) {
            if (tile.Equals(actionable)) return true;
        }
        return false;
    }

    private void CancelAction() {
        OnCancelAction?.Invoke(this, new CancelTileArgs { card = actionCard });
        UnsetActionables();
    }

    private void PlacingOnlineCards(InputSystem.PlayerActionType playerActionType) {
        if (!GameBoard.Instance.GetTile(lastMouseWorldPosition, out Tile tile)) return;
        if (tile.GetTeam() != team.Value) return;

            switch (playerActionType) {
            default: break;
            case InputSystem.PlayerActionType.Action:
                PlacingCardsServerRpc(tile.GetComponent<NetworkObject>(), OnlineCardState.Link, OnlineCardState.Virus);
                break;
            case InputSystem.PlayerActionType.SecondaryAction:
                PlacingCardsServerRpc(tile.GetComponent<NetworkObject>(), OnlineCardState.Virus, OnlineCardState.Link);
                break;
        }
    }

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    private void PlacingCardsServerRpc(NetworkObjectReference tileNetworkReference, OnlineCardState onlineCardState1, OnlineCardState onlineCardState2) {
        if (!IsPlacingCards()) return;

        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile tile = tileNetwork.GetComponent<Tile>();

        if (!tile.TryGetComponent(out StartTile startTile)) return;

        if (!tile.GetCard(out Card card)) {
            PlayerEntity playerEntity = GameBoard.Instance.GetPlayerEntityByTeam(team.Value);
            List<OnlineCard> onlineCards = playerEntity.GetOnlineCards();
            List<OnlineCard> onlineLinks = onlineCards.Where(onlineCard => onlineCard.GetServerCardState() == onlineCardState1).ToList();
            if (onlineLinks.Count < playerEntity.GetOnlineCardsCount(onlineCardState1)) startTile.TryPlaceOnlineCard(onlineCardState1, team.Value);
            else startTile.TryPlaceOnlineCard(onlineCardState2, team.Value);
        }
        else if (card is not OnlineCard) return;
        else if ((card as OnlineCard).GetServerCardState() == onlineCardState1) startTile.TryPlaceOnlineCard(onlineCardState2, team.Value);
        else if ((card as OnlineCard).GetServerCardState() == onlineCardState2) startTile.TryPlaceOnlineCard(OnlineCardState.None, team.Value);
    }

    private void WaitingForTurn(InputSystem.PlayerActionType playerActionType) {

    }

    private void PlayingTurn(InputSystem.PlayerActionType playerActionType) {
        if (playerActionType != InputSystem.PlayerActionType.Action) return;
        if (!GameBoard.Instance.GetTile(lastMouseWorldPosition, out Tile tile)) return;
        PlayingTurnServerRpc(tile.GetComponent<NetworkObject>());
    }

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    private void PlayingTurnServerRpc(NetworkObjectReference tileNetworkReference) {
        if (!IsPlayingTurn()) return;

        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile tile = tileNetwork.GetComponent<Tile>();

        if (IsTileAcionable(tile)) {
            OnAction?.Invoke(this, new ActionTileArgs {
                actionTokens = actionTokens.Value,
                tile = tile,
                card = actionCard
            }); ;
            return;
        }

        if (tile.Equals(selectedTile)) {
            CancelAction();
            UnsetActionCard();
            UnsetSelectedTile();
            return;
        }

        tile.OnSelectedValueChanged += Tile_OnSelectedTile;
        OnSelectTile?.Invoke(this, new SelectedTileArgs { tile = tile, team = GetTeam() });
    }

    private void Tile_OnSelectedTile(object sender, Tile.SelectedTileArgs e) {
        if (!IsPlayingTurn()) return;

        e.tile.OnSelectedValueChanged -= Tile_OnSelectedTile;

        if (!e.tile.GetCard(out Card card)) return;
        if (!card.IsUsable()) return;
        if (!e.isSelected) return;

        CancelAction();

        UnsetSelectedTile();
        SetSelectedTile(e.tile);

        UnsetActionables();
        SetActionables(card);
        
        UnsetActionCard();
        SetActionCard(card);
    }

    private void Card_OnActionCallback(object sender, Card.ActionCallbackArgs e) {
        if (!IsPlayingTurn()) return;

        actionTokens.Value -= e.tokenCost;

        UnsetActionables();
        if (e.finished) {
            CancelAction();
            UnsetActionCard();
            UnsetSelectedTile();
        }
        else SetActionables(actionCard);
    }
}
