using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTile : Tile {
    protected override void SelectedTile(object sender, PlayerController.SelectedTileArgs e) {
        if (e.selectedTile == this) {
            if (HasCard()) {
                OnSelectedTile?.Invoke(this, new SelectedTileArgs { selectedTile = this, isSelected = true });
            } else {
                OnSelectedTile?.Invoke(this, new SelectedTileArgs { selectedTile = this, isSelected = false });
            }
        }
    }

    protected override void ActionedTile(object sender, PlayerController.ActionTileArgs e) {
        if (e.actionedTile == this && actionable) {
            OnActionedTile?.Invoke(this, new ActionedTileArgs { actionedTile = this });
        }
    }

    protected override void CanceledTile(object sender, PlayerController.CancelTileArgs e) {
        if (e.canceledTile == this) {
            OnSelectedTile?.Invoke(this, new SelectedTileArgs { selectedTile = null });
        }
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
