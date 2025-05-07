using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfosElement : NetworkBehaviour
{
    [SerializeField] private TeamColorsSO teamColors;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerBackgroundImage;

    private PlayerTeam team;

    public void Initialize(int playerIndex) {
        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataByIndex(playerIndex);
        playerNameText.text = playerData.playerName.ToString();
        team = playerData.team;

        NotPlaying();

        PlayerController.OnPlayerStateChanged += PlayerController_OnPlayerStateChanged;
    }

    private void PlayerController_OnPlayerStateChanged(object sender, EventArgs e) {
        NotPlaying();
        PlayerController playerController = sender as PlayerController;
        if (!playerController.GetTeam().Equals(team)) return;
        if (playerController.IsWaitingForTurn()) return;
        Playing();
    }

    public override void OnDestroy() {
        base.OnDestroy();
        PlayerController.OnPlayerStateChanged -= PlayerController_OnPlayerStateChanged;
    }

    private void Playing() {
        playerBackgroundImage.color = teamColors.GetTeamColors()[team] * new Color(1, 1, 1, 0.5f);
    }

    private void NotPlaying() {
        playerBackgroundImage.color = teamColors.GetTeamColors()[team] * new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }
}
