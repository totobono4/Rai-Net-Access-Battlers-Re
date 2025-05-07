using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Scriptable/Other/GBOnlineCardsObject")]
public class PlayerOnlineCardsSO : ScriptableObject
{
    [SerializeField] private Transform prefab;
    [SerializeField] private List<OnlineCardState> onlineCardTypes;
    [SerializeField] private List<int> counts;
    [SerializeField] private List<Vector2Int> placements;

    public Transform GetPrefab() {
        return prefab;
    }

    public Dictionary<OnlineCardState, int> GetCounts() {
        Dictionary<OnlineCardState, int> countsDict = new Dictionary<OnlineCardState, int>();
        for (int i = 0; i < onlineCardTypes.Count; i++) countsDict.Add(onlineCardTypes[i], counts[i]);
        return countsDict;
    }

    public List<Vector2Int> GetPlacements() { return placements; }
}
