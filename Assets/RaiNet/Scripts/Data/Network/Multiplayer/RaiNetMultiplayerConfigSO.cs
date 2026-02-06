using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaiNetMultiplayerConfigSO", menuName = "RaiNet/Network/RaiNetMultiplayerConfig")]
public class RaiNetMultiplayerConfigSO : MultiplayerConfigSO
{
    [SerializeField] private List<PlayerTeam> playerTeams;

    public List<PlayerTeam> GetPlayerTeams() { return new List<PlayerTeam>(playerTeams); }
}
