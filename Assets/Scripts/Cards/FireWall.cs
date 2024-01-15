using System.Collections.Generic;
using Unity.Netcode;

public class FireWall : TerminalCard {
    private bool activated;

    protected override void Awake() {
        base.Awake();

        BoardTile.OnFireWallUpdate += FireWallUpdate;

        activated = false;
    }

    private void FireWallUpdate(object sender, BoardTile.FireWallUpdateArgs e) {
        activated = e.boardTile.HasFireWall();
    }

    public override void Action(Tile actionable) {
        ActionServerRpc(actionable.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void ActionServerRpc(NetworkObjectReference tileNetworkReference) {
        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        Tile actionable = tileNetwork.GetComponent<Tile>();

        if (!IsTileActionable(actionable)) {
            ActionClientRpc();
            return;
        }

        if (!activated) (actionable as BoardTile).SetFireWall(GetTeam());
        else (actionable as BoardTile).UnsetFireWall();
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
            if (!IsTileActionable(tile)) continue;
            actionableTiles.Add(tile);
        }
        return actionableTiles;
    }

    private bool IsTileActionable(Tile tile) {
        if (!activated) {
            if (tile.GetCard(out Card card) && card.GetTeam() != GetTeam()) return false;
            if (tile is not BoardTile) return false;
            if ((tile as BoardTile).HasFireWall()) return false;
            if (tile is ExitTile) return false;
        }
        else {
            if (tile is not BoardTile) return false;
            if (!(tile as BoardTile).HasFireWall()) return false;
            if ((tile as BoardTile).GetFireWall() != GetTeam()) return false;
        }
        return true;
    }
}
