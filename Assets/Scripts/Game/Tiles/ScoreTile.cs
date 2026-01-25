using System;

public class ScoreTile : Tile {
    protected override void PlayerController_OnSelectedTile(object sender, PlayerController.SelectedTileArgs e) {
        return;
    }
    protected override void PlayerController_OnCancelSelection(object sender, PlayerController.CancelSelectionArgs e) {
        return;
    }
}
