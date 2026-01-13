using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListElement : MonoBehaviour {
    [SerializeField] private TeamColorsSO teamColors;

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image teamColorImage;
    [SerializeField] private Toggle readyToggle;
    [SerializeField] private Button kickButton;

    private ulong clientId;
    private int playerIndex;

    private string playerId;

    public void Initialize(int playerIndex) {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
        LobbyRoomReadyManager.Instance.OnClientReadyStateChanged += LobbyRoomReadyManager_OnClientReadyStateChanged;

        this.playerIndex = playerIndex;

        kickButton.onClick.AddListener(() => {
            MultiplayerManager.Instance.KickPlayerByClientId(clientId);
            LobbyManager.Instance.KickPlayer(playerId);
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
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= MultiplayerManager_OnPlayerDataNetworkListChanged;
        LobbyRoomReadyManager.Instance.OnClientReadyStateChanged -= LobbyRoomReadyManager_OnClientReadyStateChanged;
    }

    private void UpdatePlayer() {
        if (!MultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex)) {
            Hide();
            return;
        }

        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataByIndex(playerIndex);

        clientId = playerData.clientId;
        playerId = playerData.playerId.ToString();
        playerNameText.text = playerData.playerName.ToString();

        teamColorImage.color = teamColors.GetTeamColors()[MultiplayerManager.Instance.GetPlayerDataByIndex(playerIndex).team];

        Show();

        if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.LocalClientId == clientId) HideKickButton();
    }

    private void UpdatePlayerReady() {
        readyToggle.isOn = LobbyRoomReadyManager.Instance.GetPlayerReady(clientId);
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
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= MultiplayerManager_OnPlayerDataNetworkListChanged;
        LobbyRoomReadyManager.Instance.OnClientReadyStateChanged -= LobbyRoomReadyManager_OnClientReadyStateChanged;

        Destroy(gameObject);
    }
}