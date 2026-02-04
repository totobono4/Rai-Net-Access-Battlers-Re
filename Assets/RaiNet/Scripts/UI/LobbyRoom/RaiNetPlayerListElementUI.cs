using UnityEngine;
using UnityEngine.UI;

public class RaiNetPlayerListElementUI : PlayerListElementUI<RaiNetPlayerData> {
    [SerializeField] private TeamColorsSO teamColors;
    [SerializeField] private Image teamColorImage;

    protected override void UpdatePlayerOverride(PlayerData<RaiNetPlayerData> playerData) {
        teamColorImage.color = teamColors.GetTeamColors()[playerData.customData.team];
    }
}
