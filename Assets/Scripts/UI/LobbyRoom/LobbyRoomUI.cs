using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    [SerializeField] private Button readyButton;

    [SerializeField] private Transform playerSlots;
    [SerializeField] private Transform playerListElementTemplate;

    private List<PlayerListElement> playerListElements;

    private void Awake() {
        playerListElements = new List<PlayerListElement>();

        mainMenuButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
        readyButton.onClick.AddListener(() => {
            LobbyRoomReadyManager.Instance.TogglePlayerReady();
        });
    }

    private void Start() {
        lobbyNameText.text = LobbyManager.Instance.GetLobbyName();
        lobbyCodeText.text = LobbyManager.Instance.GetLobbyCode();

        for (int i = 0; i < MultiplayerManager.Instance.GetMaxPlayerCount(); i++) {
            Transform playerListElementTransform = Instantiate(playerListElementTemplate, playerSlots);
            PlayerListElement playerListElement = playerListElementTransform.GetComponent<PlayerListElement>();
            playerListElements.Add(playerListElement);
            playerListElement.Initialize(i);
        }   
    }

    public void Clean() {
        foreach (PlayerListElement playerListElement in playerListElements) {
            playerListElement.Clean();
        }

        Destroy(gameObject);
    }
}
