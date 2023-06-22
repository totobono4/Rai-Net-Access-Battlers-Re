using System;
using System.Collections.Generic;
using UnityEngine;
using static Player;

[CreateAssetMenu (menuName = "Team")]
public class PlayerSO : ScriptableObject
{
    public Team team;
    public CardPrefabs cardPrefabs;
    public List<Vector2Int> cardsPlacements;
}
