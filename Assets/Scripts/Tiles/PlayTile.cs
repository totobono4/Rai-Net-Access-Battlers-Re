public class PlayTile : Tile {
    protected override void SelectedTile(object sender, PlayerController.SelectedTileArgs e) {
        if (e.selectedTile != this) return;
        if (HasCard()) OnSelectedTile?.Invoke(this, new SelectedTileArgs { selectedTile = this, isSelected = true });
        else OnSelectedTile?.Invoke(this, new SelectedTileArgs { selectedTile = this, isSelected = false });
    }

    protected override void ActionedTile(object sender, PlayerController.ActionTileArgs e) {
        if (e.actionedTile != this) return;
        if (!actionable) return;
        OnActionedTile?.Invoke(this, new ActionedTileArgs { actionedTile = this });
    }

    protected override void CanceledTile(object sender, PlayerController.CancelTileArgs e) {
        if (e.canceledTile != this) return;
        OnSelectedTile?.Invoke(this, new SelectedTileArgs { selectedTile = null });
    }

    public override void SetActionable() {
        actionable = true;
        OnActionableTile?.Invoke(this, new ActionableTileArgs { actionableTile = this, isActionable = true });
    }

    public override void UnsetActionable() {
        actionable = false;
        OnActionableTile?.Invoke(this, new ActionableTileArgs { actionableTile = this, isActionable = false });
    }
}
