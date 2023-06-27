using System.Collections.Generic;
using UnityEngine;

public class NotFound : TerminalCard {
    private int NEEDED_ACTIONABLES = 2;
    private List<Tile> actionables;

    private void Awake() {
        actionables = new List<Tile>();
    }

    private void Shuffle() {
        for (int i = 0; i < actionables.Count; i++) {
            int rand = Random.Range(0, actionables.Count);
            actionables[i].GetCard(out Card card1);
            actionables[rand].GetCard(out Card card2);

            Tile parent1 = card1.GetTileParent();
            Tile parent2 = card2.GetTileParent();
            card1.SetTileParent(null);
            card2.SetTileParent(null);
            card1.SetTileParent(parent2);
            card2.SetTileParent(parent1);
        }
    }

    private void Unreveal() {
        foreach (Tile actionable in actionables) {
            if (!actionable.GetCard(out Card card)) continue;
            if (card is not OnlineCard) continue;
            (card as OnlineCard).Unreveal();
        }
    }

    public override void ResetAction() {
        foreach (Tile actionable in actionables) actionable.SendActioUnused();
        actionables = new List<Tile>();
    }

    public override void Action(Tile actionable) {
        if (!IsTileActionable(actionable)) {
            SendActionFinishedCallBack();
            return;
        }

        actionable.SendActionUsed();

        actionables.Add(actionable);
        if (actionables.Count < NEEDED_ACTIONABLES) SendActionUnfinishedCallBack();
        else {
            Unreveal();
            Shuffle();
            SendActionFinishedCallBack();
            ResetAction();
        }
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
        if (!tile.GetCard(out Card card)) return false;
        if (card.GetTeam() != GetTeam()) return false;
        if (card is not OnlineCard) return false;
        if (actionables.Contains(tile)) return false;
        return true;
    }
}
