using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button closeButton;

    private void Awake() {
        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start() {
        MultiplayerManager.Instance.OnTryToConnect += MultiplayerManager_OnTryToConect;
        MultiplayerManager.Instance.OnConnectionFailed += MultiplayerManager_OnConnectionFailed;

        LobbyManager.Instance.OnTryCreateLobby += LobbyManager_OnTryCreateLobby;
        LobbyManager.Instance.OnCreateLobbyFailed += LobbyManager_OnCreateLobbyFailed;

        LobbyManager.Instance.OnTryQuickJoinLobby += LobbyManager_OnTryQuickJoinLobby;
        LobbyManager.Instance.OnQuickJoinLobbyFailed += LobbyManager_OnQuickJoinnLobbyFailed;

        LobbyManager.Instance.OnTryJoinLobbyById += LobbyManager_OnTryJoinLobbyById;
        LobbyManager.Instance.OnJoinLobbyByIdFailed += LobbyManager_OnJoinLobbyByIdFailed;

        LobbyManager.Instance.OnTryJoinLobbyByCode += LobbyManager_OnTryJoinLobbyByCode;
        LobbyManager.Instance.OnJoinLobbyByCodeFailed += LobbyManager_OnJoinLobbyByCodeFailed;

        LobbyManager.Instance.OnRefusedToJoinLobby += LobbyManager_OnRefusedToJoinLobby;

        Hide();
    }

    private void LobbyManager_OnTryCreateLobby(object sender, EventArgs e) {
        statusText.text = "Creating Lobby...";
        messageText.text = "";
        HideCloseButton();
        Show();
    }

    private void LobbyManager_OnCreateLobbyFailed(object sender, LobbyManager.LobbyServiceExceptionArgs e) {
        statusText.text = "Create Lobby Failed";
        messageText.text = e.lobbyServiceException.Message;
        ShowCloseButton();
        Show();
    }

    private void LobbyManager_OnTryQuickJoinLobby(object sender, EventArgs e) {
        statusText.text = "Quick Joining Lobby...";
        messageText.text = "";
        HideCloseButton();
        Show();
    }

    private void LobbyManager_OnQuickJoinnLobbyFailed(object sender, LobbyManager.LobbyServiceExceptionArgs e) {
        statusText.text = "Quick Join Lobby Failed";
        messageText.text = e.lobbyServiceException.Message;
        ShowCloseButton();
        Show();
    }

    private void MultiplayerManager_OnTryToConect(object sender, EventArgs e) {
        statusText.text = "Connecting...";
        messageText.text = "";
        HideCloseButton();
        Show();
    }

    private void MultiplayerManager_OnConnectionFailed(object sender, EventArgs e) {
        statusText.text = "Failed to Connect";
        messageText.text = NetworkManager.Singleton.DisconnectReason;
        ShowCloseButton();
        Show();
    }

    private void LobbyManager_OnTryJoinLobbyById(object sender, EventArgs e) {
        statusText.text = "Joining Lobby by Id...";
        messageText.text = "";
        HideCloseButton();
        Show();
    }

    private void LobbyManager_OnJoinLobbyByIdFailed(object sender, LobbyManager.LobbyServiceExceptionArgs e) {
        statusText.text = "Joining Lobby by Id Failed";
        messageText.text = e.lobbyServiceException.Message;
        ShowCloseButton();
        Show();
    }

    private void LobbyManager_OnTryJoinLobbyByCode(object sender, EventArgs e) {
        statusText.text = "Joining Lobby by Code...";
        messageText.text = "";
        HideCloseButton();
        Show();
    }

    private void LobbyManager_OnJoinLobbyByCodeFailed(object sender, LobbyManager.LobbyServiceExceptionArgs e) {
        statusText.text = "Joining Lobby by Code Failed";
        messageText.text = e.lobbyServiceException.Message;
        ShowCloseButton();
        Show();
    }

    private void LobbyManager_OnRefusedToJoinLobby(object sender, LobbyManager.RefusedToJoinLobbyArgs e) {
        statusText.text = "Refused to join Lobby";
        messageText.text = e.message;
        ShowCloseButton();
        Show();
    }

    private void ShowCloseButton() {
        closeButton.gameObject.SetActive(true);
    }

    private void HideCloseButton() {
        closeButton.gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerManager.Instance.OnTryToConnect -= MultiplayerManager_OnTryToConect;
        MultiplayerManager.Instance.OnConnectionFailed -= MultiplayerManager_OnConnectionFailed;

        LobbyManager.Instance.OnTryCreateLobby -= LobbyManager_OnTryCreateLobby;
        LobbyManager.Instance.OnCreateLobbyFailed -= LobbyManager_OnCreateLobbyFailed;

        LobbyManager.Instance.OnTryQuickJoinLobby -= LobbyManager_OnTryQuickJoinLobby;
        LobbyManager.Instance.OnQuickJoinLobbyFailed -= LobbyManager_OnQuickJoinnLobbyFailed;

        LobbyManager.Instance.OnTryJoinLobbyById -= LobbyManager_OnTryJoinLobbyById;
        LobbyManager.Instance.OnJoinLobbyByIdFailed -= LobbyManager_OnJoinLobbyByIdFailed;

        LobbyManager.Instance.OnTryJoinLobbyByCode -= LobbyManager_OnTryJoinLobbyByCode;
        LobbyManager.Instance.OnJoinLobbyByCodeFailed -= LobbyManager_OnJoinLobbyByCodeFailed;
    }
}
