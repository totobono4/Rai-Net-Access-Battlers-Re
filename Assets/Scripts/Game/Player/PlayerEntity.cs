using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEntity : NetworkBehaviour {
    [SerializeField] private Team team;

    [SerializeField] private PlayerOnlineCardsSO playerOnlineCardsSO;
    Transform onlineCardPrefab;
    Dictionary<OnlineCard.CardState, int> onlineCardCounts;
    List<Vector2Int> onlineCardPlacements;

    [SerializeField] private List<OnlineCard.CardState> onlineCardTypes;
    [SerializeField] private List<ScoreSlotGroup> scoreSlotsGroups;
    private Dictionary<OnlineCard.CardState, ScoreSlotGroup> scoreSlotsGroupDict = new Dictionary<OnlineCard.CardState, ScoreSlotGroup>();

    private NetworkVariable<int> linkScore;
    private NetworkVariable<int> virusScore;

    [SerializeField] private TerminalGroup terminalGroup;
    [SerializeField] private InfiltrationGroup infiltrationGroup;

    private void Awake() {
        onlineCardPrefab = playerOnlineCardsSO.GetPrefab();
        onlineCardCounts = playerOnlineCardsSO.GetCounts();
        onlineCardPlacements = playerOnlineCardsSO.GetPlacements();

        for (int i = 0; i < onlineCardTypes.Count; i++) scoreSlotsGroupDict.Add(onlineCardTypes[i], scoreSlotsGroups[i]);

        linkScore = new NetworkVariable<int>();
        virusScore = new NetworkVariable<int>();

        linkScore.Value = 0;
        virusScore.Value = 0;
    }

    public Team GetTeam() {
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
    public Transform GetOnlineCardPrefab() {
        return onlineCardPrefab;
    }
    public Dictionary<OnlineCard.CardState, int> GetOnlineCardCounts() {
        return onlineCardCounts;
    }
    public List<Vector2Int> GetOnlineCardPlacements() {
        return onlineCardPlacements;
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

    public void SubOnlineCards(List<OnlineCard> onlineCards) {
        foreach (OnlineCard onlineCard in onlineCards) {
            SubOnlineCard(onlineCard);
        }
    }

    public void SubOnlineCard(OnlineCard onlineCard) {
        onlineCard.OnMoveCard += OnlineCard_OnMoveCard;
        onlineCard.OnCaptureCard += OnlineCard_OnCaptureCard;
    }

    private void OnlineCard_OnMoveCard(object sender, OnlineCard.MoveCardArgs e) {
        e.movingCard.SetTileParent(e.moveTarget);
    }

    private void OnlineCard_OnCaptureCard(object sender, OnlineCard.CaptureCardArgs e) {
        scoreSlotsGroupDict[e.capturedCard.GetServerCardState()].AddOnlineCard(e.capturedCard);
        e.capturedCard.Capture();

        if (e.capturingCard.GetTeam() == GetTeam()) AddScore(e.capturedCard.GetCardState());
    }

    private void AddScore(OnlineCard.CardState cardState) {
        if (cardState == OnlineCard.CardState.Virus) virusScore.Value++;
        if (cardState == OnlineCard.CardState.Link) linkScore.Value++;
    }
}
