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

        if (!activated.Value) {
            onlineCard.OnBoostUpdate += BoostUpdate;
            onlineCard.SetBoost();
        }
        else onlineCard.UnsetBoost();
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
        if (!activated.Value)
        {
            if (!tile.GetCard(out Card card)) return false;
            if (card.GetTeam() != GetTeam()) return false;
            if (card is not OnlineCard) return false;
            onlineCard = card as OnlineCard;
        }
        else {
            if (!tile.GetCard(out Card card)) return false;
            if (card.GetTeam() != GetTeam()) return false;
            if (card is not OnlineCard) return false;
            if ((card as OnlineCard).IsBoosted() == false) return false;
            onlineCard = card as OnlineCard;
        }
        return true;
    }
}
