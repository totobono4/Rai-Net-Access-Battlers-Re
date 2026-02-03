using System;
using UnityEngine;

public class LobbyRoomCleaner : MonoBehaviour
{
    public static LobbyRoomCleaner Instance { get; private set; }

    [SerializeField] DisconnectedUI disconnectedUI;
    [SerializeField] LobbyRoomUI lobbyRoomUI;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        LobbyRoomManager.Instance.OnStartGame += LobbyRoomReadyManager_OnStartGame;
        disconnectedUI.OnClean += DisconnectedUI_OnClean;
    }

    private void LobbyRoomReadyManager_OnStartGame(object sender, EventArgs e) {
        Clean();
    }

    private void DisconnectedUI_OnClean(object sender, EventArgs e) {
        Clean();
    }

    public void Clean() {
        LobbyRoomManager.Instance.OnStartGame -= LobbyRoomReadyManager_OnStartGame;
        LobbyManager.Instance.Clean();

        disconnectedUI.OnClean -= DisconnectedUI_OnClean;
        disconnectedUI.Clean();

        lobbyRoomUI.Clean();
        

        Destroy(gameObject);
    }
}
