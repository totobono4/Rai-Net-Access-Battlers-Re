using System;
using Unity.Netcode;
using UnityEngine;

public abstract class Tile : NetworkBehaviour {
    [SerializeField] private Transform tileCardPoint;
    [SerializeField] private GameBoard.Team teamColor;

    [SerializeField] Transform worldPosition;
    private Vector3 position;
    [SerializeField] private Card card;

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

    protected NetworkVariable<bool> selected;
    protected NetworkVariable<bool> actionable;
    private NetworkVariable<bool> actionUsed;

    protected virtual void Awake() {
        selected = new NetworkVariable<bool>(false);
        selected.OnValueChanged += SelectedChanged;

        actionable = new NetworkVariable<bool>(false);
        actionable.OnValueChanged += ActionableChanged;

        actionUsed = new NetworkVariable<bool>(false);
        actionUsed.OnValueChanged += ActionUsedChanged;

        PlayerController.OnSelectTile += SelectedTile;
        PlayerController.OnActionTile += ActionedTile;
        PlayerController.OnCancelTile += CanceledTile;
    }

    private void Start() {
        position = worldPosition.position;

        if (PlayerController.LocalInstance == null) PlayerController.OnAnyPlayerSpawned += PlayerControllerSpawned;
    }

    private void PlayerControllerSpawned(object sender, PlayerController.AnyPlayerSpawnedArgs e) {
        if (PlayerController.LocalInstance == null) return;

        PlayerController.OnAnyPlayerSpawned -= PlayerControllerSpawned;
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

    private void SelectedChanged(bool previous, bool current) {
        OnSelectedTile?.Invoke(this, new SelectedTileArgs { selectedTile = this, isSelected = current });
    }

    public void SetActionable() {
        actionable.Value = true;
    }

    public void UnsetActionable() {
        actionable.Value = false;
    }

    private void ActionableChanged(bool previous, bool current) {
        OnActionableTile?.Invoke(this, new ActionableTileArgs { actionableTile = this, isActionable = current });
    }

    public void SetActionUsed() {
        actionUsed.Value = true;
    }

    public void UnsetActionUsed() {
        actionUsed.Value = false;
    }

    private void ActionUsedChanged(bool previous, bool current) {
        OnActionUsed?.Invoke(this, new ActionUsedArgs { usedTile = this, isUsed = current });
    }
}
