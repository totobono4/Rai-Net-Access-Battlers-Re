using RaiNet.Game;
using RaiNet.Network.Data;
using Totobono4.Network.UI;

namespace RaiNet.UI {
    public class RaiNetDisconnectUI : DisconnectedUI<RaiNetPlayerData> {
        protected override void Show() {
            if (InputSystem.Instance != null) InputSystem.Instance.SetInactive();
            base.Show();
        }
    }
}