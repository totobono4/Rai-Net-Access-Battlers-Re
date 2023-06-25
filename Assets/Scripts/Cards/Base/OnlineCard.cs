using System;
using System.Collections.Generic;
using UnityEngine;

public class OnlineCard : Card
{
    public enum CardType {
        Link,
        Virus
    };

    public enum CardState {
        Revealed,
        Unrevealed,
        Captured
    }

    [SerializeField] private CardType type;
    [SerializeField] private CardState state;

    public EventHandler<StateChangedArgs> OnStateChanged;
    public class StateChangedArgs {
        public CardState state;
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
        state = CardState.Unrevealed;
        StateChanged();
    }

    public CardType GetCardType() {
        return type;
    }

    public override List<Tile> GetActionables(Vector3 worldPosition) {
        List<Tile> neighbors = gameBoard.GetNeighbors(worldPosition);
        List<Tile> actionableTiles = new List<Tile>();
        foreach (Tile tile in neighbors) {
            if (tile.GetCard(out Card card) && card.GetTeam() == GetTeam()) continue;
            if (tile is ExitTile && tile.GetTeam() == GetTeam()) continue;
            actionableTiles.Add(tile);
        }
        return actionableTiles;
    }

    public override void Action(Tile actioned) {
        TryCapture(actioned);
        Move(actioned);
        if (actioned is ExitTile) TryCapture(actioned);
    }

    private void TryCapture(Tile tile) {
        if (!tile.GetCard(out Card card)) return;
        if (card is not OnlineCard) return;
        OnCaptureCard?.Invoke(this, new CaptureCardArgs { capturedCard = card as OnlineCard, capturingCard = this });
    }

    private void Move(Tile tile) {
        OnMoveCard?.Invoke(this, new MoveCardArgs { movingCard = this, moveTarget = tile });
    }

    public void Capture() {
        state = CardState.Revealed;
        StateChanged();
    }

    private void StateChanged() {
        OnStateChanged?.Invoke(this, new StateChangedArgs { state = state });
    }
}
