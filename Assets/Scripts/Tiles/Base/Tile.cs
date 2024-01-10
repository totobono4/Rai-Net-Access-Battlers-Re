using System;
using UnityEngine;

public abstract class Tile : MonoBehaviour {
    protected PlayerController playerController;
    [SerializeField] private Transform tileCardPoint;
    [SerializeField] private GameBoard.Team teamColor;

    [SerializeField] Transform worldPosition;
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

    public EventHandler<ActionUsedArgs> OnActionUsed;
    public class ActionUsedArgs : EventArgs {
        public Tile usedTile;
        public bool isUsed;
    }

    protected bool selected;
    protected bool actionable;

    protected virtual void Awake() {
        position = worldPosition.position;

        selected = false;
        actionable = false;
    }

    private void Start() {
        if (PlayerController.LocalInstance == null) PlayerController.OnAnyPlayerSpawned += PlayerControllerSpawned;
        else SetPlayerController();
    }

    private void PlayerControllerSpawned(object sender, PlayerController.OnAnyPlayerSpawnedArgs e) {
        if (PlayerController.LocalInstance == null) return;

        SetPlayerController();

        PlayerController.OnAnyPlayerSpawned -= PlayerControllerSpawned;
    }

    private void SetPlayerController() {
        if (PlayerController.LocalInstance == null) return;

        playerController = PlayerController.LocalInstance;

        playerController.OnSelectTile += SelectedTile;
        playerController.OnActionTile += ActionedTile;
        playerController.OnCancelTile += CanceledTile;
    }

    public Vector3 GetPosition() { return position; }

    public GameBoard.Team GetTeam() { return teamColor; }

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

    public void SendActionUsed() {
        OnActionUsed?.Invoke(this, new ActionUsedArgs { usedTile = this, isUsed = true });
    }
    public void SendActioUnused() {
        OnActionUsed?.Invoke(this, new ActionUsedArgs { usedTile = this, isUsed = false });
    }
}
