using System.Collections.Generic;

public class LineBoost : TerminalCard {
    private bool activated;

    private void Awake() {
        activated = false;
    }

    private void BoostUpdate(object sender, OnlineCard.BoostUpdateArgs e) {
        if (!e.boosted) e.onlineCard.OnBoostUpdate -= BoostUpdate;
        activated = e.boosted;
    }

    public override void Action(Tile actionable) {
        if (!IsTileActionable(actionable, out OnlineCard onlineCard)) return;
        if (!activated) {
            onlineCard.OnBoostUpdate += BoostUpdate;
            onlineCard.SetBoost();
        }
        else onlineCard.UnsetBoost();
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
        if (!activated)
        {
            if (!tile.GetCard(out Card card)) return false;
            if (card.GetTeam() != GetTeam()) return false;
            if (card is not OnlineCard) return false;
            onlineCard = card as OnlineCard;
        }
        else {
            if (!tile.GetCard(out Card card)) return false;
            if (card is not OnlineCard) return false;
            if ((card as OnlineCard).IsBoosted() == false) return false;
            onlineCard = card as OnlineCard;
        }
        return true;
    }
}
