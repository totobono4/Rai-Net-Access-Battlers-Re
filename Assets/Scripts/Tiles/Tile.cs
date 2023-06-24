using System;
using UnityEngine;

public abstract class Tile : MonoBehaviour {
    private PlayerController playerController;
    [SerializeField] private Transform tileCardPoint;
    [SerializeField] private GameBoard.TeamColor teamColor;

    private Vector3 position;
    private Card card;

    public EventHandler<SelectedTileArgs> OnSelectedTile;
    public class SelectedTileArgs : EventArgs {
        public Tile selectedTile;
        public bool isSelected;
    }

    public EventHandler<ActionedTileArgs> OnActionedTile;
    public class ActionedTileArgs : EventArgs {
        public Tile actionedTile;
    }

    public EventHandler<ActionableTileArgs> OnActionableTile;
    public class ActionableTileArgs : EventArgs {
        public Tile actionableTile;
        public bool isActionable;
    }

    protected bool selected;
    protected bool actionable;

    private void Awake() {
        selected = false;
        actionable = false;
    }

    private void Start() {
        playerController = PlayerController.Instance;

        playerController.OnSelectTile += SelectedTile;
        playerController.OnActionTile += ActionedTile;
        playerController.OnCancelTile += CanceledTile;
    }

    public Vector2 GetPosition() {
        return position;
    }

    public void SetCard(Card card) {
        this.card = card;
    }

    public bool GetCard(out Card card) {
        card = this.card;
        return card != null;
    }

    public void ClearCard() {
        card = null;
    }

    public bool HasCard() {
        return card != null;
    }

    public Transform GetTileCardPointTransform() {
        return tileCardPoint;
    }

    protected abstract void SelectedTile(object sender, PlayerController.SelectedTileArgs e);

    protected abstract void ActionedTile(object sender, PlayerController.ActionTileArgs e);

    protected abstract void CanceledTile(object sender, PlayerController.CancelTileArgs e);

    public abstract void SetActionable();
    public abstract void UnsetActionable();
}
