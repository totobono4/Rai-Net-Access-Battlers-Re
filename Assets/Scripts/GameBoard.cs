using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {
    [SerializeField] private PlayMap playMap;
    [SerializeField] private List<PlayerEntity> players;

    public enum Team {
        Yellow,
        Blue
    }

    private List<TileMap> tileMaps = new List<TileMap>();

    private void Start() {
        foreach (PlayerEntity playerEntity in players) {
            Dictionary<OnlineCard.CardType, Transform> onlineCardPrefabs = playerEntity.GetOnlineCardPrefabs();
            Dictionary<OnlineCard.CardType, int> onlineCardCounts = playerEntity.GetOnlineCardCounts();
            List<Vector2Int> onlineCardPlacements = playerEntity.GetOnlineCardPlacements();

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

            for (int i = 0;i < onlineCards.Count; i++) { onlineCards[i].SetTileParent(playMap.GetTile(onlineCardPlacements[i])); }

            playerEntity.SubOnlineCards(onlineCards);

            foreach (TerminalCard terminalCard in playerEntity.GetTerminalCards()) { terminalCard.SetGameBoard(this); }

            foreach (TileMap tileMap in playerEntity.GetTileMaps()) { tileMaps.Add(tileMap); }
        }

        tileMaps.Add(playMap);
    }

    public bool GetTile(Vector3 worldPosition, out Tile tile) {
        foreach (TileMap tilemap in tileMaps) {
            if (tilemap.GetTile(worldPosition, out tile)) return true;
        }
        tile = null;
        return false;
    }

    public List<Tile> GetAllTiles() {
        return playMap.GetAllTiles();
    }

    public List<Tile> GetNeighbors(Vector3 worldPosition, NeighborMatrixSO neighborMatrixSO) {
        return playMap.GetNeighbors(worldPosition, neighborMatrixSO);
    }
}
