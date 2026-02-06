using System;
using Unity.Netcode;
using UnityEngine;

public abstract class LobbyRoomCleaner<TCustomData> : MonoBehaviour where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable { 
    public static LobbyRoomCleaner<TCustomData> Instance { get; private set; }

    private DisconnectedUI<TCustomData> disconnectedUI;
    private LobbyRoomUI<TCustomData> lobbyRoomUI;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("LobbyRoomCleaner has multiple instances");
            return;
        }
        Instance = this;

        disconnectedUI = GetDisconnectedUI();
        lobbyRoomUI = GetLobbyRoomUI();
    }

    protected abstract DisconnectedUI<TCustomData> GetDisconnectedUI();
    protected abstract LobbyRoomUI<TCustomData> GetLobbyRoomUI();

    private void Start() {
        LobbyRoomManager<TCustomData>.Instance.OnStartGame += LobbyRoomReadyManager_OnStartGame;
        disconnectedUI.OnClean += DisconnectedUI_OnClean;
    }

    private void LobbyRoomReadyManager_OnStartGame(object sender, EventArgs e) {
        Clean();
    }

    private void DisconnectedUI_OnClean(object sender, EventArgs e) {
        Clean();
    }

    public void Clean() {
        LobbyRoomManager<TCustomData>.Instance.OnStartGame -= LobbyRoomReadyManager_OnStartGame;
        LobbyManager<TCustomData>.Instance.Clean();

        disconnectedUI.OnClean -= DisconnectedUI_OnClean;
        disconnectedUI.Clean();

        lobbyRoomUI.Clean();
        

        Destroy(gameObject);
    }
}
