public class PlayTile : Tile {
    private bool IsSelectedTileValid(Team controllerTeam) {
        if (!GetCard(out Card card)) return false;
        if (!card.IsUsable()) return false;
        if (!card.GetTeam().Equals(controllerTeam)) return false;
        return true;
    }

    protected override void PlayerController_OnSelectedTile(object sender, PlayerController.SelectedTileArgs e) {
        if (e.tile != this) return;
        selected.Value = IsSelectedTileValid(e.team);
    }

    protected override void PlayerController_OnCancelAction(object sender, PlayerController.CancelTileArgs e) {
        if (e.tile != this) return;
        selected.Value = false;
    }
}
