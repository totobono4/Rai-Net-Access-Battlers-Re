using System.Collections.Generic;
using UnityEngine;

public class NotFound : TerminalCard {
    public override void Action(Tile actionable) {
        throw new System.NotImplementedException();
    }

    public override List<Tile> GetActionables(Vector3 worldPosition) {
        List<Tile> allTiles = gameBoard.GetAllTiles();
        List<Tile> actionableTiles = new List<Tile>();
        foreach (Tile tile in allTiles) {
            if (!IsTileActionable(tile, out _)) continue;
            actionableTiles.Add(tile);
        }
        return actionableTiles;
    }

    private bool IsTileActionable(Tile tile, out OnlineCard onlineCard) {
        onlineCard = null;
        if (!tile.GetCard(out Card card)) return false;
        if (card.GetTeam() != GetTeam()) return false;
        if (card is not OnlineCard) return false;
        onlineCard = card as OnlineCard;
        return true;
    }
}
