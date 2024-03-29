using System.Collections.Generic;
using UnityEngine;

public class PlayMap : TileMap {
    public List<Tile> GetNeighbors(Vector3 worldPosition, NeighborMatrixSO neighborMatrixSO) {
        int x, y;
        tileMap.GetCoords(worldPosition, out x, out y);
        Vector2Int coords = new Vector2Int(x, y);

        List<Tile> result = new List<Tile>();

        List<Vector2Int> neighborMatrix = neighborMatrixSO.GetNeighborMatrix();

        foreach (Vector2Int neighborRelativeCoords in neighborMatrix) {
            Vector2Int neighborCoords = coords + neighborRelativeCoords;
            if (TryGetTile(neighborCoords, out Tile tile)) {
                result.Add(tile);
            }
        }

        return result;
    }
}
