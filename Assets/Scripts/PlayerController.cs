using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
using UnityEngine;

public class PlayerController : NetworkBehaviour {
    public static PlayerController LocalInstance { get; private set; }

    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private GameBoard gameBoard;
    [SerializeField] private NetworkVariable<GameBoard.Team> team;

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
        public GameBoard.Team team;
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

    public static EventHandler<PlayerReadyArgs> OnPlayerReady;
    public class PlayerReadyArgs : EventArgs {
        public ulong clientId;
    }

    public static EventHandler<TeamChangedArgs> OnTeamChanged;
    public class TeamChangedArgs : EventArgs {
        public GameBoard.Team team;
    }

    private NetworkVariable<PlayerState> playerState;

    private enum PlayerState {
        WaitingForTurn,
        SelectingForAction,
        ThinkingForAction
    }

    public static EventHandler<EventArgs> OnActionFinished;
    public class ActionFinishedArgs : EventArgs {
        public int actionCost;
    }

    private NetworkVariable<int> actionTokens;
    private int actionCost;

    private void Awake() {
        playerState = new NetworkVariable<PlayerState>(PlayerState.WaitingForTurn);

        actionTokens = new NetworkVariable<int>(0);
        actionTokens.OnValueChanged += ActionTokenChanged;

        selectedTile = null;
        actionableTiles = new List<Tile>();
        actionCard = null;

        team = new NetworkVariable<GameBoard.Team>();
        team.Value = GameBoard.Team.None;
        team.OnValueChanged += TeamChanged;

        OnlineCard.OnAnyOnlineCardSpawned += OnlineCardSpawned;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        inputSystem = InputSystem.Instance;
        gameBoard = GameBoard.Instance;

        inputSystem.OnPlayerAction += PlayerAction;
        gameBoard.OnPlayerGivePriority += PlayerGivePriority;

        if (IsOwner) {
            LocalInstance = this;
            SetPlayerTeamServerRpc(OwnerClientId);
        }

        OnAnyPlayerSpawned?.Invoke(this, new AnyPlayerSpawnedArgs {
            clientId = OwnerClientId
        });
    }

    private void Update() {
        if (!IsOwner) return;

        SendEventHoverTileChanged();
    }

    [ServerRpc]
    private void SetPlayerTeamServerRpc(ulong clientId) {
        team.Value = gameBoard.PickTeam(clientId);
    }

    private void TeamChanged(GameBoard.Team previous, GameBoard.Team current) {
        OnTeamChanged?.Invoke(this, new TeamChangedArgs { team = current });
        PlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        OnPlayerReady?.Invoke(this, new PlayerReadyArgs { clientId = serverRpcParams.Receive.SenderClientId });
    }

    private void OnlineCardSpawned(object sender, EventArgs e) {
        OnTeamChanged?.Invoke(this, new TeamChangedArgs { team = team.Value });
    }

    public GameBoard.Team GetTeam() { return team.Value; }

    private void SendEventHoverTileChanged() {
        Vector3 mouseWorldPosition = inputSystem.GetMouseWorldPosition();
        if (gameBoard.GetTile(mouseWorldPosition, out Tile tile)) {
            OnHoverTileChanged?.Invoke(this, new HoverTileChangedArgs {
                hoverTile = tile
            });
            return;
        }
        OnHoverTileChanged?.Invoke(this, new HoverTileChangedArgs {
            hoverTile = null
        });
    }

    private void PlayerGivePriority(object sender, GameBoard.PlayerGivePriorityArgs e) {
        if (e.team == team.Value) playerState.Value = PlayerState.SelectingForAction;
        else playerState.Value = PlayerState.WaitingForTurn;
        actionTokens.Value = e.actionTokens;
    }

    public bool IsPlaying() {
        return playerState.Value != PlayerState.WaitingForTurn;
    }

    private void ActionTokenChanged(int previous, int current) {
        if (!IsServer) return;
        gameBoard.PassPriority(actionTokens.Value);
    }

    private void PlayerAction(object sender, EventArgs e) {
        if (!IsOwner) return;

        lastMouseWorldPosition = inputSystem.GetMouseWorldPosition();

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
        if (!gameBoard.GetTile(lastMouseWorldPosition, out Tile tile)) return;
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
        if (!gameBoard.GetTile(lastMouseWorldPosition, out Tile tile)) return;
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
