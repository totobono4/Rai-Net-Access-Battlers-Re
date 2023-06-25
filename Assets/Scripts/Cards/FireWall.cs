using System.Collections.Generic;

public class FireWall : TerminalCard {
    private bool activated;

    private void Awake() {
        activated = false;
    }

    public override void Action(Tile actionable) {
        if (!IsTileActionable(actionable)) return;
        if (!activated) {
            (actionable as BoardTile).SetFireWall(GetTeam());
            activated = true;
        }
        else {
            (actionable as BoardTile).UnsetFireWall();
            activated = false;
        }
    }

    public override List<Tile> GetActionables() {
        List<Tile> allTiles = gameBoard.GetAllTiles();
        List<Tile> actionableTiles = new List<Tile>();
        foreach (Tile tile in allTiles) {
            if (!IsTileActionable(tile)) continue;
            actionableTiles.Add(tile);
        }
        return actionableTiles;
    }

    private bool IsTileActionable(Tile tile) {
        if (!activated) {
            if (tile.GetCard(out Card card) && card.GetTeam() != GetTeam()) return false;
            if (tile is not BoardTile) return false;
            if ((tile as BoardTile).HasFireWall()) return false;
            if (tile is ExitTile) return false;
        }
        else {
            if (tile is not BoardTile) return false;
            if (!(tile as BoardTile).HasFireWall()) return false;
            if ((tile as BoardTile).GetFireWall() != GetTeam()) return false;
        }
        return true;
    }
}
