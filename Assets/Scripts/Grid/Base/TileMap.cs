using UnityEngine;

public class TileMap : MonoBehaviour {
    [SerializeField] private TileMapSO tileMapSO;
    [SerializeField] private Transform origin;

    private Transform[,] tilePrefabArray;
    private int width;
    private int height;
    protected GridMap<Transform> tileMap;

    protected virtual void Awake() {
        tilePrefabArray = tileMapSO.GetTilePrefabArray();
        width = tileMapSO.GetWidth();
        height = tileMapSO.GetHeight();

        tileMap = new GridMap<Transform>(width, height, origin);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Transform tile = Instantiate(tilePrefabArray[y, x], tileMap.GetWorldPosition(x, y), Quaternion.identity, transform);
                tile.transform.localScale = origin.localScale;
                tileMap.SetValue(x, y, tile);
            }
        }
    }

    protected int GetWidth() { return width; }
    protected int GetHeight() { return height; }

    public Tile GetTile(int x, int y) {
        if (tileMap.GetValue(x, y, out Transform tileTransform)) {
            return tileTransform.GetComponent<Tile>();
        }
        return null;
    }

    public Tile GetTile(Vector2Int coords) {
        return GetTile(coords.x, coords.y);
    }

    public bool GetTile(Vector3 worldPosition, out Tile tile) {
        tileMap.GetCoords(worldPosition, out int x, out int y);
        tile = GetTile(x, y);
        return tile != null;
    }
}
