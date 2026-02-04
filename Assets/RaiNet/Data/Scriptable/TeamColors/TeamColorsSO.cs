using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamColorsSO", menuName = "RaiNet/TeamColors")]
public class TeamColorsSO : ScriptableObject
{
    [SerializeField] private List<PlayerTeam> teams;
    [SerializeField] private List<Color> colors;

    public Dictionary<PlayerTeam, Color> GetTeamColors() {
        Dictionary<PlayerTeam, Color> teamColors = new Dictionary<PlayerTeam, Color>();
        for (int i = 0; i < teams.Count; i++) teamColors.Add(teams[i], colors[i]);
        return teamColors;
    }
}
