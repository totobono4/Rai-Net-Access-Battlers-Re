using System.Collections.Generic;
using UnityEngine;

public class NotFound : TerminalCard {
    public override void Action(Tile actionable) {
        throw new System.NotImplementedException();
    }

    public override List<Tile> GetActionables(Vector3 worldPosition) {
        throw new System.NotImplementedException();
    }
}
