using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoard : MonoBehaviour {
    [SerializeField] private TileMap playGrid;
    [SerializeField] private TileMap scoreYellowLinks;
    [SerializeField] private TileMap scoreYellowVirus;
    [SerializeField] private TileMap scoreBlueLinks;
    [SerializeField] private TileMap scoreBlueVirus;

    [SerializeField] private List<Player> players;

    private void Start() {
        foreach (var player in players) {
            Player.CardPrefabs cardPrefabs = player.GetCardPrefabs();
            Transform link = cardPrefabs.link;
            Transform virus = cardPrefabs.virus;

            List<Transform> playerCardsTransforms = new List<Transform> {
                Instantiate(link),
                Instantiate(link),
                Instantiate(link),
                Instantiate(link),
                Instantiate(virus),
                Instantiate(virus),
                Instantiate(virus),
                Instantiate(virus)
            };

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
            foreach (OnlineCard onlineCard in playerOnlineCards) {
                onlineCard.OnMoveCard += MoveCard;
                onlineCard.OnCaptureCard += CaptureCard;
            }
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

    private void MoveCard(object sender, OnlineCard.MoveCardArgs e) {
        e.movingCard.SetTileParent(e.moveTarget);
    }

    // PLEASE REFACTOR THIS WTF BRO
    private void CaptureCard(object sender, OnlineCard.CaptureCardArgs e) {
        if (e.capturedCard.GetTeam() == Player.Team.Blue) {
            if (e.capturedCard.GetCardType() == OnlineCard.Type.Link) {
                if (!scoreYellowLinks.GetTile(0, 0).HasCard()) e.capturedCard.SetTileParent(scoreYellowLinks.GetTile(0, 0));
                else if (!scoreYellowLinks.GetTile(1, 0).HasCard()) e.capturedCard.SetTileParent(scoreYellowLinks.GetTile(1, 0));
                else if (!scoreYellowLinks.GetTile(2, 0).HasCard()) e.capturedCard.SetTileParent(scoreYellowLinks.GetTile(2, 0));
                else if (!scoreYellowLinks.GetTile(3, 0).HasCard()) e.capturedCard.SetTileParent(scoreYellowLinks.GetTile(3, 0));
            }
            if (e.capturedCard.GetCardType() == OnlineCard.Type.Virus) {
                if (!scoreYellowVirus.GetTile(0, 0).HasCard()) e.capturedCard.SetTileParent(scoreYellowVirus.GetTile(0, 0));
                else if (!scoreYellowVirus.GetTile(1, 0).HasCard()) e.capturedCard.SetTileParent(scoreYellowVirus.GetTile(1, 0));
                else if (!scoreYellowVirus.GetTile(2, 0).HasCard()) e.capturedCard.SetTileParent(scoreYellowVirus.GetTile(2, 0));
                else if (!scoreYellowVirus.GetTile(3, 0).HasCard()) e.capturedCard.SetTileParent(scoreYellowVirus.GetTile(3, 0));
            }
        }
        if (e.capturedCard.GetTeam() == Player.Team.Yellow) {
            if (e.capturedCard.GetCardType() == OnlineCard.Type.Link) {
                if (!scoreBlueLinks.GetTile(0, 0).HasCard()) e.capturedCard.SetTileParent(scoreBlueLinks.GetTile(0, 0));
                else if (!scoreBlueLinks.GetTile(1, 0).HasCard()) e.capturedCard.SetTileParent(scoreBlueLinks.GetTile(1, 0));
                else if (!scoreBlueLinks.GetTile(2, 0).HasCard()) e.capturedCard.SetTileParent(scoreBlueLinks.GetTile(2, 0));
                else if (!scoreBlueLinks.GetTile(3, 0).HasCard()) e.capturedCard.SetTileParent(scoreBlueLinks.GetTile(3, 0));
            }
            if (e.capturedCard.GetCardType() == OnlineCard.Type.Virus) {
                if (!scoreBlueVirus.GetTile(0, 0).HasCard()) e.capturedCard.SetTileParent(scoreBlueVirus.GetTile(0, 0));
                else if (!scoreBlueVirus.GetTile(1, 0).HasCard()) e.capturedCard.SetTileParent(scoreBlueVirus.GetTile(1, 0));
                else if (!scoreBlueVirus.GetTile(2, 0).HasCard()) e.capturedCard.SetTileParent(scoreBlueVirus.GetTile(2, 0));
                else if (!scoreBlueVirus.GetTile(3, 0).HasCard()) e.capturedCard.SetTileParent(scoreBlueVirus.GetTile(3, 0));
            }
        }
    }
}
