using UnityEngine;

namespace RaiNet.Data {
    [CreateAssetMenu(fileName = "TileMap1x1SO", menuName = "RaiNet/Grid/TileMap1x1Object")]
    public class TileMap1x1SO : TileMapSO {
        [SerializeField] Transform tile;

        private int GRID_WIDTH = 1;
        private int GRID_HEIGHT = 1;

        public override int GetWidth() {
            return GRID_WIDTH;
        }

        public override int GetHeight() {
            return GRID_HEIGHT;
        }

        public override Transform[,] GetTilePrefabArray() {
            return new Transform[,] {
            { tile }
        };
        }
    }
}