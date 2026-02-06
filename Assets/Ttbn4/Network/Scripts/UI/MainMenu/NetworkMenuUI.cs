public class NetworkMenuUI : MenuUI {
    protected override void Awake() {
        base.Awake();

        playButton.onClick.AddListener(() => {
            NetworkSceneLoader.Load(NetworkSceneLoader.Scene.LobbyScene);
        });
    }
}
