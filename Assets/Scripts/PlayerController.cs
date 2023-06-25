using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController Instance { get; private set; }

    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private GameBoard gameBoard;
    [SerializeField] private GameBoard.Team teamColor;

    private Vector3 lastMouseWorldPosition;
    private Tile selectedTile;
    private List<Tile> actionableTiles;

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
        Instance = this;

        inputSystem.OnPlayerAction += PlayerAction;

        playerState = PlayerState.WaitingForTurn;
        selectedTile = null;

        PlayerStart();
    }

    private void Update() {
        SendEventHoverTileChanged();
    }

    public void PlayerStart() {
        playerState = PlayerState.SelectingForAction;
    }

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

    private void WaitingForTurn() {

    }

    private void SelectingForAction() {
        if (gameBoard.GetTile(lastMouseWorldPosition, out Tile tile)) {
            tile.OnSelectedTile += OnTileSelected;
            OnSelectTile?.Invoke(this, new SelectedTileArgs { selectedTile = tile });
        }
    }

    private void ThinkingForAction() {
        if (gameBoard.GetTile(lastMouseWorldPosition, out Tile tile)) {
            if (tile == selectedTile) {
                foreach (Tile actionable in actionableTiles) actionable.UnsetActionable();
                OnCancelTile?.Invoke(this, new CancelTileArgs { canceledTile = tile });
                playerState = PlayerState.SelectingForAction;
            }
            else {
                tile.OnActionedTile += OnTileActioned;
                OnActionTile?.Invoke(this, new ActionTileArgs { actionedTile = tile });
            }
        }
    }

    private void OnTileSelected(object sender, Tile.SelectedTileArgs e) {
        e.selectedTile.OnSelectedTile -= OnTileSelected;
        if (e.isSelected) {
            selectedTile = e.selectedTile;
            if (selectedTile.GetCard(out Card card)) {
                actionableTiles = card.GetActionables();
            }
            foreach (Tile actionable in actionableTiles) actionable.SetActionable();
            playerState = PlayerState.ThinkingForAction;
        }
    }

    private void OnTileActioned(object sender, Tile.ActionedTileArgs e) {
        e.actionedTile.OnActionedTile -= OnTileActioned;
        if (selectedTile != null && selectedTile.GetCard(out Card card)) {
            card.Action(e.actionedTile);
        }
        foreach (Tile actionable in actionableTiles) actionable.UnsetActionable();
        OnCancelTile?.Invoke(this, new CancelTileArgs { canceledTile = selectedTile });
        selectedTile = null;
        playerState = PlayerState.SelectingForAction;
    }
}
