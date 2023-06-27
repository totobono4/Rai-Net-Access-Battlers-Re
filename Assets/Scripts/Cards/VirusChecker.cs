using System.Collections.Generic;
using UnityEngine;

public class VirusChecker : TerminalCard {
    private bool used;

    private void Awake() {
        used = false;
    }

    public override void Action(Tile actionable) {
        if (!IsTileActionable(actionable, out OnlineCard onlineCard)) {
            SendActionFinishedCallBack();
            return;
        }

        onlineCard.Reveal();
        used = true;
        SendActionFinishedCallBack();
    }

    public override List<Tile> GetActionables() {
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
        if (card.GetTeam() == GetTeam()) return false;
        if (card is not OnlineCard) return false;
        onlineCard = card as OnlineCard;
        if (onlineCard.IsRevealed()) return false;
        return true;
    }
}
