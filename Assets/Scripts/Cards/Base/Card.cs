using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Card : NetworkBehaviour {
    protected GameBoard gameBoard;

    [SerializeField] private Tile tileParent;
    public EventHandler<TileParentChangedArgs> OnTileParentChanged;
    public class TileParentChangedArgs : EventArgs {
        public Tile tile;
        public GameBoard.Team team;
    }

    [SerializeField] private GameBoard.Team team;

    public EventHandler<ActionCallbackArgs> OnActionCallback;
    public class ActionCallbackArgs : EventArgs {
        public bool actionFinished;
        public Tile actioned;
    }

    [SerializeField] private int actionTokenCost;

    protected virtual void Awake() {
        gameBoard = GameBoard.Instance;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsClient) return;
        if (IsHost) return;

        SyncCardParent();
    }

    public void SyncCardParent() {
        SyncCardParentServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SyncCardParentServerRpc() {
        if (tileParent == null) return;
        NetworkObject tileParentNetwork = tileParent.GetComponent<NetworkObject>();
        SyncCardParentClientRpc(tileParentNetwork);
    }

    [ClientRpc]
    private void SyncCardParentClientRpc(NetworkObjectReference tileNetworkReference) {
        tileNetworkReference.TryGet(out NetworkObject tileNetwork);
        SetTileParent(tileNetwork.GetComponent<Tile>());
    }

    public GameBoard.Team GetTeam() { return team; }

    public void SetTileParent(Tile tile) {
        if (tileParent != null) tileParent.ClearCard();
        tileParent = tile;
        if (tile == null) return;

        tile.SetCard(this);

        transform.position = tile.GetTileCardPointTransform().position;
        transform.rotation = tile.GetTileCardPointTransform().rotation;

        OnTileParentChanged?.Invoke(this, new TileParentChangedArgs { tile = tile, team = team });
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
    protected abstract bool IsTileActionable(Tile tile);
    public List<Tile> GetActionables() {
        List<Tile> allTiles = gameBoard.GetAllTiles();
        List<Tile> actionableTiles = new List<Tile>();
        foreach (Tile tile in allTiles) {
            if (!IsTileActionable(tile)) continue;
            actionableTiles.Add(tile);
        }
        return actionableTiles;
    }
    public abstract int Action(Tile actionable);
    public virtual void ResetAction() { }
    public virtual bool IsUsable() { return true; }

    public int TryAction(int actionTokens, Tile actionable) {
        if (actionTokens < GetActionTokenCost() || !IsTileActionable(actionable)) {
            SendActionFinishedCallBack();
            return 0;
        }
        return Action(actionable);
    }

    public int GetActionTokenCost() { return actionTokenCost;}
}
