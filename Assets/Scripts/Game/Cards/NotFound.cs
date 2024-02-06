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
        ClientRpcParams clientRpcParams = new ClientRpcParams { Send = { TargetClientIds = ids } };

        SwitchAskClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void SwitchAskClientRpc(ClientRpcParams clientRpcParams) {
        OnSwitchAsking?.Invoke(this, EventArgs.Empty);
    }

    private void GetCardAndParent(Tile usedTile, out Card card, out Tile parent) {
        usedTile.GetCard(out card);
        parent = card.GetTileParent();
    }

    public void Switch(bool switching) {
        SwitchServerRpc(switching);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchServerRpc(bool switching) {
        GetCardAndParent(usedTiles[0], out Card card1, out Tile parent1);
        GetCardAndParent(usedTiles[1], out Card card2, out Tile parent2);

        OnlineCard newCard1 = GameBoard.Instance.CopyOnlineCard(card1 as OnlineCard);
        OnlineCard newCard2 = GameBoard.Instance.CopyOnlineCard(card2 as OnlineCard);

        if (switching) {
            newCard1.SetTileParent(parent2);
            newCard2.SetTileParent(parent1);
        }

        newCard1.SetNotFound();
        newCard2.SetNotFound();

        SetUsed();
        ResetAction();
        SendActionFinishedCallBack(true, GetTokenCost());
    }

    public override void Action(Tile tile, out bool finished, out int tokenCost) {
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
