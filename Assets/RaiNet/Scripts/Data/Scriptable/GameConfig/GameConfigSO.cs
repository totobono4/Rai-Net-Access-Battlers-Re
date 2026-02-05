using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigSO", menuName = "RaiNet/GameConfig")]
public class GameConfigSO : ScriptableObject
{
    [SerializeField] private Transform playerPrefab;

    [SerializeField] private int actionTokens;
    [SerializeField] private List<PlayerTeam> playerTeams;
    [SerializeField] private List<PlayerTeam> playOrder;

    public Transform GetPlayerPrefab() { return playerPrefab; }
    public int GetActionTokens() { return actionTokens; }
    public List<PlayerTeam> GetPlayerTeams() {  return playerTeams; }
    public List<PlayerTeam> GetPlayOrder() { return playOrder; }
}
