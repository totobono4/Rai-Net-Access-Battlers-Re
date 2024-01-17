using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig")]
public class GameConfigSO : ScriptableObject
{
    [SerializeField] private int actionTokens;
    [SerializeField] private List<GameBoard.Team> playerTeams;
    [SerializeField] private List<GameBoard.Team> playOrder;

    public int GetActionTokens() { return actionTokens; }
    public List<GameBoard.Team> GetPlayerTeams() { return new List<GameBoard.Team>(playerTeams); }
    public List<GameBoard.Team> GetPlayOrder() { return playOrder; }
}
