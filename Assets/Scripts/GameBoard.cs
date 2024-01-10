using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameBoard : MonoBehaviour {
    public static GameBoard Instance { get; private set; }

    [SerializeField] private PlayMap playMap;
    [SerializeField] private List<PlayerEntity> players;
    [SerializeField] private Dictionary<ulong, Team> teamsByIds;
    [SerializeField] private TeamNetworkSO teamNetworkSO;

    public enum Team {
        None,
        Yellow,
        Blue
    }

    private List<TileMap> tileMaps = new List<TileMap>();

    private void Awake() {
        Instance = this;

        teamsByIds = new Dictionary<ulong, Team>();
    }

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
                int rand = UnityEngine.Random.Range(0, onlineCards.Count);
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

    public Team PickTeam(ulong clientId) {
        if (teamsByIds.Count < teamNetworkSO.playerTeams.Count) teamsByIds.Add(clientId, teamNetworkSO.playerTeams[teamsByIds.Count]);

        if (!teamsByIds.ContainsKey(clientId)) return Team.None;

        return teamsByIds[clientId];
    }

    public bool GetTile(Vector3 worldPosition, out Tile tile) {
        foreach (TileMap tileMap in tileMaps) {
            if (tileMap.GetTile(worldPosition, out tile)) return true;
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
