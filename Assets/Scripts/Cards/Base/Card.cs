using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour {
    protected GameBoard gameBoard;

    private Tile tileParent;

    [SerializeField] private GameBoard.Team team;

    public void SetGameBoard(GameBoard gameBoard) {
        this.gameBoard = gameBoard;
    }

    public GameBoard.Team GetTeam() { return team; }

    public void SetTileParent(Tile tile) {
        if (this.tileParent != null) this.tileParent.ClearCard();

        this.tileParent = tile;
        tile.SetCard(this);

        transform.parent = tile.GetTileCardPointTransform();
        transform.localPosition = Vector3.zero;
    }

    protected Tile GetTileParent() { return tileParent; }

    protected Vector3 GetPosition() {
        return tileParent.GetPosition();
    }

    public abstract List<Tile> GetActionables();
    public abstract void Action(Tile actionable);
}
