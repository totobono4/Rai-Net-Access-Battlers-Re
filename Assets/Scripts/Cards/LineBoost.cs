using System.Collections.Generic;
using Unity.Netcode;

public class LineBoost : TerminalCard {
    private NetworkVariable<bool> activated;

    protected override void Awake() {
        base.Awake();

        activated = new NetworkVariable<bool>();
        activated.Value = false;
    }

    private void BoostUpdate(object sender, OnlineCard.BoostUpdateArgs e) {
        if (!e.boosted) e.onlineCard.OnBoostUpdate -= BoostUpdate;
        activated.Value = e.boosted;
    }

    public override int Action(Tile actionable) {
        if (!actionable.GetCard(out Card card)) return 0;
        OnlineCard onlineCard = card as OnlineCard;

        if (!activated.Value) {
            onlineCard.OnBoostUpdate += BoostUpdate;
            onlineCard.SetBoost();
        }
        else onlineCard.UnsetBoost();
        SendActionFinishedCallBack();
        return GetActionTokenCost();
    }

    protected override bool IsTileActionable(Tile tile) {
        if (!activated.Value)
        {
            if (!tile.GetCard(out Card card)) return false;
            if (card.GetTeam() != GetTeam()) return false;
            if (card is not OnlineCard) return false;
        }
        else {
            if (!tile.GetCard(out Card card)) return false;
            if (card.GetTeam() != GetTeam()) return false;
            if (card is not OnlineCard) return false;
            if ((card as OnlineCard).IsBoosted() == false) return false;
        }
        return true;
    }
}
