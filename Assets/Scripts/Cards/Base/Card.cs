using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Card : NetworkBehaviour {
    protected GameBoard gameBoard;

    [SerializeField] private Tile tileParent;

    [SerializeField] private GameBoard.Team team;

    public EventHandler<ActionCallbackArgs> OnActionCallback;
    public class ActionCallbackArgs : EventArgs {
        public bool actionFinished;
    }

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
    public virtual bool IsUsable() { return true; }
}
