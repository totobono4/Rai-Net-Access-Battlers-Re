using RaiNet.Data;
using RaiNet.Game;
using RaiNet.Network.Data;
using System;
using TMPro;
using Ttbn4.Network;
using Ttbn4.Network.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace RaiNet.UI {
    public class PlayerInfosElementUI : NetworkBehaviour {
        [SerializeField] private TeamColorsSO teamColors;

        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image playerBackgroundImage;

        private PlayerTeam team;

        public void Initialize(int playerIndex) {
            PlayerData<RaiNetPlayerData> playerData = MultiplayerManager<RaiNetPlayerData>.Instance.GetPlayerDataByIndex(playerIndex);
            playerNameText.text = playerData.basePlayerData.playerName.ToString();
            team = playerData.customData.team;

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

        private void Playing() {
            playerBackgroundImage.color = teamColors.GetTeamColors()[team] * new Color(1, 1, 1, 0.5f);
        }

        private void NotPlaying() {
            playerBackgroundImage.color = teamColors.GetTeamColors()[team] * new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        public void Clean() {
            PlayerController.OnPlayerStateChanged -= PlayerController_OnPlayerStateChanged;

            Destroy(gameObject);
        }
    }
}