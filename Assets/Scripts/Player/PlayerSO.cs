using System;
using System.Collections.Generic;
using UnityEngine;
using static Player;

[CreateAssetMenu (menuName = "PlayerObject")]
public class PlayerSO : ScriptableObject
{
    public Team team;
    public List<OnlineCard.CardType> onlineCardTypes;
    public List<Transform> onlineCardPrefabs;
    public List<Vector2Int> onlineCardsPlacements;
}
