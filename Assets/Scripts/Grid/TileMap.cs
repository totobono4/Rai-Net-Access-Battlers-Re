using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TileMap : NetworkBehaviour {
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
    }

    public void InstantiateTileMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Transform tileTransform = Instantiate(tilePrefabArray[y, x], tileMap.GetWorldPosition(x, y), Quaternion.identity);
                NetworkObject tileNetwork = tileTransform.GetComponent<NetworkObject>();
                tileNetwork.Spawn();

                tileTransform.transform.localScale = origin.localScale;
                tileMap.SetValue(x, y, tileTransform);
            }
        }
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsClient) return;
        if (IsHost) return;

        SyncGridMap();
    }

    public void SyncGridMap() {
        SyncGridMapServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    protected void SyncGridMapServerRpc() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tileMap.TryGetValue(x, y, out Transform tileTransform);
                if (tileTransform == null) continue;
                SyncGridMapClientRpc(x, y, tileTransform.GetComponent<NetworkObject>());
            }
        }
    }

    [ClientRpc]
    private void SyncGridMapClientRpc(int x, int y, NetworkObjectReference tileNetworkReference) {
        if (!tileNetworkReference.TryGet(out NetworkObject tileNetwork)) return;
        tileMap.SetValue(x, y, tileNetwork.transform);
    }

    protected int GetWidth() { return width; }
    protected int GetHeight() { return height; }

    public Tile GetTile(int x, int y) {
        if (tileMap.TryGetValue(x, y, out Transform tileTransform)) {
            if (tileTransform == null) return null;
            return tileTransform.GetComponent<Tile>();
        }
        return null;
    }

    public Tile GetTile(Vector2Int coords) {
        return GetTile(coords.x, coords.y);
    }

    public bool TryGetTile(Vector3 worldPosition, out Tile tile) {
        tileMap.GetCoords(worldPosition, out int x, out int y);
        tile = GetTile(x, y);
        return tile != null;
    }

    public bool TryGetTile(Vector2Int coords, out Tile tile) {
        tile = GetTile(coords.x, coords.y);
        return tile != null;
    }

    public List<Tile> GetAllTiles() {
        List<Tile> result = new List<Tile>();

        for (int x = 0; x < GetWidth(); x++) {
            for (int y = 0; y < GetHeight(); y++) {
                result.Add(GetTile(x, y));
            }
        }

        return result;
    }
}
