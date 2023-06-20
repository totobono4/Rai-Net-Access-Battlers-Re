using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineCard : Card
{
    public enum Type {
        Link,
        Virus
    };

    public enum State {
        Revealed,
        Unrevealed,
        Captured
    }

    [SerializeField] public Type type;
    [SerializeField] public State state;

    public override List<Tile> GetActionables(Vector3 worldPosition) {
        return gameBoard.GetNeighbors(worldPosition);
    }

    public override void Action(Tile actionable) {
        SetTileParent(actionable);
    }
}
