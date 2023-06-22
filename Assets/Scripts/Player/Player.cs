using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Team {
        Yellow,
        Blue
    };
    [Serializable]
    public struct CardPrefabs {
        public Transform link;
        public Transform virus;
    }

    [SerializeField] private PlayerSO teamSO;

    private Team team;
    private CardPrefabs cardPrefabs;
    private List<Vector2Int> cardPlacements;

    private int linkScore;
    private int virusScore;

    private void Awake() {
        team = teamSO.team;
        cardPrefabs = teamSO.cardPrefabs;
        cardPlacements = teamSO.cardsPlacements;

        linkScore = 0;
        virusScore = 0;
    }

    public CardPrefabs GetCardPrefabs() { return cardPrefabs; }
    public List<Vector2Int> GetCardPlacements() { return cardPlacements; }
    public void SubOnlineCards(List<OnlineCard> onlineCards) {
        foreach (OnlineCard onlineCard in onlineCards) { onlineCard.OnCaptureCard += AddScore; }
    }

    private void AddScore(object sender, OnlineCard.CaptureCardArgs e) {
        if (e.capturedCard.GetCardType() == OnlineCard.Type.Virus) { virusScore++; }
        if (e.capturedCard.GetCardType() == OnlineCard.Type.Link) { linkScore++; }
    }
}
