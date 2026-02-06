using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomUI<TCustomData> : MonoBehaviour where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
    [SerializeField] private Button mainMenuButton;

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    [SerializeField] private Button readyButton;

    [SerializeField] private Transform playerSlots;
    [SerializeField] private Transform playerListElementTemplate;

    private List<PlayerListElementUI<TCustomData>> playerListElements;

    private void Awake() {
        playerListElements = new List<PlayerListElementUI<TCustomData>>();

        mainMenuButton.onClick.AddListener(() => {
            LobbyManager<TCustomData>.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            NetworkSceneLoader.Load(NetworkSceneLoader.Scene.MainMenuScene);
        });
        readyButton.onClick.AddListener(() => {
            LobbyRoomManager<TCustomData>.Instance.TogglePlayerReady();
        });
    }

    private void Start() {
        lobbyNameText.text = LobbyManager<TCustomData>.Instance.GetLobbyName();
        lobbyCodeText.text = LobbyManager<TCustomData>.Instance.GetLobbyCode();

        for (int i = 0; i < MultiplayerManager<TCustomData>.Instance.GetMaxPlayerCount(); i++) {
            Transform playerListElementTransform = Instantiate(playerListElementTemplate, playerSlots);
            PlayerListElementUI<TCustomData> playerListElement = playerListElementTransform.GetComponent<PlayerListElementUI<TCustomData>>();
            playerListElements.Add(playerListElement);
            playerListElement.Initialize(i);
        }   
    }

    public void Clean() {
        foreach (PlayerListElementUI<TCustomData> playerListElement in playerListElements) {
            playerListElement.Clean();
        }

        Destroy(gameObject);
    }
}
