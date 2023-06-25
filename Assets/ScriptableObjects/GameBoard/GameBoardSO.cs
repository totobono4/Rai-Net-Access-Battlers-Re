using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "GameBoardObject")]
public class GameBoardSO : ScriptableObject
{
    [SerializeField] private List<GameBoard.Team> teamColors;
    [SerializeField] private List<GBOnlineCardsSO> gBOnlineCardsSOs;

    public Dictionary<GameBoard.Team, Dictionary<OnlineCard.CardType, Transform>> GetPrefabs() {
        Dictionary<GameBoard.Team, Dictionary<OnlineCard.CardType, Transform>> prefabsDict = new Dictionary<GameBoard.Team, Dictionary<OnlineCard.CardType, Transform>> ();
        for (int i = 0; i < teamColors.Count; i++) { prefabsDict.Add(teamColors[i], gBOnlineCardsSOs[i].GetPrefabs()); }
        return prefabsDict;
    }

    public Dictionary<GameBoard.Team, Dictionary<OnlineCard.CardType, int>> GetCounts() {
        Dictionary<GameBoard.Team, Dictionary<OnlineCard.CardType, int>> countsDict = new Dictionary<GameBoard.Team, Dictionary<OnlineCard.CardType, int>> ();
        for (int i = 0;i < teamColors.Count;i++) { countsDict.Add(teamColors[i], gBOnlineCardsSOs[i].GetCounts()); }
        return countsDict;
    }

    public Dictionary<GameBoard.Team, List<Vector2Int>> GetPlacements() {
        Dictionary<GameBoard.Team, List<Vector2Int>> placementsDict = new Dictionary<GameBoard.Team, List<Vector2Int>> ();
        for (int i = 0; i < teamColors.Count; i++) { placementsDict.Add(teamColors[i], gBOnlineCardsSOs[i].GetPlacements()); }
        return placementsDict;
    }
}
