using UnityEngine;

public class GridMap<TGridObject> {
    private int height;
    private int width;
    Transform origin;
    protected TGridObject[,] gridMap;

    public GridMap(int width, int height, Transform origin) {
        this.width = width;
        this.height = height;
        this.origin = origin;

        gridMap = new TGridObject[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.blue, Mathf.Infinity);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.blue, Mathf.Infinity);
            }
        }
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.blue, Mathf.Infinity);
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.blue, Mathf.Infinity);
    }

    private bool IsValidPosition(int x, int y) {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return Vector3.Scale(new Vector3(x, 0, y), origin.localScale) + origin.position;
    }

    public void GetCoords(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - origin.position).x / origin.localScale.x);
        y = Mathf.FloorToInt((worldPosition - origin.position).z / origin.localScale.z);
    }

    public void SetValue(int x, int y, TGridObject value) {
        if (IsValidPosition(x,y)) {
            gridMap[x, y] = value;
        }
    }

    public void SetValue(Vector3 worldPosition, TGridObject value) {
        int x,y;
        GetCoords(worldPosition, out x, out y);
        gridMap[x, y] = value;
    }

    public bool GetValue(int x, int y, out TGridObject tGridObject) {
        if (IsValidPosition(x, y)) {
            tGridObject = gridMap[x, y];
            return true;
        }
        tGridObject = default(TGridObject);
        return false;
    }
}
