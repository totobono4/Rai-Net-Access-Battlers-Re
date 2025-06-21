using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Card : NetworkBehaviour {
    [SerializeField] private Tile tileParent;
    public EventHandler<TileParentChangedArgs> OnTileParentChanged;
    public class TileParentChangedArgs : EventArgs {
        public Tile tile;
        public PlayerTeam team;
    }

    [SerializeField] private PlayerTeam team;

    [SerializeField] private int actionTokenCost;

    public EventHandler<ActionCallbackArgs> OnActionCallback;
    public class ActionCallbackArgs : EventArgs {
        public Card card;
        public bool finished;
        public int tokenCost;
    }

    public EventHandler OnClean;

    protected virtual void Awake() {

    }

    protected virtual void Start() {
        PlayerController.OnAction += PlayerController_OnAction;
        PlayerController.OnCancelAction += PlayerController_OnCancelAction;
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

    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    private void SyncCardParentServerRpc() {
        if (tileParent == null) return;
        NetworkObject tileParentNetwork = tileParent.GetComponent<NetworkObject>();
        SyncCardParentClientRpc(tileParentNetwork);
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void SyncCardParentClientRpc(NetworkObjectReference tileNetworkReference) {
        tileNetworkReference.TryGet(out NetworkObject tileNetwork);
        SetTileParent(tileNetwork.GetComponent<Tile>());
    }

    public PlayerTeam GetTeam() {
        return team;
    }

    public void SetTileParent(Tile tile) {
        if (tileParent != null) tileParent.ClearCard();
        tileParent = tile;
        if (tile == null) return;

        tile.SetCard(this);

        transform.position = tile.GetTileCardPointTransform().position;
        transform.rotation = tile.GetTileCardPointTransform().rotation;

        OnTileParentChanged?.Invoke(this, new TileParentChangedArgs { tile = tile, team = team });
    }

    public Tile GetTileParent() {
        return tileParent;
    }

    protected Vector3 GetPosition() {
        return tileParent.GetPosition();
    }

    public void SendActionFinishedCallBack(bool finished, int tokenCost) {
        OnActionCallback?.Invoke(this, new ActionCallbackArgs {
            card = this,
            finished = finished,
            tokenCost = tokenCost
        });
    }

    public int GetTokenCost() {
        return actionTokenCost;
    }

    protected abstract void Action(Tile tile, out bool finished, out int tokenCost);

    protected void TryAction(int actionTokens, Tile tile, out bool finished, out int tokenCost) {
        tokenCost = 0;
        finished = true;

        if (actionTokens < GetTokenCost() || !IsActionable(tile)) return;

        Action(tile, out finished, out tokenCost);
        return;
    }

    public List<Tile> GetActionables() {
        List<Tile> allTiles = GameBoard.Instance.GetAllTiles();
        List<Tile> actionableTiles = new List<Tile>();
        foreach (Tile tile in allTiles) {
            if (!IsActionable(tile)) continue;
            actionableTiles.Add(tile);
        }
        return actionableTiles;
    }
    public virtual void ResetAction() { }
    public virtual bool IsUsable() { return true; }
    public abstract bool IsActionable(Tile tile);

    private void PlayerController_OnAction(object sender, PlayerController.ActionTileArgs e) {
        if (e.card != this) return;
        if (!IsActionable(e.tile)) return;

        TryAction(e.actionTokens, e.tile, out bool finished, out int tokenCost);

        SendActionFinishedCallBack(finished, tokenCost);
    }
    public void PlayerController_OnCancelAction(object sender, PlayerController.CancelTileArgs e) {
        if ((object)e.card != this) return;
        ResetAction();
    }

    public virtual void Clean() {
        PlayerController.OnAction -= PlayerController_OnAction;
        PlayerController.OnCancelAction -= PlayerController_OnCancelAction;

        OnClean?.Invoke(this, EventArgs.Empty);
    }
}
