using UnityEngine;

[CreateAssetMenu(fileName = "TileMap1x4SO", menuName = "RaiNet/Grid/TileMap1x4Object")]
public class TileMap1x4SO : TileMapSO {
    public Transform tile_0;
    public Transform tile_1;
    public Transform tile_2;
    public Transform tile_3;

    private int GRID_WIDTH = 1;
    private int GRID_HEIGHT = 4;

    public override int GetWidth() { return GRID_WIDTH; }
    public override int GetHeight() { return GRID_HEIGHT; }

    public override Transform[,] GetTilePrefabArray() {
        return new Transform[,] {
            { tile_0 },
            { tile_1 },
            { tile_2 },
            { tile_3 }
        };
    }
}
