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

    [SerializeField] private NeighborMatrixSO neighborMatrixSO;

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

    public override void Action(Tile actioned) {
        if (!IsTileActionable(actioned)) return;
        TryCapture(actioned);
        Move(actioned);
        if (actioned is InfiltrationTile) TryCapture(actioned);
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
        if (tile is not InfiltrationTile && !GetNeighbors().Contains(tile)) return false;
        if (tile is InfiltrationTile && tile.GetTeam() == GetTeam()) return false;
        if (tile is InfiltrationTile && GetTileParent() is not ExitTile) return false;
        if (tile.GetCard(out Card card) && card.GetTeam() == GetTeam()) return false;
        if (tile is ExitTile && tile.GetTeam() == GetTeam()) return false;
        return true;
    }

    private List<Tile> GetNeighbors() {
        return gameBoard.GetNeighbors(GetPosition(), neighborMatrixSO);
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

    public void Reveal() {
        state = CardState.Revealed;
        StateChanged();
    }

    private void StateChanged() {
        OnStateChanged?.Invoke(this, new StateChangedArgs { state = state });
    }

    public bool IsRevealed() {
        return CardState.Revealed == state;
    }
}
