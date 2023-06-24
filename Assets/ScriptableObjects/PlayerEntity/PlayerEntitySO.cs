using System;
using System.Collections.Generic;
using UnityEngine;
using static PlayerEntity;

[CreateAssetMenu (menuName = "PlayerObject")]
public class PlayerSO : ScriptableObject
{
    public GameBoard.Team teamColor;
}
