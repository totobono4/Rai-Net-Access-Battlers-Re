using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TeamNetworkObject")]
public class TeamNetworkSO : ScriptableObject
{
    [SerializeField] private List<GameBoard.Team> playerTeams;

    public List<GameBoard.Team> GetPlayerTeams() { return new List<GameBoard.Team>(playerTeams); }
}
