using RaiNet.Network.Data;
using RaiNet.UI;
using Totobono4.Network.Clean;
using Totobono4.Network.UI;
using UnityEngine;

namespace RaiNet.Network.Clean {
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
}