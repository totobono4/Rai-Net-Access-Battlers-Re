using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerEntity : NetworkBehaviour {
    [SerializeField] private GameBoard.Team team;

    [SerializeField] private PlayerOnlineCardsSO playerOnlineCardsSO;
    Dictionary<OnlineCard.CardType, Transform> onlineCardPrefabs;
    Dictionary<OnlineCard.CardType, int> onlineCardCounts;
    List<Vector2Int> onlineCardPlacements;

    [SerializeField] private List<OnlineCard.CardType> onlineCardTypes;
    [SerializeField] private List<ScoreSlotGroup> scoreSlotsGroups;
    private Dictionary<OnlineCard.CardType, ScoreSlotGroup> scoreSlotsGroupDict = new Dictionary<OnlineCard.CardType, ScoreSlotGroup>();

    [SerializeField] private NetworkVariable<int> linkScore;
    [SerializeField] private NetworkVariable<int> virusScore;

    [SerializeField] private TerminalGroup terminalGroup;
    [SerializeField] private InfiltrationGroup infiltrationGroup;

    private void Awake() {
        onlineCardPrefabs = playerOnlineCardsSO.GetPrefabs();
        onlineCardCounts = playerOnlineCardsSO.GetCounts();
        onlineCardPlacements = playerOnlineCardsSO.GetPlacements();

        for (int i = 0; i < onlineCardTypes.Count; i++) scoreSlotsGroupDict.Add(onlineCardTypes[i], scoreSlotsGroups[i]);

        linkScore.Value = 0;
        virusScore.Value = 0;
    }

    public GameBoard.Team GetTeam() { return team; }
    public List<TileMap> GetTileMaps() {
        List<TileMap> tileMaps = new List<TileMap>();
        foreach (TileMap tileMap in scoreSlotsGroupDict.Values) { tileMaps.Add(tileMap); }
        tileMaps.Add(terminalGroup);
        tileMaps.Add(infiltrationGroup);
        return tileMaps;
    }
    public Dictionary<OnlineCard.CardType, Transform> GetOnlineCardPrefabs() { return onlineCardPrefabs; }
    public Dictionary<OnlineCard.CardType, int> GetOnlineCardCounts() { return onlineCardCounts; }
    public List<Vector2Int> GetOnlineCardPlacements() { return onlineCardPlacements; }

    public void InstantiateTiles() {
        foreach (ScoreSlotGroup scoreSlotGroup in scoreSlotsGroupDict.Values) { scoreSlotGroup.InstantiateTileMap(); }
        terminalGroup.InstantiateTileMap();
        infiltrationGroup.InstantiateTileMap();
    }

    public void InstantiateCards() {
        terminalGroup.InstantiateTerminalCards();
    }

    public List<TerminalCard> GetTerminalCards() { return terminalGroup.GetTerminalCards(); }

    public void SubOnlineCards(List<OnlineCard> onlineCards) {
        foreach (OnlineCard onlineCard in onlineCards) {
            onlineCard.OnMoveCard += MoveCard;
            onlineCard.OnCaptureCard += CaptureCard;
            onlineCard.OnCaptureCard += AddScore;
        }
    }

    private void MoveCard(object sender, OnlineCard.MoveCardArgs e) {
        e.movingCard.SetTileParent(e.moveTarget);
    }

    private void CaptureCard(object sender, OnlineCard.CaptureCardArgs e) {
        scoreSlotsGroupDict[e.capturedCard.GetCardType()].AddOnlineCard(e.capturedCard);
        e.capturedCard.Capture();
    }

    private void AddScore(object sender, OnlineCard.CaptureCardArgs e) {
        if (e.capturedCard.GetCardType() == OnlineCard.CardType.Virus) { virusScore.Value++; }
        if (e.capturedCard.GetCardType() == OnlineCard.CardType.Link) { linkScore.Value++; }
    }
}
