using System;
using Unity.Netcode;
using UnityEngine;

public abstract class Tile : NetworkBehaviour {
    [SerializeField] private Transform tileCardPoint;
    [SerializeField] private PlayerTeam teamColor;

    [SerializeField] Transform worldPosition;
    private Vector3 position;
    [SerializeField] private Card card;

    public EventHandler<SelectedTileArgs> OnSelectedValueChanged;
    public class SelectedTileArgs : EventArgs {
        public Tile tile;
        public bool isSelected;
    }

    public EventHandler<ActionableTileArgs> OnActionableValueChanged;
    public class ActionableTileArgs : EventArgs {
        public Tile tile;
        public bool isActionable;
    }

    public EventHandler<ActionUsedArgs> OnActionUsedValueChanged;
    public class ActionUsedArgs : EventArgs {
        public Tile tile;
        public bool isUsed;
    }

    protected NetworkVariable<bool> selected;
    protected NetworkVariable<bool> actionable;
    protected NetworkVariable<bool> actionUsed;

    protected virtual void Awake() {
        selected = new NetworkVariable<bool>(false);
        selected.OnValueChanged += Selected_OnValueChanged;

        actionable = new NetworkVariable<bool>(false);
        actionable.OnValueChanged += Actionable_OnValueChanged;

        actionUsed = new NetworkVariable<bool>(false);
        actionUsed.OnValueChanged += ActionUsed_OnValueChanged;

        PlayerController.OnSelectTile += PlayerController_OnSelectedTile;
        PlayerController.OnCancelAction += PlayerController_OnCancelAction;
    }

    private void Start() {
        position = worldPosition.position;

        if (PlayerController.LocalInstance == null) PlayerController.OnAnyPlayerSpawned += PlayerController_OnAnyPlayerSpawned;
    }

    private void PlayerController_OnAnyPlayerSpawned(object sender, PlayerController.AnyPlayerSpawnedArgs e) {
        if (PlayerController.LocalInstance == null) return;

        PlayerController.OnAnyPlayerSpawned -= PlayerController_OnAnyPlayerSpawned;
    }

    public Vector3 GetPosition() { return position; }

    public PlayerTeam GetTeam() { return teamColor; }

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

    protected abstract void PlayerController_OnSelectedTile(object sender, PlayerController.SelectedTileArgs e);
    protected abstract void PlayerController_OnCancelAction(object sender, PlayerController.CancelTileArgs e);

    private void Selected_OnValueChanged(bool previous, bool current) {
        OnSelectedValueChanged?.Invoke(this, new SelectedTileArgs { tile = this, isSelected = current });
    }

    public void SetActionable() {
        actionable.Value = true;
    }

    public void UnsetActionable() {
        actionable.Value = false;
    }

    private void Actionable_OnValueChanged(bool previous, bool current) {
        OnActionableValueChanged?.Invoke(this, new ActionableTileArgs { tile = this, isActionable = current });
    }

    public void SetActionUsed() {
        actionUsed.Value = true;
    }

    public void UnsetActionUsed() {
        actionUsed.Value = false;
    }

    private void ActionUsed_OnValueChanged(bool previous, bool current) {
        OnActionUsedValueChanged?.Invoke(this, new ActionUsedArgs { tile = this, isUsed = current });
    }
}
