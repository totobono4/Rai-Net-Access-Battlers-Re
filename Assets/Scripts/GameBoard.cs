using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameBoard : NetworkBehaviour {
    public static GameBoard Instance { get; private set; }

    [SerializeField] private GameConfigSO gameConfigSO;

    [SerializeField] private PlayMap playMap;
    [SerializeField] private List<PlayerEntity> players;

    private List<Team> playerTeams;
    private List<Team> playOrder;
    private int playCursor;
    private Team teamPriority;

    public enum Team {
        None,
        Yellow,
        Blue
    }

    private PlayersInfos playerInfos;

    private List<TileMap> tileMaps = new List<TileMap>();

    public EventHandler<PlayerGivePriorityArgs> OnPlayerGivePriority;
    public class PlayerGivePriorityArgs : EventArgs {
        public Team team;
        public int actionTokens;
    }

    private void Awake() {
        Instance = this;

        playerTeams = gameConfigSO.GetPlayerTeams();
        playOrder = gameConfigSO.GetPlayOrder();
        playCursor = 0;
        teamPriority = Team.None;

        playerInfos = new PlayersInfos();

        PlayerController.OnPlayerReady += PlayerReady;
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

            TryCreatePlayers();
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

    private void TryCreatePlayers() {
        foreach (PlayerEntity playerEntity in players) {
            playerInfos.TryCreatePlayer(playerEntity.GetTeam());
        }
    }

    private void PlayerReady(object sender, PlayerController.PlayerReadyArgs e) {
        if (playerInfos.AreAllPlayersReady()) return;

        playerInfos.TrySetReadyFromId(e.clientId, true);

        if (!playerInfos.AreAllPlayersReady()) return;

        PassPriority(0);
    }

    public void PassPriority(int actionTokens) {
        if (actionTokens <= 0) {
            teamPriority = playOrder[playCursor % playOrder.Count];
            playCursor++;
        }
        
        OnPlayerGivePriority?.Invoke(this, new PlayerGivePriorityArgs { team = teamPriority, actionTokens = gameConfigSO.GetActionTokens() });
    }

    public void CopyOnlineCard(OnlineCard onlineCard, Tile tileParent) {
        if (!playerInfos.TryGetPlayerEntityFromTeam(onlineCard.GetTeam(), out PlayerEntity playerEntity)) return;
        if (!playerInfos.TryGetOnlineCardPrefabFromTeam(onlineCard.GetTeam(), out Transform onlineCardPrefab)) return;

        Transform newOnlineCardTransform = Instantiate(onlineCardPrefab);
        OnlineCard newOnlineCard = newOnlineCardTransform.GetComponent<OnlineCard>();

        newOnlineCard.SetServerState(onlineCard.GetServerCardState());
        newOnlineCard.SetTileParent(tileParent);
        newOnlineCard.GetComponent<NetworkObject>().Spawn();
        playerEntity.SubOnlineCard(newOnlineCard);

        onlineCard.GetComponent<NetworkObject>().Despawn();
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

        TryCreatePlayers();
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
