namespace RaiNet.Game {
    public class VirusChecker : TerminalCard {
        protected override void Action(Tile tile, out bool finished, out int tokenCost) {
            tokenCost = 0; finished = false;

            if (!tile.GetCard(out Card card)) return;
            OnlineCard onlineCard = card as OnlineCard;

            onlineCard.Reveal();
            SetUsed();

            tokenCost = GetTokenCost();
            finished = true;
        }

        public override bool IsActionable(Tile tile) {
            if (!tile.GetCard(out Card card)) return false;
            if (card.GetTeam() == GetTeam()) return false;
            if (card is not OnlineCard) return false;
            if ((card as OnlineCard).IsRevealed()) return false;
            return true;
        }

        public override bool IsUsable() {
            return !used.Value;
        }
    }
}