using System;

public class LineBoost : TerminalCard {
    protected override void Action(Tile tile, out bool finished, out int tokenCost) {
        tokenCost = 0; finished = false;
        if (!tile.GetCard(out Card card)) return;
        OnlineCard onlineCard = card as OnlineCard;

        if (!used.Value) Boost(onlineCard);
        else Unboost(onlineCard);

        tokenCost = GetTokenCost();
        finished = true;
        return;
    }

    private void Boost(OnlineCard onlineCard) {
        onlineCard.SetBoosted();
        SetUsed();
        onlineCard.OnCapturedValueChanged += OnlineCard_OnCapturedValueChanged;
    }

    private void Unboost(OnlineCard onlineCard) {
        onlineCard.UnsetBoosted();
        UnsetUsed();
        onlineCard.OnCapturedValueChanged -= OnlineCard_OnCapturedValueChanged;
    }

    private void OnlineCard_OnCapturedValueChanged(object sender, EventArgs e) {
        if ((sender as OnlineCard).IsCaptured()) Unboost(sender as OnlineCard);
    }

    public override bool IsActionable(Tile tile) {
        if (!tile.GetCard(out Card card)) return false;
        if (card.GetTeam() != GetTeam()) return false;
        if (card is not OnlineCard) return false;
        if (used.Value && !(card as OnlineCard).IsBoosted()) return false;

        return true;
    }

    public override void Clean() {
        if (GameBoard.Instance != null) {
            foreach (Tile tile in GameBoard.Instance.GetAllTiles()) {
                if (tile.GetCard(out Card card) && card is OnlineCard onlineCard) {
                    onlineCard.OnCapturedValueChanged -= OnlineCard_OnCapturedValueChanged;
                }
            }
        }
        base.Clean();
    }
}
