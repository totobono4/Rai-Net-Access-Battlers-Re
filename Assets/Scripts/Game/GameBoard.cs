using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameBoard : NetworkBehaviour {
    public static GameBoard Instance { get; private set; }

    [SerializeField] private PlayMap playMap;
    [SerializeField] private List<PlayerEntity> players;

    private List<TileMap> tileMaps = new List<TileMap>();

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        tileMaps.Add(playMap);
        foreach (PlayerEntity playerEntity in players) foreach (TileMap tileMap in playerEntity.GetTileMaps()) { tileMaps.Add(tileMap); }
    }

    public void Initialize() {
        if (!IsServer) return;

        playMap.InstantiateTileMap();

        foreach (PlayerEntity playerEntity in players) {
            Transform onlineCardPrefab = playerEntity.GetOnlineCardPrefab();

            Dictionary<OnlineCard.CardState, int> onlineCardCounts = playerEntity.GetOnlineCardCounts();
            List<Vector2Int> onlineCardPlacements = playerEntity.GetOnlineCardPlacements();

            List<OnlineCard> onlineCards = new List<OnlineCard>();
            foreach (OnlineCard.CardState cardType in onlineCardCounts.Keys) {
                for (int i = 0; i < onlineCardCounts[cardType]; i++) {
                    OnlineCard onlineCard = Instantiate(onlineCardPrefab).GetComponent<OnlineCard>();
                    onlineCards.Add(onlineCard);

                    onlineCard.SetServerState(cardType);

                    onlineCard.GetComponent<NetworkObject>().Spawn();
                }
            }

            for (int i = 0; i < onlineCards.Count; i++) {
                int rand = Random.Range(0, onlineCards.Count);
                OnlineCard temp = onlineCards[i];
                onlineCards[i] = onlineCards[rand];
                onlineCards[rand] = temp;
            }

            for (int i = 0; i < onlineCards.Count; i++) {
                onlineCards[i].SetTileParent(playMap.GetTile(onlineCardPlacements[i]));
                playMap.GetTile(onlineCardPlacements[i]).GetCard(out Card card);
            }

            playerEntity.SubOnlineCards(onlineCards);

            playerEntity.InstantiateTiles();
            playerEntity.InstantiateCards();
        }
    }

    public bool GetTile(Vector3 worldPosition, out Tile tile) {
        foreach (TileMap tileMap in tileMaps) {
            if (tileMap.TryGetTile(worldPosition, out tile)) return true;
        }
        tile = null;
        return false;
    }

    public List<Tile> GetAllTiles() {
        List<Tile> allTiles = new List<Tile>();

        foreach (TileMap tileMap in tileMaps) { allTiles = allTiles.Union(tileMap.GetAllTiles()).ToList(); }

        return allTiles;
    }

    public List<Tile> GetNeighbors(Vector3 worldPosition, NeighborMatrixSO neighborMatrixSO) {
        return playMap.GetNeighbors(worldPosition, neighborMatrixSO);
    }

    public List<PlayerEntity> GetPlayerEntities() {
        return players;
    }
}
