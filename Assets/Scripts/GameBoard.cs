using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {
    [SerializeField] private PlayGrid playGrid;
    [SerializeField] private List<Player> players;

    private void Start() {
        foreach (var player in players) {
            Dictionary<OnlineCard.CardType, int> onlineCardCounts = player.GetOnlineCardCounts();
            Dictionary<OnlineCard.CardType, Transform> onlineCardPrefabs = player.GetOnlineCardPrefabs();
            Transform link = onlineCardPrefabs[OnlineCard.CardType.Link];
            Transform virus = onlineCardPrefabs[OnlineCard.CardType.Virus];

            List<Transform> playerCardsTransforms = new List<Transform>();
            foreach (OnlineCard.CardType cardType in onlineCardCounts.Keys) { 
                for (int i = 0; i < onlineCardCounts[cardType];  i++) { playerCardsTransforms.Add(Instantiate(onlineCardPrefabs[cardType])); }
            }

            List<OnlineCard> playerOnlineCards = new List<OnlineCard>();
            foreach (Transform playerCardTransform in playerCardsTransforms) playerOnlineCards.Add(playerCardTransform.GetComponent<OnlineCard>());
            foreach (OnlineCard playerOnlineCard  in playerOnlineCards) { playerOnlineCard.SetGameBoard(this); }

            for (int i = 0; i < playerOnlineCards.Count; i++) {
                int rand = Random.Range(0, 8);
                OnlineCard temp = playerOnlineCards[i];
                playerOnlineCards[i] = playerOnlineCards[rand];
                playerOnlineCards[rand] = temp;
            }

            List<Vector2Int> cardPlacements = player.GetCardPlacements();

            for (int i = 0;i < playerOnlineCards.Count; i++) { playerOnlineCards[i].SetTileParent(playGrid.GetTile(cardPlacements[i])); }

            player.SubOnlineCards(playerOnlineCards);
        }
    }

    public bool GetPlayGridTile(Vector3 worldPosition, out Tile tile) {
        if (playGrid.GetTile(worldPosition, out tile)) {
            return true;
        }
        tile = default(Tile);
        return false;
    }

    public List<Tile> GetNeighbors(Vector3 worldPosition) {
        return playGrid.GetNeighbors(worldPosition);
    }
}
