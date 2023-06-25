using System.Collections.Generic;
using UnityEngine;

public class FireWall : TerminalCard {
    public override void Action(Tile actionable) {
        throw new System.NotImplementedException();
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
        if (tile.GetCard(out Card card) && card.GetTeam() != GetTeam()) return false;
        if (tile is not BoardTile) return false;
        if (tile is ExitTile) return false;
        
        return true;
    }
}
