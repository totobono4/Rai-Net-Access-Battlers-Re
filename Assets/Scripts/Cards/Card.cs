using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour {
    protected GameBoard gameBoard;

    private Tile tile;

    [SerializeField] public Player.Team team;

    public void SetGameBoard(GameBoard gameBoard) {
        this.gameBoard = gameBoard;
    }

    public void SetTileParent(Tile tile) {
        if (this.tile != null) this.tile.ClearCard();

        this.tile = tile;
        tile.SetCard(this);

        transform.parent = tile.GetTileCardPointTransform();
        transform.localPosition = Vector3.zero;
    }

    public abstract List<Tile> GetActionables(Vector3 worldPosition);
    public abstract void Action(Tile actionable);
}
