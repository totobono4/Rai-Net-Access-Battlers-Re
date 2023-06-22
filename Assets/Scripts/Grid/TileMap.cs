using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour {
    [SerializeField] private Transform tilePrefab;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Transform origin;
    [SerializeField] private GridMap<Transform> tileMap;

    private void Awake() {
        tileMap = new GridMap<Transform>(width, height, origin);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Transform tile = Instantiate(tilePrefab, tileMap.GetWorldPosition(x, y), Quaternion.identity, transform);
                tile.transform.localScale = origin.localScale;
                tileMap.SetValue(x, y, tile);
            }
        }
    }

    public Tile GetTile(Vector2Int coords) {
        return GetTile(coords.x, coords.y);
    }

    public Tile GetTile(int x, int y) {
        if (tileMap.GetValue(x, y, out Transform tileTransform)) {
            return tileTransform.GetComponent<Tile>();
        }
        return null;
    }

    public bool GetTile(Vector3 worldPosition, out Tile tile) {
        tileMap.GetCoords(worldPosition, out int x, out int y);
        if (tileMap.GetValue(x, y, out Transform tileTransform)) {
            tile = tileTransform.GetComponent<Tile>();
            return true;
        }
        tile = null;
        return false;
    }

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
