using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static GameBoard;
using static PlayerController;

public class GameBoard : NetworkBehaviour {
    public static GameBoard Instance { get; private set; }

    [SerializeField] private PlayMap playMap;
    [SerializeField] private List<PlayerEntity> players;

    [SerializeField] private TeamNetworkSO teamNetworkSO;
    private List<Team> playerTeams;

    private PlayerInfos playerInfos;
    

    public enum Team {
        None,
        Yellow,
        Blue
    }

    private List<TileMap> tileMaps = new List<TileMap>();

    private void Awake() {
        Instance = this;

        playerTeams = teamNetworkSO.GetPlayerTeams();

        playerInfos = new PlayerInfos();
    }

    private void Start() {
        tileMaps.Add(playMap);
        foreach (PlayerEntity playerEntity in players) foreach (TileMap tileMap in playerEntity.GetTileMaps()) { tileMaps.Add(tileMap); }
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        playMap.InstantiateTileMap();

        foreach (PlayerEntity playerEntity in players) {
            Transform onlineCardPrefab = playerEntity.GetOnlineCardPrefab();

            playerInfos.TryCreatePlayer(playerEntity.GetTeam());
            playerInfos.TrySetPlayerEntityFromTeam(playerEntity.GetTeam(), playerEntity);
            playerInfos.TrySetOnlineCardPrefabFromTeam(playerEntity.GetTeam(), onlineCardPrefab);

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
                int rand = UnityEngine.Random.Range(0, onlineCards.Count);
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

    public OnlineCard CopyOnlineCard(OnlineCard onlineCard, Tile tileParent) {
        if (!playerInfos.TryGetPlayerEntityFromTeam(onlineCard.GetTeam(), out PlayerEntity playerEntity)) return null;
        if (!playerInfos.TryGetOnlineCardPrefabFromTeam(onlineCard.GetTeam(), out Transform onlineCardPrefab)) return null;

        Transform newOnlineCardTransform = Instantiate(onlineCardPrefab);
        OnlineCard newOnlineCard = newOnlineCardTransform.GetComponent<OnlineCard>();

        newOnlineCard.SetServerState(onlineCard.GetServerCardState());
        newOnlineCard.SetTileParent(tileParent);
        newOnlineCard.GetComponent<NetworkObject>().Spawn();
        playerEntity.SubOnlineCard(newOnlineCard);

        onlineCard.GetComponent<NetworkObject>().Despawn();

        return newOnlineCardTransform.GetComponent<OnlineCard>();
    }

    private bool TryGetTeam(out GameBoard.Team team) {
        team = Team.None;
        if (playerTeams.Count == 0) return false;
        team = playerTeams[0];
        playerTeams.RemoveAt(0);
        return true;
    }

    public Team PickTeam(ulong clientId) {
        if (playerInfos.TryGetTeamFromId(clientId, out Team teamExists)) return teamExists;
        if (!TryGetTeam(out Team team)) { Debug.Log(team); return team; }

        playerInfos.TryCreatePlayer(team);
        playerInfos.TrySetIdFromTeam(team, clientId);

        return team;
    }

    public bool TryGetClientIdByTeam(Team team, out ulong clientId) {
        if (!playerInfos.TryGetIdFromTeam(team, out clientId)) return false;
        return true;
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
}
