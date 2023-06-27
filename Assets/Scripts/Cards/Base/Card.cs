using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour {
    protected GameBoard gameBoard;

    private Tile tileParent;

    [SerializeField] private GameBoard.Team team;

    public EventHandler<ActionCallbackArgs> OnActionCallback;
    public class ActionCallbackArgs : EventArgs {
        public bool actionFinished;
    }

    public void SetGameBoard(GameBoard gameBoard) {
        this.gameBoard = gameBoard;
    }

    public GameBoard.Team GetTeam() { return team; }

    public void SetTileParent(Tile tile) {
        if (tileParent != null) tileParent.ClearCard();
        tileParent = tile;
        if (tile == null) return;

        tile.SetCard(this);

        transform.parent = tile.GetTileCardPointTransform();
        transform.localPosition = Vector3.zero;
    }

    public Tile GetTileParent() { return tileParent; }

    protected Vector3 GetPosition() {
        return tileParent.GetPosition();
    }

    protected void SendActionFinishedCallBack() {
        OnActionCallback?.Invoke(this, new ActionCallbackArgs { actionFinished = true });
    }
    protected void SendActionUnfinishedCallBack() {
        OnActionCallback?.Invoke(this, new ActionCallbackArgs { actionFinished = false });
    }
    public abstract List<Tile> GetActionables();
    public abstract void Action(Tile actionable);
    public virtual void ResetAction() { }
}
