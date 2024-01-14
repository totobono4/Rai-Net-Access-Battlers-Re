using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static PlayerController;

public class GameBoard : NetworkBehaviour {
    public static GameBoard Instance { get; private set; }

    [SerializeField] private PlayMap playMap;
    [SerializeField] private List<PlayerEntity> players;

    [SerializeField] private TeamNetworkSO teamNetworkSO;
    private List<GameBoard.Team> playerTeams;
    private Dictionary<ulong, Team> teamsByIds;
    private Dictionary<Team, ulong> idsByTeams;

    public enum Team {
        None,
        Yellow,
        Blue
    }

    private List<TileMap> tileMaps = new List<TileMap>();

    private void Awake() {
        Instance = this;

        playerTeams = teamNetworkSO.GetPlayerTeams();
        teamsByIds = new Dictionary<ulong, Team>();
        idsByTeams = new Dictionary<Team, ulong>();
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
            Dictionary<OnlineCard.CardState, int> onlineCardCounts = playerEntity.GetOnlineCardCounts();
            List<Vector2Int> onlineCardPlacements = playerEntity.GetOnlineCardPlacements();

            List<Transform> cardTransforms = new List<Transform>();
            foreach (OnlineCard.CardState cardType in onlineCardCounts.Keys) {
                for (int i = 0; i < onlineCardCounts[cardType]; i++) {
                    Transform cardTransform = Instantiate(onlineCardPrefab);
                    cardTransforms.Add(cardTransform);

                    cardTransform.GetComponent<OnlineCard>().SetServerState(cardType);

                    NetworkObject cardNetwork = cardTransform.GetComponent<NetworkObject>();
                    cardNetwork.Spawn();
                }
            }

            List<OnlineCard> onlineCards = new List<OnlineCard>();
            foreach (Transform cardTransform in cardTransforms) onlineCards.Add(cardTransform.GetComponent<OnlineCard>());

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

    private bool TryGetTeam(out GameBoard.Team team) {
        team = Team.None;
        if (playerTeams.Count == 0) return false;
        team = playerTeams[0];
        playerTeams.RemoveAt(0);
        return true;
    }

    public Team PickTeam(ulong clientId) {
        if (teamsByIds.TryGetValue(clientId, out Team teamExists)) return teamExists;
        if (!TryGetTeam(out Team team)) { Debug.Log(team); return team; }
        teamsByIds.Add(clientId, team);
        idsByTeams.Add(team, clientId);
        return team;
    }

    public bool TryGetClientIdByTeam(Team team, out ulong clientId) {
        if (!idsByTeams.TryGetValue(team, out clientId)) return false;
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
