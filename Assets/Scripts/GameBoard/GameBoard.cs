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
    Dictionary<TeamColor, Dictionary<OnlineCard.CardType, Transform>> onlineCardPrefabs;
    Dictionary<TeamColor, Dictionary<OnlineCard.CardType, int>> onlineCardCounts;
    Dictionary<TeamColor, List<Vector2Int>> onlineCardPlacements;

    private List<TileMap> tileMaps = new List<TileMap>();

    private void Awake() {
        onlineCardPrefabs = gameBoardSO.GetPrefabs();
        onlineCardCounts = gameBoardSO.GetCounts();
        onlineCardPlacements = gameBoardSO.GetPlacements();

        tileMaps.Add(playGrid);
    }

    private void Start() {
        foreach (PlayerEntity playerEntity in players) {
            TeamColor playerTeamColor = playerEntity.GetTeamColor();

            List<Transform> cardTransforms = new List<Transform>();
            foreach (OnlineCard.CardType cardType in onlineCardCounts[playerTeamColor].Keys) { 
                for (int i = 0; i < onlineCardCounts[playerTeamColor][cardType];  i++) { cardTransforms.Add(Instantiate(onlineCardPrefabs[playerTeamColor][cardType])); }
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

            for (int i = 0;i < onlineCards.Count; i++) { onlineCards[i].SetTileParent(playGrid.GetTile(onlineCardPlacements[playerTeamColor][i])); }

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
