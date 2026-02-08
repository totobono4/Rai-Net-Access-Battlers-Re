using System;
using TMPro;
using Ttbn4.Network.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Ttbn4.Network.UI {
    public abstract class PlayerListElementUI<TCustomData> : MonoBehaviour where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Toggle readyToggle;
        [SerializeField] private Button kickButton;

        private ulong clientId;
        private int playerIndex;

        private string playerId;

        public void Initialize(int playerIndex) {
            MultiplayerManager<TCustomData>.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
            LobbyRoomManager<TCustomData>.Instance.OnClientReadyStateChanged += LobbyRoomReadyManager_OnClientReadyStateChanged;

            this.playerIndex = playerIndex;

            kickButton.onClick.AddListener(() => {
                MultiplayerManager<TCustomData>.Instance.KickPlayerByClientId(clientId);
                LobbyManager<TCustomData>.Instance.KickPlayer(playerId);
            });

            UpdatePlayer();
            UpdatePlayerReady();

            if (!NetworkManager.Singleton.IsServer) HideKickButton();
        }

        private void MultiplayerManager_OnPlayerDataNetworkListChanged(object sender, EventArgs e) {
            UpdatePlayer();
            UpdatePlayerReady();
        }

        private void LobbyRoomReadyManager_OnClientReadyStateChanged(object sender, EventArgs e) {
            UpdatePlayerReady();
        }

        private void OnDestroy() {
            MultiplayerManager<TCustomData>.Instance.OnPlayerDataNetworkListChanged -= MultiplayerManager_OnPlayerDataNetworkListChanged;
            LobbyRoomManager<TCustomData>.Instance.OnClientReadyStateChanged -= LobbyRoomReadyManager_OnClientReadyStateChanged;
        }

        private void UpdatePlayer() {
            if (!MultiplayerManager<TCustomData>.Instance.IsPlayerIndexConnected(playerIndex)) {
                Hide();
                return;
            }

            PlayerData<TCustomData> playerData = MultiplayerManager<TCustomData>.Instance.GetPlayerDataByIndex(playerIndex);

            clientId = playerData.basePlayerData.clientId;
            playerId = playerData.basePlayerData.playerId.ToString();
            playerNameText.text = playerData.basePlayerData.playerName.ToString();
            UpdatePlayerOverride(playerData);

            Show();

            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.LocalClientId == clientId) HideKickButton();
        }

        protected abstract void UpdatePlayerOverride(PlayerData<TCustomData> playerData);

        private void UpdatePlayerReady() {
            readyToggle.isOn = LobbyRoomManager<TCustomData>.Instance.GetPlayerReady(clientId);
        }

        private void HideKickButton() {
            kickButton.gameObject.SetActive(false);
        }

        private void Show() {
            this.gameObject.SetActive(true);
        }

        private void Hide() {
            this.gameObject.SetActive(false);
        }

        public void Clean() {
            MultiplayerManager<TCustomData>.Instance.OnPlayerDataNetworkListChanged -= MultiplayerManager_OnPlayerDataNetworkListChanged;
            LobbyRoomManager<TCustomData>.Instance.OnClientReadyStateChanged -= LobbyRoomReadyManager_OnClientReadyStateChanged;

            Destroy(gameObject);
        }
    }
}