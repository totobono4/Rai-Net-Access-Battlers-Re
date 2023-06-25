using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NeighborMatrixObject")]
public class NeighborMatrixSO : ScriptableObject {
    public List<Vector2Int> neighborMatrix;
}
