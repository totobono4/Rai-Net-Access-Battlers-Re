using UnityEngine;

public class RaiNetLobbyRoomCleaner : LobbyRoomCleaner<RaiNetPlayerData> {
    [SerializeField] private RaiNetDisconnectUI raiNetDisconnectedUI;
    [SerializeField] private LobbyRoomUI<RaiNetPlayerData> raiNetLobbyRoomUI;

    protected override DisconnectedUI<RaiNetPlayerData> GetDisconnectedUI() {
        return raiNetDisconnectedUI;
    }

    protected override LobbyRoomUI<RaiNetPlayerData> GetLobbyRoomUI() {
        return raiNetLobbyRoomUI;
    }
}
