using System.Collections.Generic;
using UnityEngine;

public class PlayGrid : TileMap {

    public List<Tile> GetNeighbors(Vector3 worldPosition) {
        int x, y;
        tileMap.GetCoords(worldPosition, out x, out y);

        List<Tile> result = new List<Tile>();

        if (tileMap.GetValue(x + 1, y, out Transform upNeighbor)) {
            result.Add(upNeighbor.GetComponent<Tile>());
        }
        if (tileMap.GetValue(x - 1, y, out Transform downNeighbor)) {
            result.Add(downNeighbor.GetComponent<Tile>());
        }
        if (tileMap.GetValue(x, y + 1, out Transform leftNeighbor)) {
            result.Add(leftNeighbor.GetComponent<Tile>());
        }
        if (tileMap.GetValue(x, y - 1, out Transform rightNeihbor)) {
            result.Add(rightNeihbor.GetComponent<Tile>());
        }

        return result;
    }
}
