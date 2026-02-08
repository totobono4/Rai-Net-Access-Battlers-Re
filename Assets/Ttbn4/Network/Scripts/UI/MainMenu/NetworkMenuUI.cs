using Ttbn4.Game.UI;

namespace Ttbn4.Network.UI {
    public class NetworkMenuUI : MenuUI {
        protected override void Awake() {
            base.Awake();

            playButton.onClick.AddListener(() => {
                NetworkSceneLoader.Load(NetworkSceneLoader.Scene.LobbyScene);
            });
        }
    }
}