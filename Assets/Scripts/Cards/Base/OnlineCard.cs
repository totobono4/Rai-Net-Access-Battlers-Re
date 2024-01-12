using System;
using System.Collections.Generic;
using System.Linq;
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
    public class StateChangedArgs : EventArgs {
        public CardState state;
    }

    public EventHandler<MoveCardArgs> OnMoveCard;
    public class MoveCardArgs : EventArgs {
        public OnlineCard movingCard;
        public Tile moveTarget;
    }

    public EventHandler<CaptureCardArgs> OnCaptureCard;
    public class CaptureCardArgs : EventArgs {
        public OnlineCard capturedCard;
        public OnlineCard capturingCard;
    }

    private bool boosted;

    public EventHandler<BoostUpdateArgs> OnBoostUpdate;
    public class BoostUpdateArgs : EventArgs {
        public OnlineCard onlineCard;
        public bool boosted;
    }

    [SerializeField] private int defaultRange;
    [SerializeField] private int boostRange;

    protected override void Awake() {
        base.Awake();

        boosted = false;
    }

    private int GetRange() {
        int range = defaultRange;
        if (boosted) range = boostRange;
        return range;
    }

    public void SetBoost() {
        boosted = true;
        BoostChanged();
    }

    public void UnsetBoost() {
        boosted = false;
        BoostChanged();
    }

    public bool IsBoosted() { return boosted; }

    private void BoostChanged() {
        OnBoostUpdate?.Invoke(this, new BoostUpdateArgs { onlineCard = this, boosted = boosted });
    }

    private void Start() {
        state = CardState.Unrevealed;
        StateChanged();
    }

    public CardType GetCardType() {
        return type;
    }

    public override void Action(Tile actioned) {
        if (!IsTileActionable(actioned)) {
            SendActionFinishedCallBack();
            return;
        }

        TryCapture(actioned);
        Move(actioned);
        if (actioned is InfiltrationTile) TryCapture(actioned);
        SendActionFinishedCallBack();
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
        if (!GetValidNeighborsInRange(GetTileParent(), GetRange()).Contains(tile)) return false;
        if (tile.GetCard(out Card card) && card.GetTeam() == GetTeam()) return false;
        if (tile is ExitTile && tile.GetTeam() == GetTeam()) return false;
        if (tile is BoardTile && (tile as BoardTile).HasFireWall() && (tile as BoardTile).GetFireWall() != GetTeam()) return false;

        //Debug.Log(GetTileParent().GetPosition());
        //Debug.Log(tile.GetPosition());

        return true;
    }

    public List<Tile> GetValidNeighborsInRange(Tile tile, int range) {
        if (range <= 0) return new List<Tile>();

        List<Tile> neighbors = gameBoard.GetNeighbors(tile.GetPosition(), neighborMatrixSO);
        List<Tile> validNeighbors = new List<Tile>();
        List<Tile> result = new List<Tile>();

        foreach (Tile neighbor in neighbors) {
            if (!IsNeighborValid(neighbor)) continue;
            validNeighbors.Add(neighbor);
            result.Add(neighbor);
        }

        foreach (Tile neighbor in validNeighbors) {
            if (!IsNextNeighborsValid(neighbor)) continue;
            result = result.Union(GetValidNeighborsInRange(neighbor, range - 1)).ToList();
        }

        if (tile is ExitTile && tile.GetTeam() != GetTeam()) result.Add(GetInfiltrationTile(tile as ExitTile));

        return result;
    }

    private bool IsNeighborValid(Tile neighbor) {
        if (neighbor.GetCard(out Card card) && card.GetTeam() == GetTeam()) return false;
        if (neighbor is BoardTile && (neighbor as BoardTile).HasFireWall() && (neighbor as BoardTile).GetFireWall() != GetTeam()) return false;
        if (neighbor is ExitTile && neighbor.GetTeam() == GetTeam()) return false;
        return true;
    }

    private bool IsNextNeighborsValid(Tile neighbor) {
        if (neighbor.HasCard()) return false;
        return true;
    }

    private Tile GetInfiltrationTile(ExitTile exitTile) {
        List<Tile> allTiles = gameBoard.GetAllTiles();
        Tile infiltrationTile = null;
        foreach (Tile tile in allTiles) if (tile is InfiltrationTile && tile.GetTeam() == exitTile.GetTeam()) infiltrationTile = tile;
        return infiltrationTile;
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
        if (boosted) UnsetBoost();
        StateChanged();
    }

    public void Reveal() {
        state = CardState.Revealed;
        StateChanged();
    }

    public void Unreveal() {
        state = CardState.Unrevealed;
        StateChanged();
    }

    private void StateChanged() {
        OnStateChanged?.Invoke(this, new StateChangedArgs { state = state });
    }

    public bool IsRevealed() {
        return CardState.Revealed == state;
    }
}
