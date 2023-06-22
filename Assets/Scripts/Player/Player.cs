using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Team {
        Yellow,
        Blue
    };

    [SerializeField] private PlayerSO playerSO;
    private Team team;
    private Dictionary<OnlineCard.CardType, Transform> onlineCardPrefabs = new Dictionary<OnlineCard.CardType, Transform>();
    private List<Vector2Int> onlineCardPlacements;

    [SerializeField] private List<OnlineCard.CardType> onlineCardTypes;
    [SerializeField] private List<ScoreSlotGroup> scoreSlotsGroups;
    private Dictionary<OnlineCard.CardType, ScoreSlotGroup> scoreSlotsGroupDict = new Dictionary<OnlineCard.CardType, ScoreSlotGroup>();
    private int linkScore;
    private int virusScore;

    private void Awake() {
        team = playerSO.team;
        for (int i = 0; i < playerSO.onlineCardTypes.Count; i++) { onlineCardPrefabs.Add(playerSO.onlineCardTypes[i], playerSO.onlineCardPrefabs[i]); }
        onlineCardPlacements = playerSO.onlineCardsPlacements;

        for (int i = 0; i < onlineCardTypes.Count; i++) scoreSlotsGroupDict.Add(onlineCardTypes[i], scoreSlotsGroups[i]);

        linkScore = 0;
        virusScore = 0;
    }

    public Dictionary<OnlineCard.CardType, Transform> GetCardPrefabs() { return onlineCardPrefabs; }
    public List<Vector2Int> GetCardPlacements() { return onlineCardPlacements; }
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
        if (e.capturedCard.GetCardType() == OnlineCard.CardType.Virus) { virusScore++; }
        if (e.capturedCard.GetCardType() == OnlineCard.CardType.Link) { linkScore++; }
    }
}
