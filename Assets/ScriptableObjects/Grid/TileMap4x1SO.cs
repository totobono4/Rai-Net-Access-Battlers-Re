using UnityEngine;

[CreateAssetMenu (menuName = "TileMap4x1Object")]
public class TileMap4x1SO : TileMapSO {
    public Transform tile_0;
    public Transform tile_1;
    public Transform tile_2;
    public Transform tile_3;

    private int GRID_WIDTH = 4;
    private int GRID_HEIGHT = 1;

    public override int GetWidth() { return GRID_WIDTH; }
    public override int GetHeight() { return GRID_HEIGHT; }

    public override Transform[,] GetTilePrefabArray() { return new Transform[,] { { tile_0, tile_1, tile_2, tile_3 } }; }
}
