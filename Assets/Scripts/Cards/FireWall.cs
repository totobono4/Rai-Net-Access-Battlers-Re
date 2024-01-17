using System.Collections.Generic;
using Unity.Netcode;

public class FireWall : TerminalCard {
    private NetworkVariable<bool> activated;

    protected override void Awake() {
        base.Awake();

        activated = new NetworkVariable<bool>(false);
    }

    public override int Action(Tile actionable) {
        activated.Value = !activated.Value;

        if (activated.Value) (actionable as BoardTile).SetFireWall(GetTeam());
        else (actionable as BoardTile).UnsetFireWall();

        SendActionFinishedCallBack();
        return GetActionTokenCost();
    }

    protected override bool IsTileActionable(Tile tile) {
        if (!activated.Value) {
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
