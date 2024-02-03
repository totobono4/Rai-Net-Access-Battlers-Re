using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "GBOnlineCardsObject")]
public class PlayerOnlineCardsSO : ScriptableObject
{
    [SerializeField] private Transform prefab;
    [SerializeField] private List<OnlineCard.CardState> onlineCardTypes;
    [SerializeField] private List<int> counts;
    [SerializeField] private List<Vector2Int> placements;

    public Transform GetPrefab() {
        return prefab;
    }

    public Dictionary<OnlineCard.CardState, int> GetCounts() {
        Dictionary<OnlineCard.CardState, int> countsDict = new Dictionary<OnlineCard.CardState, int>();
        for (int i = 0; i < onlineCardTypes.Count; i++) countsDict.Add(onlineCardTypes[i], counts[i]);
        return countsDict;
    }

    public List<Vector2Int> GetPlacements() { return placements; }
}
