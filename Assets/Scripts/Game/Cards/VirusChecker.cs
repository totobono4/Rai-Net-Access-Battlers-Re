using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VirusChecker : TerminalCard {
    private NetworkVariable<bool> used;

    protected override void Awake() {
        base.Awake();

        used = new NetworkVariable<bool>(false);
    }

    private void SetUsed() { used.Value = true; }
    public override bool IsUsable() { return !used.Value; }

    public override int Action(Tile actionable) {
        if (!actionable.GetCard(out Card card)) return 0;
        OnlineCard onlineCard = card as OnlineCard;

        onlineCard.Reveal();
        SetUsed();
        SendActionFinishedCallBack();
        return GetActionTokenCost();
    }

    protected override bool IsTileActionable(Tile tile) {
        if (!tile.GetCard(out Card card)) return false;
        if (card.GetTeam() == GetTeam()) return false;
        if (card is not OnlineCard) return false;
        if ((card as OnlineCard).IsRevealed()) return false;
        return true;
    }
}
