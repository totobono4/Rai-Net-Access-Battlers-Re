using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiplayerConfigSO", menuName = "ttbn4/Data/Network/MultiplayerConfig")]
public class MultiplayerConfigSO : ScriptableObject
{
    [SerializeField] private int MaxPlayerCount;
    [SerializeField] private int MinPlayerCount;

    [SerializeField] private List<PlayerTeam> playerTeams;

    public int GetMaxPlayerCount() { return MaxPlayerCount; }
    public int GetMinPlayerCount() { return MinPlayerCount; }
    public List<PlayerTeam> GetPlayerTeams() { return new List<PlayerTeam>(playerTeams); }
}
