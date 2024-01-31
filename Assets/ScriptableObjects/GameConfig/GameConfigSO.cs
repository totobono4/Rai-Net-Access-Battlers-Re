using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig")]
public class GameConfigSO : ScriptableObject
{
    [SerializeField] private int MAX_PLAYER_COUNT;
    [SerializeField] private int MIN_PLAYER_COUNT;

    [SerializeField] private Transform playerPrefab;

    [SerializeField] private int actionTokens;
    [SerializeField] private List<Team> playOrder;

    public Transform GetPlayerPrefab() { return playerPrefab; }
    public int GetActionTokens() { return actionTokens; }
    public List<Team> GetPlayOrder() { return playOrder; }
}
