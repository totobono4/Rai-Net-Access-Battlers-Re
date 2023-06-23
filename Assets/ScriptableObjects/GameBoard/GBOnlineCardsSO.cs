using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "GBOnlineCardsObject")]
public class GBOnlineCardsSO : ScriptableObject
{
    public List<OnlineCard.CardType> onlineCardTypes;
    public List<Transform> prefabs;
    public List<int> counts;
    public List<Vector2Int> placements;
}
