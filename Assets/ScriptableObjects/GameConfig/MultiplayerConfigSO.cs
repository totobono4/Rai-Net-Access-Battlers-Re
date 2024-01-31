using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MultiplayerConfig")]
public class MultiplayerConfigSO : ScriptableObject
{
    [SerializeField] private int MaxPlayerCount;
    [SerializeField] private int MinPlayerCount;

    [SerializeField] private List<Team> playerTeams;

    public int GetMaxPlayerCount() { return MaxPlayerCount; }
    public int GetMinPlayerCount() { return MinPlayerCount; }
    public List<Team> GetPlayerTeams() { return new List<Team>(playerTeams); }
}
