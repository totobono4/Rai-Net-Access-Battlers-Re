using System;
using System.Collections.Generic;
using Unity.Netcode;

public class NotFound : TerminalCard {
    private int NEEDED_ACTIONABLES = 2;
    private List<Tile> usedTiles;

    public EventHandler OnSwitchAsking;

    protected override void Awake() {
        base.Awake();

        usedTiles = new List<Tile>();
    }

    private void SwitchAsk() {
        List<ulong> ids = GameManager.Instance.GetClientIdsByTeam(GetTeam());

        for (int i = 0; i < ids.Count; i++) {
            ulong id = ids[i];
            SwitchAskClientRpc(RpcTarget.Single(id, RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams, Delivery = RpcDelivery.Reliable)]
    private void SwitchAskClientRpc(RpcParams rpcParams) {
        OnSwitchAsking?.Invoke(this, EventArgs.Empty);
    }

    private void GetCardAndParent(Tile usedTile, out Card card, out Tile parent) {
        usedTile.GetCard(out card);
        parent = card.GetTileParent();
    }

    public void Switch(bool switching) {
        SwitchServerRpc(switching);
    }

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    public void SwitchServerRpc(bool switching) {
        if (IsUsed()) return;

        GetCardAndParent(usedTiles[0], out Card card1, out Tile parent1);
        GetCardAndParent(usedTiles[1], out Card card2, out Tile parent2);

        if (switching) {
            card1.SetTileParent(parent2);
            card2.SetTileParent(parent1);
            card1.SetTileParent(parent2);
            card1.SyncCardParent();
            card2.SyncCardParent();
        }

        (card1 as OnlineCard).SetNotFound();
        (card2 as OnlineCard).SetNotFound();

        SetUsed();
        ResetAction();
        SendActionFinishedCallBack(true, GetTokenCost());
    }

    protected override void Action(Tile tile, out bool finished, out int tokenCost) {
        tokenCost = 0; finished = false;

        tile.SetActionUsed();

        usedTiles.Add(tile);
        if (usedTiles.Count < NEEDED_ACTIONABLES) return;

        SwitchAsk();
        return;
    }

    public override void ResetAction() {
        foreach (Tile usedTile in usedTiles) usedTile.UnsetActionUsed();
        usedTiles = new List<Tile>();
    }

    public override bool IsActionable(Tile tile) {
        if (!tile.GetCard(out Card card)) return false;
        if (card.GetTeam() != GetTeam()) return false;
        if (card is not OnlineCard) return false;
        if (usedTiles.Contains(tile)) return false;
        return true;
    }

    public override bool IsUsable() {
        return !used.Value;
    }
}
