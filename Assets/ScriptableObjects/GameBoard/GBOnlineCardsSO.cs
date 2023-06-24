using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "GBOnlineCardsObject")]
public class GBOnlineCardsSO : ScriptableObject
{
    [SerializeField] private List<OnlineCard.CardType> onlineCardTypes;
    [SerializeField] private List<Transform> prefabs;
    [SerializeField] private List<int> counts;
    [SerializeField] private List<Vector2Int> placements;

    public Dictionary<OnlineCard.CardType, Transform> GetPrefabs() {
        Dictionary<OnlineCard.CardType, Transform> prefabsDict = new Dictionary<OnlineCard.CardType, Transform>();
        for (int i = 0; i < onlineCardTypes.Count; i++) { prefabsDict.Add(onlineCardTypes[i], prefabs[i]); }
        return prefabsDict;
    }

    public Dictionary<OnlineCard.CardType, int> GetCounts() {
        Dictionary<OnlineCard.CardType, int> countsDict = new Dictionary<OnlineCard.CardType, int>();
        for (int i = 0; i < onlineCardTypes.Count; i++) { countsDict.Add(onlineCardTypes[i], counts[i]); }
        return countsDict;
    }

    public List<Vector2Int> GetPlacements() { return placements; }
}
