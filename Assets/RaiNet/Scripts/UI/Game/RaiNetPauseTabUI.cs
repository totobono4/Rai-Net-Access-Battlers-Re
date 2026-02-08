using RaiNet.Game;
using Ttbn4.Game.UI;

namespace RaiNet.UI {
    public class RaiNetPauseTabUI : PauseTabUI {
        public override void Show() {
            InputSystem.Instance.SetInactive();
            base.Show();
        }

        public override void Hide() {
            InputSystem.Instance.SetActive();
            base.Hide();
        }
    }
}