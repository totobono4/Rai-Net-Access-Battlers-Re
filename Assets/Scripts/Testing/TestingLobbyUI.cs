using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;

    private void Awake() {
        menuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
        createButton.onClick.AddListener(() => {
            MultiplayerManager.Instance.StartHost();
            SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyRoomScene);
        });
        joinButton.onClick.AddListener(() => {
            MultiplayerManager.Instance.StartClient();
        });
    }
}
