using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TeamColorObject")]
public class TeamColorsSO : ScriptableObject
{
    [SerializeField] private List<Team> teams;
    [SerializeField] private List<Color> colors;

    public Dictionary<Team, Color> GetTeamColors() {
        Dictionary<Team, Color> teamColors = new Dictionary<Team, Color>();
        for (int i = 0; i < teams.Count; i++) teamColors.Add(teams[i], colors[i]);
        return teamColors;
    }
}
