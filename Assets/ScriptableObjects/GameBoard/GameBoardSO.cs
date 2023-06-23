using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "GameBoardObject")]
public class GameBoardSO : ScriptableObject
{
    public List<GameBoard.TeamColor> teamColors;
    public List<GBOnlineCardsSO> gBOnlineCardsSOs;
}
