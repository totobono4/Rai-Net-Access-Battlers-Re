using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerEntity : NetworkBehaviour {
    [SerializeField] private PlayerTeam team;

    [SerializeField] private PlayerOnlineCardsSO playerOnlineCardsSO;
    Transform onlineCardPrefab;
    Dictionary<OnlineCardState, int> onlineCardCounts;

    [SerializeField] private List<OnlineCardState> onlineCardTypes;
    [SerializeField] private List<ScoreSlotGroup> scoreSlotsGroups;
    private Dictionary<OnlineCardState, ScoreSlotGroup> scoreSlotsGroupDict = new Dictionary<OnlineCardState, ScoreSlotGroup>();

    private NetworkVariable<int> linkScore;
    private NetworkVariable<int> virusScore;

    [SerializeField] private TerminalGroup terminalGroup;
    [SerializeField] private InfiltrationGroup infiltrationGroup;

    private List<OnlineCard> onlineCards;
    private NetworkVariable<bool> areOnlineCardsPlaced;

    public static EventHandler<OnlineCardsPlacedArgs> OnOnlineCardsPlaced;
    public class OnlineCardsPlacedArgs : EventArgs {
        public PlayerEntity playerEntity;
        public bool onlineCardsPlaced;
    }

    private NetworkVariable<bool> areCardsReady;

    public static EventHandler<CardsReadyArgs> OnCardsReady;
    public class CardsReadyArgs : EventArgs {
        public PlayerEntity playerEntity;
        public bool cardsReady;
    }

    private void Awake() {
        onlineCardPrefab = playerOnlineCardsSO.GetPrefab();
        onlineCardCounts = playerOnlineCardsSO.GetCounts();

        for (int i = 0; i < onlineCardTypes.Count; i++) scoreSlotsGroupDict.Add(onlineCardTypes[i], scoreSlotsGroups[i]);

        linkScore = new NetworkVariable<int>();
        virusScore = new NetworkVariable<int>();

        linkScore.Value = 0;
        virusScore.Value = 0;

        onlineCards = new List<OnlineCard>();

        areOnlineCardsPlaced = new NetworkVariable<bool>(false);
        areOnlineCardsPlaced.OnValueChanged += AreOnlineCardsPlaced_OnValueChanged;

        areCardsReady = new NetworkVariable<bool>(false);
        areCardsReady.OnValueChanged += AreCardsReady_OnValueChanged;
    }

    public PlayerTeam GetTeam() {
        return team;
    }

    public bool GetWin(out bool winState) {
        winState = default;
        if (linkScore.Value >= 4) {
            winState = true;
            return true;
        }
        if (virusScore.Value >= 4) {
            winState = false;
            return true;
        }
        return false;
    }

    public List<TileMap> GetTileMaps() {
        List<TileMap> tileMaps = new List<TileMap>();
        foreach (TileMap tileMap in scoreSlotsGroupDict.Values) { tileMaps.Add(tileMap); }
        tileMaps.Add(terminalGroup);
        tileMaps.Add(infiltrationGroup);
        return tileMaps;
    }

    public bool TryPlaceOnlineCard(StartTile startTile, OnlineCardState onlineCardState) {
        if (AreCardsReady()) return false;

        if (startTile.IsOnlineCardPlaced()) {
            OnlineCard onlinecardAlreadyPlaced = onlineCards.Where(onlineCard => onlineCard.GetTileParent() == startTile).FirstOrDefault();
            if (onlinecardAlreadyPlaced == null) return false;

            onlinecardAlreadyPlaced.GetTileParent().ClearCard();
            onlineCards.Remove(onlinecardAlreadyPlaced);
            onlinecardAlreadyPlaced.GetComponent<NetworkObject>().Despawn();

            UpdateAreOnlineCardsPlaced();
            return false;
        }

        if (onlineCards.Where(onlinecard => onlinecard.GetServerCardState() == onlineCardState).ToList().Count >= onlineCardCounts[onlineCardState]) return false;
        if (startTile.GetTeam() != team) return false;

        Transform onlineCardTransform = Instantiate(onlineCardPrefab);
        OnlineCard onlineCard = onlineCardTransform.GetComponent<OnlineCard>();

        onlineCards.Add(onlineCard);
        onlineCard.SetServerState(onlineCardState);
        onlineCard.GetComponent<NetworkObject>().Spawn();
        onlineCard.SyncServerState();
        onlineCard.SetTileParent(startTile);

        UpdateAreOnlineCardsPlaced();
        return true;
    }

    private bool AreOnlineCardsPlaced() {
        bool onlineCardsPlaced = true;
        foreach (OnlineCardState onlineCardState in onlineCardCounts.Keys) {
            if (onlineCards.Where(onlinecard => onlinecard.GetServerCardState() == onlineCardState).ToList().Count != onlineCardCounts[onlineCardState]) onlineCardsPlaced = false;
        }
        return onlineCardsPlaced;
    }

    private void UpdateAreOnlineCardsPlaced() {
        areOnlineCardsPlaced.Value = AreOnlineCardsPlaced();
    }

    private void AreOnlineCardsPlaced_OnValueChanged(bool previousValue, bool newValue) {
        OnOnlineCardsPlaced?.Invoke(this, new OnlineCardsPlacedArgs {
            playerEntity = this,
            onlineCardsPlaced = newValue,
        });
    }

    public void InstantiateTiles() {
        foreach (ScoreSlotGroup scoreSlotGroup in scoreSlotsGroupDict.Values) { scoreSlotGroup.InstantiateTileMap(); }
        terminalGroup.InstantiateTileMap();
        infiltrationGroup.InstantiateTileMap();
    }

    public void InstantiateCards() {
        terminalGroup.InstantiateTerminalCards();
    }

    public List<TerminalCard> GetTerminalCards() {
        return terminalGroup.GetTerminalCards();
    }

    public void SubOnlineCards() {
        foreach (OnlineCard onlineCard in onlineCards) {
            onlineCard.OnMoveCard += OnlineCard_OnMoveCard;
            onlineCard.OnCaptureCard += OnlineCard_OnCaptureCard;
        }
    }

    private void OnlineCard_OnMoveCard(object sender, OnlineCard.MoveCardArgs e) {
        e.movingCard.SetTileParent(e.moveTarget);
    }

    private void OnlineCard_OnCaptureCard(object sender, OnlineCard.CaptureCardArgs e) {
        scoreSlotsGroupDict[e.capturedCard.GetServerCardState()].AddOnlineCard(e.capturedCard);
        e.capturedCard.Capture();

        if (e.capturingCard.GetTeam() == GetTeam()) AddScore(e.capturedCard.GetCardState());
    }

    private void AddScore(OnlineCardState cardState) {
        if (cardState == OnlineCardState.Virus) virusScore.Value++;
        if (cardState == OnlineCardState.Link) linkScore.Value++;
    }

    public void SetCardsReady() {
        SetReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void SetReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        areCardsReady.Value = AreOnlineCardsPlaced();
    }

    private void AreCardsReady_OnValueChanged(bool previousValue, bool newValue) {
        OnCardsReady?.Invoke(this, new CardsReadyArgs {
            playerEntity = this,
            cardsReady = newValue,
        });
    }

    public bool AreCardsReady() {
        return areCardsReady.Value;
    }

    public void Clean() {
        areOnlineCardsPlaced.OnValueChanged -= AreOnlineCardsPlaced_OnValueChanged;
        areCardsReady.OnValueChanged -= AreCardsReady_OnValueChanged;

        foreach (OnlineCard onlineCard in onlineCards) {
            onlineCard.OnMoveCard -= OnlineCard_OnMoveCard;
            onlineCard.OnCaptureCard -= OnlineCard_OnCaptureCard;

            onlineCard.CleanOnlineCard();

            onlineCard.GetComponent<NetworkObject>().Despawn();
        }

        Destroy(gameObject);
    }
}
