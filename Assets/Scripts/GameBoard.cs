using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoard : MonoBehaviour {
    [SerializeField] private TileMap playGrid;
    [SerializeField] private TileMap scorePlayerLinks;
    [SerializeField] private TileMap scorePlayerVirus;
    [SerializeField] private TileMap scoreEnnemyLinks;
    [SerializeField] private TileMap scoreEnnemyVirus;

    [SerializeField] private Player player;
    [SerializeField] private Player ennemy;

    private void Start() {
        List<Card> playerCards = player.GetCards();
        foreach (Card card in playerCards) { card.SetGameBoard(this); }

        playerCards[0].SetTileParent(playGrid.GetTile(0, 0));
        playerCards[1].SetTileParent(playGrid.GetTile(1, 0));
        playerCards[2].SetTileParent(playGrid.GetTile(2, 0));
        playerCards[3].SetTileParent(playGrid.GetTile(3, 1));
        playerCards[4].SetTileParent(playGrid.GetTile(4, 1));
        playerCards[5].SetTileParent(playGrid.GetTile(5, 0));
        playerCards[6].SetTileParent(playGrid.GetTile(6, 0));
        playerCards[7].SetTileParent(playGrid.GetTile(7, 0));

        List<Card> ennemyCards = ennemy.GetCards();
        foreach (Card card in ennemyCards) { card.SetGameBoard(this); }

        ennemyCards[0].SetTileParent(playGrid.GetTile(0, 7));
        ennemyCards[1].SetTileParent(playGrid.GetTile(1, 7));
        ennemyCards[2].SetTileParent(playGrid.GetTile(2, 7));
        ennemyCards[3].SetTileParent(playGrid.GetTile(3, 6));
        ennemyCards[4].SetTileParent(playGrid.GetTile(4, 6));
        ennemyCards[5].SetTileParent(playGrid.GetTile(5, 7));
        ennemyCards[6].SetTileParent(playGrid.GetTile(6, 7));
        ennemyCards[7].SetTileParent(playGrid.GetTile(7, 7));
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
