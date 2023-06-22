using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public EventHandler<MoveCardArgs> OnMoveCard;
    public class MoveCardArgs {
        public OnlineCard movingCard;
        public Tile moveTarget;
    }

    public EventHandler<CaptureCardArgs> OnCaptureCard;
    public class CaptureCardArgs {
        public OnlineCard capturedCard;
        public OnlineCard capturingCard;
    }

    private void Start() {
        state = State.Unrevealed;
        OnStateChanged?.Invoke(this, new StateChangedArgs { state = state });
    }

    public Type GetCardType() {
        return type;
    }

    public override List<Tile> GetActionables(Vector3 worldPosition) {
        List<Tile> neighbors = gameBoard.GetNeighbors(worldPosition);
        List<Tile> actionableTiles = new List<Tile>();
        foreach (Tile tile in neighbors) {
            if (tile.GetCard(out Card card) && card.GetTeam() == GetTeam()) continue;
            actionableTiles.Add(tile);
        }
        return actionableTiles;
    }

    public override void Action(Tile actionable) {
        TryCapture(actionable);
        Move(actionable);
    }

    private void TryCapture(Tile tile) {
        if (!tile.GetCard(out Card card)) return;
        if (card.GetTeam() == GetTeam()) return;
        if (card is not OnlineCard) return;
        OnCaptureCard?.Invoke(this, new CaptureCardArgs { capturedCard = card as OnlineCard, capturingCard = this });
    }

    private void Move(Tile tile) {
        OnMoveCard?.Invoke(this, new MoveCardArgs { movingCard = this, moveTarget = tile });
    }
}
