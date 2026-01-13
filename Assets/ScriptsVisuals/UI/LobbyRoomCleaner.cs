using System;
using UnityEngine;

public class LobbyRoomCleaner : MonoBehaviour
{
    public static LobbyRoomCleaner Instance { get; private set; }

    [SerializeField] LobbyRoomUI lobbyRoomUI;
    [SerializeField] DisconnectedUI disconnectedUI;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        LobbyRoomReadyManager.Instance.OnStartGame += LobbyRoomReadyManager_OnStartGame;
    }

    private void LobbyRoomReadyManager_OnStartGame(object sender, EventArgs e) {
        Clean();
    }

    public void Clean() {
        LobbyRoomReadyManager.Instance.OnStartGame -= LobbyRoomReadyManager_OnStartGame;
        LobbyManager.Instance.Clean();

        lobbyRoomUI.Clean();
        disconnectedUI.Clean();

        Destroy(gameObject);
    }
}
