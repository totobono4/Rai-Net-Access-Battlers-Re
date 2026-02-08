using RaiNet.Data;
using RaiNet.Network.Data;
using Totobono4.Network.Data;
using Totobono4.Network.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RaiNet.UI {
    public class RaiNetPlayerListElementUI : PlayerListElementUI<RaiNetPlayerData> {
        [SerializeField] private TeamColorsSO teamColors;
        [SerializeField] private Image teamColorImage;

        protected override void UpdatePlayerOverride(PlayerData<RaiNetPlayerData> playerData) {
            teamColorImage.color = teamColors.GetTeamColors()[playerData.customData.team];
        }
    }
}