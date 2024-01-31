using Unity.Netcode;
using UnityEngine;

public class PlayTile : Tile {
    protected override void PlayerController_OnSelectedTile(object sender, PlayerController.SelectedTileArgs e) {
        if (e.selectedTile != this) return;
        selected.Value = IsSelectedTileValid(e.team);
    }

    // Verify if this tile is a valid tile for selection.
    private bool IsSelectedTileValid(Team controllerTeam) {
        if (!GetCard(out Card card)) return false;
        if (!card.IsUsable()) return false;
        if (!card.GetTeam().Equals(controllerTeam)) return false;
        return true;
    }

    protected override void PlayerController_OnActionedTile(object sender, PlayerController.ActionTileArgs e) {
        if (e.actionedTile != this) return;
        if (!actionable.Value) return;
        OnActionedTile?.Invoke(this, new ActionedTileArgs { actionedTile = this });
    }

    protected override void PlayerController_OnCanceledTile(object sender, PlayerController.CancelTileArgs e) {
        if (e.canceledTile != this) return;
        if (GetCard(out Card card)) card.ResetAction();
        selected.Value = false;
    }
}
