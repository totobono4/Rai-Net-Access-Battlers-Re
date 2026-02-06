using UnityEngine;

public abstract class TileMapSO : ScriptableObject
{
    abstract public int GetWidth();
    abstract public int GetHeight();
    abstract public Transform[,] GetTilePrefabArray();
}
