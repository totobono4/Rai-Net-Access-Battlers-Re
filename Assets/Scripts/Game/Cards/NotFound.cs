using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NotFound : TerminalCard {
    private int NEEDED_ACTIONABLES = 2;
    private List<Tile> usedTiles;
    private NetworkVariable<bool> used;

    protected override void Awake() {
        base.Awake();

        usedTiles = new List<Tile>();
        used = new NetworkVariable<bool>(false);
    }

    private void Shuffle() {
        for (int i = 0; i < usedTiles.Count - 1;  i++) {
            int rand = Random.Range(0, 2);

            Card cardForParent1;
            Card cardForParent2;

            if (rand == 0) {
                usedTiles[0].GetCard(out cardForParent1);
                usedTiles[1].GetCard(out cardForParent2);
            } else {
                usedTiles[0].GetCard(out cardForParent2);
                usedTiles[1].GetCard(out cardForParent1);
            }
            
            Tile parent1 = cardForParent1.GetTileParent();
            Tile parent2 = cardForParent2.GetTileParent();

            usedTiles[0].GetCard(out Card card1);
            usedTiles[1].GetCard(out Card card2);

            GameBoard.Instance.CopyOnlineCard(card1 as OnlineCard, parent1);
            GameBoard.Instance.CopyOnlineCard(card2 as OnlineCard, parent2);

            usedTiles.RemoveAt(0);
        }
    }

    private void SetUsed() { used.Value = true; }

    public override bool IsUsable() { return !used.Value; }

    public override void ResetAction() {
        usedTiles = new List<Tile>();
    }

    public override int Action(Tile actionable) {
        usedTiles.Add(actionable);
        if (usedTiles.Count < NEEDED_ACTIONABLES) {
            actionable.SetActionUsed();
            SendActionUnfinishedCallBack();
            return 0;
        }
        else {
            foreach (Tile usedTile in usedTiles) usedTile.UnsetActionUsed();
            Shuffle();
            SetUsed();
            SendActionFinishedCallBack();
            ResetAction();
            return GetActionTokenCost();
        }
    }

    protected override bool IsTileActionable(Tile tile) {
        if (!tile.GetCard(out Card card)) return false;
        if (card.GetTeam() != GetTeam()) return false;
        if (card is not OnlineCard) return false;
        if (usedTiles.Contains(tile)) return false;
        return true;
    }
}
