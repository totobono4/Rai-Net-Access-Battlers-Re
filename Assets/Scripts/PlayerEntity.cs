using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    [SerializeField] private PlayerSO playerSO;
    private GameBoard.Team teamColor;

    [SerializeField] private List<OnlineCard.CardType> onlineCardTypes;
    [SerializeField] private List<ScoreSlotGroup> scoreSlotsGroups;
    private Dictionary<OnlineCard.CardType, ScoreSlotGroup> scoreSlotsGroupDict = new Dictionary<OnlineCard.CardType, ScoreSlotGroup>();

    private int linkScore;
    private int virusScore;

    [SerializeField] private TerminalGroup terminalGroup;

    private void Awake() {
        teamColor = playerSO.teamColor;

        for (int i = 0; i < onlineCardTypes.Count; i++) scoreSlotsGroupDict.Add(onlineCardTypes[i], scoreSlotsGroups[i]);

        linkScore = 0;
        virusScore = 0;
    }

    public GameBoard.Team GetTeamColor() { return teamColor; }
    public List<TileMap> GetTileMaps() {
        List<TileMap> tileMaps = new List<TileMap>();
        foreach (TileMap tileMap in scoreSlotsGroupDict.Values) { tileMaps.Add(tileMap); }
        tileMaps.Add(terminalGroup);
        return tileMaps;
    }

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
