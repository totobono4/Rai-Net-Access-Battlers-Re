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

        PlayerEntity.OnCardsReady += PlayerEntity_OnCardsReady;
    }

    public void Initialize() {
        if (!IsServer) return;

        playMap.InstantiateTileMap();

        foreach (PlayerEntity playerEntity in players) {
            playerEntity.InstantiateTiles();
            playerEntity.InstantiateCards();
        }
    }

    private void PlayerEntity_OnCardsReady(object sender, PlayerEntity.CardsReadyArgs e) {
        if (!IsServer) return;

        (sender as PlayerEntity).SubOnlineCards();

        bool allPlayersReady = true;
        foreach (PlayerEntity playerEntity in players) {
            if (!playerEntity.AreCardsReady()) {
                allPlayersReady = false;
                break;
            }
        }

        if (allPlayersReady) GameManager.Instance.PassPriority(0);
    }

    public bool GetTile(Vector3 worldPosition, out Tile tile) {
        foreach (TileMap tileMap in tileMaps) {
            if (tileMap.TryGetTile(worldPosition, out tile)) return true;
        }
        tile = null;
        return false;
    }

    public List<StartTile> GetStartTilesByTeam(PlayerTeam team) {
        return GetStartTiles().Where(tile => tile.GetTeam() == team).ToList();
    }

    public List<StartTile> GetStartTiles() {
        return GetAllTiles().OfType<StartTile>().ToList();
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

    public PlayerEntity GetPlayerEntityByTeam(PlayerTeam team) {
        return players.FirstOrDefault(player => player.GetTeam() == team);
    }

    public bool TryPlaceOnlineCard(StartTile startTile, OnlineCardState onlineCardState, PlayerTeam team) {
        return GetPlayerEntityByTeam(team).TryPlaceOnlineCard(startTile, onlineCardState);
    }

    public void Clean() {
        PlayerEntity.OnCardsReady -= PlayerEntity_OnCardsReady;

        foreach (StartTile startTile in GetStartTiles()) startTile.Clean();
        foreach (TileMap tileMap in tileMaps) tileMap.Clean();
        foreach (PlayerEntity playerEntity in players) playerEntity.Clean();

        Destroy(gameObject);
    }
}
