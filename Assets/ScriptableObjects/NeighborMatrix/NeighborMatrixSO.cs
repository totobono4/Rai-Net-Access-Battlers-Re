using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NeighborMatrixObject")]
public class NeighborMatrixSO : ScriptableObject {
    [SerializeField] private List<Vector2Int> neighborMatrix;

    public List<Vector2Int> GetNeighborMatrix() {
        return neighborMatrix;
    }
}
