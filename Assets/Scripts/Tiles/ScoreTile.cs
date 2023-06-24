public class ScoreTile : Tile {
    public override void SetActionable() {
        return;
    }

    public override void UnsetActionable() {
        return;
    }

    protected override void ActionedTile(object sender, PlayerController.ActionTileArgs e) {
        return;
    }

    protected override void CanceledTile(object sender, PlayerController.CancelTileArgs e) {
        return;
    }

    protected override void SelectedTile(object sender, PlayerController.SelectedTileArgs e) {
        return;
    }
}
