using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisconnectedUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI disconnectStatusText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private bool disconnectHost;

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            if (LobbyRoomCleaner.Instance) LobbyRoomCleaner.Instance.Clean();
            if (GameCleaner.Instance) GameCleaner.Instance.Clean();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start() {
        if (SceneManager.GetActiveScene().name == SceneLoader.Scene.LobbyRoomScene.ToString() && !disconnectHost && NetworkManager.Singleton.IsHost) {
            Hide();
            return;
        }

        if (NetworkManager.Singleton.IsHost) {
            disconnectStatusText.text = "Client have been disconnected";
            MultiplayerManager.Instance.OnClientDisconnect += MultiplayerManager_OnClientDisconnect;
        }
        else {
            disconnectStatusText.text = "You have been disconnected";
            MultiplayerManager.Instance.OnHostDisconnect += MultiplayerManager_OnHostDisconnect;
        }

        Hide();
    }

    private void MultiplayerManager_OnClientDisconnect(object sender, EventArgs e) {
        Show();
    }

    private void MultiplayerManager_OnHostDisconnect(object sender, EventArgs e) {
        Show();
    }

    private void Show() {
        if (InputSystem.Instance != null) InputSystem.Instance.SetInactive();
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void Clean() {
        MultiplayerManager.Instance.OnClientDisconnect -= MultiplayerManager_OnClientDisconnect;
        MultiplayerManager.Instance.OnHostDisconnect -= MultiplayerManager_OnHostDisconnect;

        Destroy(gameObject);
    }
}
