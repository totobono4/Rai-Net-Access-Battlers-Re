using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour {
    public static PlayerController LocalInstance { get; private set; }

    [SerializeField] private LayerMask tileLayerMask;

    [SerializeField] private NetworkVariable<Team> team;

    private Vector3 lastMouseWorldPosition;
    private Tile selectedTile;
    private List<Tile> actionableTiles;
    private Card actionCard;

    public EventHandler<HoverTileChangedArgs> OnHoverTileChanged;
    public class HoverTileChangedArgs : EventArgs {
        public Tile hoverTile;
    }
    public static EventHandler<SelectedTileArgs> OnSelectTile;
    public class SelectedTileArgs : EventArgs {
        public Team team;
        public Tile selectedTile;
    }
    public static EventHandler<ActionTileArgs> OnActionTile;
    public class ActionTileArgs : EventArgs {
        public Tile actionedTile;
    }
    public static EventHandler<CancelTileArgs> OnCancelTile;
    public class CancelTileArgs : EventArgs {
        public Tile canceledTile;
    }

    public static event EventHandler<AnyPlayerSpawnedArgs> OnAnyPlayerSpawned;
    public class AnyPlayerSpawnedArgs : EventArgs {
        public ulong clientId;
    }

    public static EventHandler<TeamChangedArgs> OnTeamChanged;
    public class TeamChangedArgs : EventArgs {
        public Team team;
    }

    private NetworkVariable<PlayerState> playerState;

    private enum PlayerState {
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
        playerState = new NetworkVariable<PlayerState>(PlayerState.WaitingForTurn);
        playerState.OnValueChanged += PlayerState_OnValueChanged;

        actionTokens = new NetworkVariable<int>(0);
        actionTokens.OnValueChanged += ActionTokens_OnValueChanged;

        selectedTile = null;
        actionableTiles = new List<Tile>();
        actionCard = null;

        team = new NetworkVariable<Team>();
        team.Value = Team.None;
        team.OnValueChanged += Team_OnValueChanged;
    }

    private void Start() {
        OnlineCard.OnAnyOnlineCardSpawned += OnlineCard_OnAnyOnlineCardSpawned;
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

        InputSystem.Instance.OnPlayerAction += PlayerAction;
        GameManager.Instance.OnPlayerGivePriority += PlayerGivePriority;

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

    [ServerRpc(RequireOwnership = false)]
    private void InitializeServerRpc(ServerRpcParams serverRpcParams = default) {
        team.Value = MultiplayerManager.Instance.GetClientTeamById(serverRpcParams.Receive.SenderClientId);
    }

    private void Team_OnValueChanged(Team previous, Team current) {
        OnTeamChanged?.Invoke(this, new TeamChangedArgs { team = current });
    }

    private void OnlineCard_OnAnyOnlineCardSpawned(object sender, EventArgs e) {
        OnTeamChanged?.Invoke(this, new TeamChangedArgs { team = team.Value });
    }

    public Team GetTeam() { return team.Value; }

    private void SendEventHoverTileChanged() {
        Vector3 mouseWorldPosition = InputSystem.Instance.GetMouseWorldPosition();
        if (GameBoard.Instance.GetTile(mouseWorldPosition, out Tile tile)) {
            OnHoverTileChanged?.Invoke(this, new HoverTileChangedArgs {
                hoverTile = tile
            });
            return;
        }
        OnHoverTileChanged?.Invoke(this, new HoverTileChangedArgs {
            hoverTile = null
        });
    }

    private void PlayerGivePriority(object sender, GameManager.PlayerGivePriorityArgs e) {
        if (e.team == team.Value) playerState.Value = PlayerState.SelectingForAction;
        else playerState.Value = PlayerState.WaitingForTurn;
        actionTokens.Value = e.actionTokens;
    }

    private void ActionTokens_OnValueChanged(int previous, int current) {
        if (!IsServer) return;
        GameManager.Instance.PassPriority(actionTokens.Value);
    }

    private void PlayerAction(object sender, EventArgs e) {
        if (!IsOwner) return;

        lastMouseWorldPosition = InputSystem.Instance.GetMouseWorldPosition();

        switch (playerState.Value) {
            default: break;
            case PlayerState.WaitingForTurn:
                WaitingForTurn();
                break;
            case PlayerState.SelectingForAction:
                SelectingForAction();
                break;
            case PlayerState.ThinkingForAction:
                ThinkingForAction();
                break;
        }
    }

    // On attend notre tour.
    private void WaitingForTurn() {

    }

    // On essaies de sélectionner une case.
    private void SelectingForAction() {
        if (!GameBoard.Instance.GetTile(lastMouseWorldPosition, out Tile tile)) return;
        SelectingForActionServerRpc(tile.GetComponent<NetworkObject>());
    }

    [ServerRpc]
    private void SelectingForActionServerRpc(NetworkObjectReference tileNetworkReference) {
        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile tile = tileNetwork.GetComponent<Tile>();

        tile.OnSelectedTile += TileSelected;
        OnSelectTile?.Invoke(this, new SelectedTileArgs { selectedTile = tile, team = GetTeam() });
    }

    // On tente de faire une action.
    private void ThinkingForAction() {
        if (!GameBoard.Instance.GetTile(lastMouseWorldPosition, out Tile tile)) return;
        ThinkingForActionServerRpc(tile.GetComponent<NetworkObject>());
    }

    [ServerRpc]
    private void ThinkingForActionServerRpc(NetworkObjectReference tileNetworkReference) {
        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile tile = tileNetwork.GetComponent<Tile>();

        if (tile.Equals(selectedTile)) {
            foreach (Tile actionable in actionableTiles) actionable.UnsetActionable();
            OnCancelTile?.Invoke(this, new CancelTileArgs { canceledTile = tile });
            playerState.Value = PlayerState.SelectingForAction;
        }
        else {
            tile.OnActionedTile += TileActioned;
            OnActionTile?.Invoke(this, new ActionTileArgs { actionedTile = tile });
        }
    }

    // Si la case est sélectionable, alors on bascule vers la réflexion.
    private void TileSelected(object sender, Tile.SelectedTileArgs e) {
        e.selectedTile.OnSelectedTile -= TileSelected;

        if (!e.selectedTile.GetCard(out Card card)) return;
        if (!card.IsUsable()) return;
        if (!e.isSelected) return;

        selectedTile = e.selectedTile;
        playerState.Value = PlayerState.ThinkingForAction;

        actionableTiles = card.GetActionables();
        foreach (Tile actionable in actionableTiles) actionable.SetActionable();
    }

    // Si on peut faire une action on la fait, et on appelle le callback d'action pour savoir s'il reste des choses à faire.
    private void TileActioned(object sender, Tile.ActionedTileArgs e) {
        e.actionedTile.OnActionedTile -= TileActioned;

        if (selectedTile == null) return;
        if (!selectedTile.GetCard(out Card card)) return;

        actionCard = card;
        actionCard.OnActionCallback += ActionCallback;

        actionTokens.Value -= actionCard.TryAction(actionTokens.Value, e.actionedTile);
    }

    // Si il reste des choses à faire, on se prépare à continuer, sinon on termine l'action et on fini le tour.
    private void ActionCallback(object sender, Card.ActionCallbackArgs e) {
        foreach (Tile actionable in actionableTiles) actionable.UnsetActionable();
        if (e.actionFinished) {
            actionCard.OnActionCallback -= ActionCallback;
            OnCancelTile?.Invoke(this, new CancelTileArgs { canceledTile = selectedTile });
            selectedTile = null;
        }
        else {
            if (!selectedTile.GetCard(out Card card)) return;

            actionableTiles = card.GetActionables();
            foreach (Tile actionable in actionableTiles) actionable.SetActionable();
        }
    }
}
