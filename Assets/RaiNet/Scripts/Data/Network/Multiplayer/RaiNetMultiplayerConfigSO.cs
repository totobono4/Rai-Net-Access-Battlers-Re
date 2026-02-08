using RaiNet.Data;
using System.Collections.Generic;
using Totobono4.Network.Data;
using UnityEngine;

namespace RaiNet.Network.Data {
    [CreateAssetMenu(fileName = "RaiNetMultiplayerConfigSO", menuName = "RaiNet/Network/RaiNetMultiplayerConfig")]
    public class RaiNetMultiplayerConfigSO : MultiplayerConfigSO {
        [SerializeField] private List<PlayerTeam> playerTeams;

        public List<PlayerTeam> GetPlayerTeams() { return new List<PlayerTeam>(playerTeams); }
    }
}