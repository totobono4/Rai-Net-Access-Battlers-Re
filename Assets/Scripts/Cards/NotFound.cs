using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NotFound : TerminalCard {
    private int NEEDED_ACTIONABLES = 2;
    private List<Tile> usedTiles;
    private NetworkVariable<bool> used;

    protected override void Awake() {
        base.Awake();

        usedTiles = new List<Tile>();
        used = new NetworkVariable<bool>(false);
    }

    private void Shuffle() {
        for (int i = 0; i < usedTiles.Count - 1;  i++) {
            int rand = Random.Range(0, 2);

            Card cardForParent1;
            Card cardForParent2;

            if (rand == 0) {
                usedTiles[0].GetCard(out cardForParent1);
                usedTiles[1].GetCard(out cardForParent2);
            } else {
                usedTiles[0].GetCard(out cardForParent2);
                usedTiles[1].GetCard(out cardForParent1);
            }
            
            Tile parent1 = cardForParent1.GetTileParent();
            Tile parent2 = cardForParent2.GetTileParent();

            usedTiles[0].GetCard(out Card card1);
            usedTiles[1].GetCard(out Card card2);

            Card newCard1 = gameBoard.CopyOnlineCard(card1 as OnlineCard, parent1);
            Card newCard2 = gameBoard.CopyOnlineCard(card2 as OnlineCard, parent2);

            usedTiles.RemoveAt(0);
        }
    }

    private void Unreveal() {
        foreach (Tile actionable in usedTiles) {
            if (!actionable.GetCard(out Card card)) continue;
            if (card is not OnlineCard) continue;
            (card as OnlineCard).Unreveal();
        }
    }

    private void SetUsed() { used.Value = true; }

    public override bool IsUsable() { return !used.Value; }

    public override void ResetAction() {
        usedTiles = new List<Tile>();
    }

    public override void Action(Tile actionable) {
        ActionServerRpc(actionable.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void ActionServerRpc(NetworkObjectReference tileNetworkReference) {
        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile actionable = tileNetwork.GetComponent<Tile>();

        if (!IsTileActionable(actionable)) {
            ActionClientRpc(true, actionable.GetComponent<NetworkObject>());
            return;
        }

        usedTiles.Add(actionable);
        if (usedTiles.Count < NEEDED_ACTIONABLES) ActionClientRpc(false, actionable.GetComponent<NetworkObject>());
        else {
            Unreveal();
            Shuffle();
            SetUsed();
            ActionClientRpc(true, actionable.GetComponent<NetworkObject>());
            ResetAction();
        }
    }

    [ClientRpc]
    private void ActionClientRpc(bool actionFinished, NetworkObjectReference tileNetworkReference) {
        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile actioned = tileNetwork.GetComponent<Tile>();

        if (actionFinished) SendActionFinishedCallBack(actioned);
        else {
            if (!IsHost) usedTiles.Add(actioned);
            SendActionUnfinishedCallBack(actioned);
        }
    }

    public override List<Tile> GetActionables() {
        List<Tile> allTiles = gameBoard.GetAllTiles();
        List<Tile> actionableTiles = new List<Tile>();
        foreach (Tile tile in allTiles) {
            if (!IsTileActionable(tile)) continue;
            actionableTiles.Add(tile);
        }
        return actionableTiles;
    }

    private bool IsTileActionable(Tile tile) {
        if (!tile.GetCard(out Card card)) return false;
        if (card.GetTeam() != GetTeam()) return false;
        if (card is not OnlineCard) return false;
        if (usedTiles.Contains(tile)) return false;
        return true;
    }
}
