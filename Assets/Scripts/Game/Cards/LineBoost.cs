public class LineBoost : TerminalCard {
    public override void Action(Tile tile, out bool finished, out int tokenCost) {
        tokenCost = 0; finished = false;
        if (!tile.GetCard(out Card card)) return;
        OnlineCard onlineCard = card as OnlineCard;

        if (!used.Value) {
            onlineCard.SetBoosted();
            SetUsed();
        }
        else {
            onlineCard.UnsetBoosted();
            UnsetUsed();
        }

        tokenCost = GetTokenCost();
        finished = true;
        return;
    }

    public override bool IsActionable(Tile tile) {
        if (!tile.GetCard(out Card card)) return false;
        if (card.GetTeam() != GetTeam()) return false;
        if (card is not OnlineCard) return false;
        if (used.Value && !(card as OnlineCard).IsBoosted()) return false;

        return true;
    }
}
