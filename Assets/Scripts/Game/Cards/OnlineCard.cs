using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class OnlineCard : Card
{
    public static event EventHandler<EventArgs> OnAnyOnlineCardSpawned;

    public enum CardState {
        Unknown,
        Link,
        Virus
    };

    [SerializeField] private CardState serverState;
    [SerializeField] private CardState state;
    private NetworkVariable<bool> revealed;

    [SerializeField] private NeighborMatrixSO neighborMatrixSO;

    public EventHandler<StateChangedArgs> OnStateValueChanged;
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

    [SerializeField] private NetworkVariable<bool> boosted;

    public EventHandler<BoostUpdateArgs> OnBoostedValueChanged;
    public class BoostUpdateArgs : EventArgs {
        public OnlineCard onlineCard;
        public bool boosted;
    }

    [SerializeField] private int defaultRange;
    [SerializeField] private int boostRange;

    public EventHandler OnRevealValueChanged;

    private NetworkVariable<bool> notFound;
    public EventHandler OnNotFoundValueChanged;

    private NetworkVariable<bool> captured;
    public EventHandler OnCapturedValueChanged;

    protected override void Awake() {
        base.Awake();

        revealed = new NetworkVariable<bool>(false);
        revealed.OnValueChanged += Revealed_OnValueChanged;

        boosted = new NetworkVariable<bool>(false);
        boosted.OnValueChanged += Boosted_OnValueChanged;

        notFound = new NetworkVariable<bool>(false);
        notFound.OnValueChanged += NotFound_OnValueChanged;

        captured = new NetworkVariable<bool>(false);
        captured.OnValueChanged += Captured_OnValueChanged;

        state = CardState.Unknown;

        PlayerController.OnTeamChanged += LocalTeamChanged;
    }

    protected override void Start() {
        base.Start();

        StateChanged();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        OnAnyOnlineCardSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void LocalTeamChanged(object sender, PlayerController.TeamChangedArgs e) {
        if (!IsSpawned) return;
        SyncServerStateServerRpc();
    }

    private void Revealed_OnValueChanged(bool previous, bool current) {
        OnRevealValueChanged?.Invoke(this, EventArgs.Empty);
        SyncServerStateServerRpc();
    }

    public void SetServerState(CardState newState) {
        serverState = newState;
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    private void SyncServerStateServerRpc() {
        List<ulong> ids = GameManager.Instance.GetClientIdsByTeam(GetTeam());
        ClientRpcParams clientRpcParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = ids.ToArray() } };
        if (IsRevealed()) SyncServerStateClientRpc(serverState, default);
        else SyncServerStateClientRpc(serverState, clientRpcParams);
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void SyncServerStateClientRpc(CardState newState, ClientRpcParams clientRpcParams) {
        state = newState;
        StateChanged();
    }

    private int GetRange() {
        int range = defaultRange;
        if (boosted.Value) range = boostRange;
        return range;
    }

    public void SetBoosted() {
        boosted.Value = true;
    }

    public void UnsetBoosted() {
        boosted.Value = false;
    }

    public bool IsBoosted() { return boosted.Value; }

    private void Boosted_OnValueChanged(bool previous, bool current) {
        OnBoostedValueChanged?.Invoke(this, new BoostUpdateArgs { onlineCard = this, boosted = current });
    }

    public CardState GetServerCardState() {
        return serverState;
    }

    public CardState GetCardState() {
        return state;
    }

    protected override void Action(Tile tile, out bool finished, out int tokenCost) {
        TryCapture(tile);
        Move(tile);
        if (tile is InfiltrationTile) TryCapture(tile);

        ActionClientRpc();
        
        tokenCost = GetTokenCost();
        finished = true;
        return;
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void ActionClientRpc() {
        SyncCardParent();
    }

    public override bool IsActionable(Tile tile) {
        if (!GetValidNeighborsInRange(GetTileParent(), GetRange()).Contains(tile)) return false;
        if (tile.GetCard(out Card card) && card.GetTeam() == GetTeam()) return false;
        if (tile is ExitTile && tile.GetTeam() == GetTeam()) return false;
        if (tile is BoardTile && (tile as BoardTile).HasFireWall() && (tile as BoardTile).GetFireWall() != GetTeam()) return false;

        return true;
    }

    public List<Tile> GetValidNeighborsInRange(Tile tile, int range) {
        if (range <= 0) return new List<Tile>();

        List<Tile> neighbors = GameBoard.Instance.GetNeighbors(tile.GetPosition(), neighborMatrixSO);
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
        List<Tile> allTiles = GameBoard.Instance.GetAllTiles();
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

    private void Captured_OnValueChanged(bool previousValue, bool newValue) {
        OnCapturedValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Capture() {
        Reveal();
        captured.Value = true;
    }

    public bool IsCaptured() {
        return captured.Value;
    }

    public void Reveal() {
        revealed.Value = true;
        notFound.Value = false;
    }

    public void Unreveal() {
        
    }

    private void StateChanged() {
        OnStateValueChanged?.Invoke(this, new StateChangedArgs { state = state });
    }

    public bool IsRevealed() {
        return revealed.Value;
    }

    public bool IsNotFound() {
        return notFound.Value;
    }

    public void SetNotFound() {
        notFound.Value = true;
    }

    private void NotFound_OnValueChanged(bool previousValue, bool newValue) {
        OnNotFoundValueChanged?.Invoke(this, EventArgs.Empty);
    }
}
