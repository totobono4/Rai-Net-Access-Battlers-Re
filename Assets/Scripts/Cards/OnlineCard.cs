using System;
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

    [SerializeField] private Type type;
    [SerializeField] private State state;

    public EventHandler<StateChangedArgs> OnStateChanged;
    public class StateChangedArgs {
        public State state;
    }

    private void Start() {
        state = State.Unrevealed;
        OnStateChanged?.Invoke(this, new StateChangedArgs { state = state });
    }

    public override List<Tile> GetActionables(Vector3 worldPosition) {
        return gameBoard.GetNeighbors(worldPosition);
    }

    public override void Action(Tile actionable) {
        SetTileParent(actionable);
    }
}
