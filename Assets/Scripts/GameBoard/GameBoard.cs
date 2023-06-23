using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {
    [SerializeField] private PlayGrid playGrid;
    [SerializeField] private List<PlayerEntity> players;

    public enum TeamColor {
        Yellow,
        Blue
    }

    [SerializeField] private GameBoardSO gameBoardSO;
    private struct GBOnlineCardsStruct {
        public Dictionary<OnlineCard.CardType, Transform> onlineCardPrefabs;
        public Dictionary<OnlineCard.CardType, int> onlineCardCounts;
        public List<Vector2Int> onlineCardPlacements;
    }
    Dictionary<TeamColor, GBOnlineCardsStruct> gBOnlineCards = new Dictionary<TeamColor, GBOnlineCardsStruct>();

    private List<TileMap> tileMaps = new List<TileMap>();

    private void Awake() {
        List<TeamColor> teamColors = gameBoardSO.teamColors;
        List<GBOnlineCardsSO> gBOnlineCardsSOs = gameBoardSO.gBOnlineCardsSOs;

        for (int i = 0; i < teamColors.Count; i++) {
            GBOnlineCardsSO gBOnlineCardsSO = gBOnlineCardsSOs[i];

            List<OnlineCard.CardType> onlineCardTypes = gBOnlineCardsSO.onlineCardTypes;
            List<Transform> prefabs = gBOnlineCardsSO.prefabs;
            List<int> counts = gBOnlineCardsSO.counts;
            List<Vector2Int> placements = gBOnlineCardsSO.placements;

            Dictionary<OnlineCard.CardType, Transform> onlineCardPrefabs = new Dictionary<OnlineCard.CardType, Transform>();
            Dictionary<OnlineCard.CardType, int> onlineCardCounts = new Dictionary<OnlineCard.CardType, int>();

            for (int j = 0; j < onlineCardTypes.Count; j++) {
                onlineCardPrefabs.Add(onlineCardTypes[j], prefabs[j]);
                onlineCardCounts.Add(onlineCardTypes[j], counts[j]);
            }
            List<Vector2Int> onlineCardPlacements = placements;

            GBOnlineCardsStruct gBOnlineCardsStruct = new GBOnlineCardsStruct();
            gBOnlineCardsStruct.onlineCardPrefabs = onlineCardPrefabs;
            gBOnlineCardsStruct.onlineCardCounts = onlineCardCounts;
            gBOnlineCardsStruct.onlineCardPlacements = onlineCardPlacements;

            gBOnlineCards.Add(teamColors[i], gBOnlineCardsStruct);
        }

        tileMaps.Add(playGrid);
    }

    private void Start() {
        foreach (PlayerEntity playerEntity in players) {
            GBOnlineCardsStruct gBOnlineCard = gBOnlineCards[playerEntity.GetTeamColor()];

            Dictionary<OnlineCard.CardType, Transform> onlineCardPrefabs = gBOnlineCard.onlineCardPrefabs;
            Dictionary<OnlineCard.CardType, int> onlineCardCounts = gBOnlineCard.onlineCardCounts;
            List<Vector2Int> onlineCardPlacements = gBOnlineCard.onlineCardPlacements;


            List<Transform> cardTransforms = new List<Transform>();
            foreach (OnlineCard.CardType cardType in onlineCardCounts.Keys) { 
                for (int i = 0; i < onlineCardCounts[cardType];  i++) { cardTransforms.Add(Instantiate(onlineCardPrefabs[cardType])); }
            }

            List<OnlineCard> onlineCards = new List<OnlineCard>();
            foreach (Transform cardTransform in cardTransforms) onlineCards.Add(cardTransform.GetComponent<OnlineCard>());
            foreach (OnlineCard onlineCard  in onlineCards) { onlineCard.SetGameBoard(this); }

            for (int i = 0; i < onlineCards.Count; i++) {
                int rand = Random.Range(0, 8);
                OnlineCard temp = onlineCards[i];
                onlineCards[i] = onlineCards[rand];
                onlineCards[rand] = temp;
            }

            for (int i = 0;i < onlineCards.Count; i++) { onlineCards[i].SetTileParent(playGrid.GetTile(onlineCardPlacements[i])); }

            playerEntity.SubOnlineCards(onlineCards);

            foreach (TileMap tileMap in playerEntity.GetTileMaps()) { tileMaps.Add(tileMap); }
        }
    }

    public bool GetTile(Vector3 worldPosition, out Tile tile) {
        foreach (TileMap tilemap in tileMaps) {
            if (tilemap.GetTile(worldPosition, out tile)) {
                return true;
            }
        }
        tile = null;
        return false;
    }

    public List<Tile> GetNeighbors(Vector3 worldPosition) {
        return playGrid.GetNeighbors(worldPosition);
    }
}
