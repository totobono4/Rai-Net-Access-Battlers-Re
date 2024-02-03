using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameConfigObject")]
public class GameConfigSO : ScriptableObject
{
    [SerializeField] private Transform playerPrefab;

    [SerializeField] private int actionTokens;
    [SerializeField] private List<Team> playerTeams;
    [SerializeField] private List<Team> playOrder;

    public Transform GetPlayerPrefab() { return playerPrefab; }
    public int GetActionTokens() { return actionTokens; }
    public List<Team> GetPlayerTeams() {  return playerTeams; }
    public List<Team> GetPlayOrder() { return playOrder; }
}
