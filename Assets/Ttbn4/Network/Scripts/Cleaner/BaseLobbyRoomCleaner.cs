using UnityEngine;

public class BaseLobbyRoomCleaner : LobbyRoomCleaner<EmptyPlayerData> {
    [SerializeField] private BaseDisconnectUI baseDisconnectedUI;
    [SerializeField] private BaseLobbyRoomUI baseLobbyRoomUI;

    protected override DisconnectedUI<EmptyPlayerData> GetDisconnectedUI() {
        return baseDisconnectedUI;
    }

    protected override LobbyRoomUI<EmptyPlayerData> GetLobbyRoomUI() {
        return baseLobbyRoomUI;
    }
}
