using UnityEngine;

[CreateAssetMenu(fileName = "TileMap8x8SO", menuName = "RaiNet/Grid/TileMap8x8Object")]
public class TileMap8x8SO : TileMapSO {
    public Transform tile_00;
    public Transform tile_01;
    public Transform tile_02;
    public Transform tile_03;
    public Transform tile_04;
    public Transform tile_05;
    public Transform tile_06;
    public Transform tile_07;

    public Transform tile_10;
    public Transform tile_11;
    public Transform tile_12;
    public Transform tile_13;
    public Transform tile_14;
    public Transform tile_15;
    public Transform tile_16;
    public Transform tile_17;

    public Transform tile_20;
    public Transform tile_21;
    public Transform tile_22;
    public Transform tile_23;
    public Transform tile_24;
    public Transform tile_25;
    public Transform tile_26;
    public Transform tile_27;

    public Transform tile_30;
    public Transform tile_31;
    public Transform tile_32;
    public Transform tile_33;
    public Transform tile_34;
    public Transform tile_35;
    public Transform tile_36;
    public Transform tile_37;

    public Transform tile_40;
    public Transform tile_41;
    public Transform tile_42;
    public Transform tile_43;
    public Transform tile_44;
    public Transform tile_45;
    public Transform tile_46;
    public Transform tile_47;

    public Transform tile_50;
    public Transform tile_51;
    public Transform tile_52;
    public Transform tile_53;
    public Transform tile_54;
    public Transform tile_55;
    public Transform tile_56;
    public Transform tile_57;

    public Transform tile_60;
    public Transform tile_61;
    public Transform tile_62;
    public Transform tile_63;
    public Transform tile_64;
    public Transform tile_65;
    public Transform tile_66;
    public Transform tile_67;

    public Transform tile_70;
    public Transform tile_71;
    public Transform tile_72;
    public Transform tile_73;
    public Transform tile_74;
    public Transform tile_75;
    public Transform tile_76;
    public Transform tile_77;

    private int GRID_WIDTH = 8;
    private int GRID_HEIGHT = 8;

    public override int GetWidth() { return GRID_WIDTH; }

    public override int GetHeight() { return GRID_HEIGHT; }

    public override Transform[,] GetTilePrefabArray() {
        return new Transform[,] {
            { tile_00, tile_01, tile_02, tile_03, tile_04, tile_05, tile_06, tile_07,},
            { tile_10, tile_11, tile_12, tile_13, tile_14, tile_15, tile_16, tile_17,},
            { tile_20, tile_21, tile_22, tile_23, tile_24, tile_25, tile_26, tile_27,},
            { tile_30, tile_31, tile_32, tile_33, tile_34, tile_35, tile_36, tile_37,},
            { tile_40, tile_41, tile_42, tile_43, tile_44, tile_45, tile_46, tile_47,},
            { tile_50, tile_51, tile_52, tile_53, tile_54, tile_55, tile_56, tile_57,},
            { tile_60, tile_61, tile_62, tile_63, tile_64, tile_65, tile_66, tile_67,},
            { tile_70, tile_71, tile_72, tile_73, tile_74, tile_75, tile_76, tile_77,},
        };
    }
}
