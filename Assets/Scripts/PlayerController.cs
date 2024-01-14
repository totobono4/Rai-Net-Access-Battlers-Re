using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour {
    public static PlayerController LocalInstance { get; private set; }
    public static event EventHandler<OnAnyPlayerSpawnedArgs> OnAnyPlayerSpawned;
    public class OnAnyPlayerSpawnedArgs : EventArgs {
        public ulong clientId;
    }

    public static EventHandler<OnTeamChangedArgs> OnTeamChanged;
    public class OnTeamChangedArgs : EventArgs {
        public GameBoard.Team team;
    }

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
    public EventHandler<SelectedTileArgs> OnSelectTile;
    public class SelectedTileArgs : EventArgs {
        public Tile selectedTile;
    }
    public EventHandler<ActionTileArgs> OnActionTile;
    public class ActionTileArgs : EventArgs {
        public Tile actionedTile;
    }
    public EventHandler<CancelTileArgs> OnCancelTile;
    public class CancelTileArgs : EventArgs {
        public Tile canceledTile;
    }

    private enum PlayerState {
        WaitingForTurn,
        SelectingForAction,
        ThinkingForAction
    }

    private PlayerState playerState;

    private void Awake() {
        playerState = PlayerState.WaitingForTurn;
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

        if (IsOwner) {
            LocalInstance = this;
            SetPlayerTeamServerRpc(OwnerClientId);
        }

        OnAnyPlayerSpawned?.Invoke(this, new OnAnyPlayerSpawnedArgs {
            clientId = OwnerClientId
        });

        PlayerStart();
    }

    private void TeamChanged(GameBoard.Team previous, GameBoard.Team current) {
        OnTeamChanged?.Invoke(this, new OnTeamChangedArgs { team = current });
    }

    private void OnlineCardSpawned(object sender, EventArgs e) {
        OnTeamChanged?.Invoke(this, new OnTeamChangedArgs { team = team.Value });
    }

    private void Update() {
        if (!IsOwner) return;

        SendEventHoverTileChanged();
    }

    public void PlayerStart() {
        playerState = PlayerState.SelectingForAction;
    }

    [ServerRpc]
    private void SetPlayerTeamServerRpc(ulong clientId) {
        team.Value = gameBoard.PickTeam(clientId);
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

    private void PlayerAction(object sender, EventArgs e) {
        if (!IsOwner) return;

        lastMouseWorldPosition = inputSystem.GetMouseWorldPosition();

        switch (playerState) {
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

        tile.OnSelectedTile += OnTileSelected;
        OnSelectTile?.Invoke(this, new SelectedTileArgs { selectedTile = tile });
    }

    // On tente de faire une action.
    private void ThinkingForAction() {
        if (!gameBoard.GetTile(lastMouseWorldPosition, out Tile tile)) return;

        if (tile.Equals(selectedTile)) {
            foreach (Tile actionable in actionableTiles) actionable.UnsetActionable();
            OnCancelTile?.Invoke(this, new CancelTileArgs { canceledTile = tile });
            playerState = PlayerState.SelectingForAction;
        }
        else {
            tile.OnActionedTile += OnTileActioned;
            OnActionTile?.Invoke(this, new ActionTileArgs { actionedTile = tile });
        }
    }

    // Si la case est sélectionable, alors on bascule vers la réflexion.
    private void OnTileSelected(object sender, Tile.SelectedTileArgs e) {
        e.selectedTile.OnSelectedTile -= OnTileSelected;
        if (!e.selectedTile.GetCard(out Card card)) return;
        if (!card.IsUsable()) return;
        if (!e.isSelected) return;

        selectedTile = e.selectedTile;
        actionableTiles = card.GetActionables();
        foreach (Tile actionable in actionableTiles) actionable.SetActionable();
        playerState = PlayerState.ThinkingForAction;
    }

    // Si on peut faire une action on la fait, et on appelle le callback d'action pour savoir s'il reste des chsoes à faire.
    private void OnTileActioned(object sender, Tile.ActionedTileArgs e) {
        e.actionedTile.OnActionedTile -= OnTileActioned;
        if (selectedTile == null) return;
        if (!selectedTile.GetCard(out Card card)) return;

        actionCard = card;
        actionCard.OnActionCallback += ActionCallback;
        actionCard.Action(e.actionedTile);
    }

    // Si il reste des choses à faire, on se prépare à continuer, sinon on termine l'action et on fini le tour.
    private void ActionCallback(object sender, Card.ActionCallbackArgs e) {
        foreach (Tile actionable in actionableTiles) actionable.UnsetActionable();
        if (e.actionFinished) {
            actionCard.OnActionCallback -= ActionCallback;
            OnCancelTile?.Invoke(this, new CancelTileArgs { canceledTile = selectedTile });
            selectedTile = null;
            playerState = PlayerState.SelectingForAction;
        }
        else {
            if (!selectedTile.GetCard(out Card card)) return;

            actionableTiles = card.GetActionables();
            foreach (Tile actionable in actionableTiles) actionable.SetActionable();
        }
    }
}
