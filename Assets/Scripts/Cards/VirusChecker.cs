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

    public override void Action(Tile actionable) {
        ActionServerRpc(actionable.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void ActionServerRpc(NetworkObjectReference tileNetworkReference) {
        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile actionable = tileNetwork.GetComponent<Tile>();

        if (!IsTileActionable(actionable, out OnlineCard onlineCard)) {
            ActionClientRpc();
            return;
        }

        onlineCard.Reveal();
        SetUsed();
        ActionClientRpc();
    }

    [ClientRpc]
    private void ActionClientRpc() {
        SendActionFinishedCallBack(null);
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
