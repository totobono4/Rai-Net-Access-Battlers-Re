using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TeamNetworkObject")]
public class TeamNetworkSO : ScriptableObject
{
    [SerializeField] public List<GameBoard.Team> playerTeams;
}
