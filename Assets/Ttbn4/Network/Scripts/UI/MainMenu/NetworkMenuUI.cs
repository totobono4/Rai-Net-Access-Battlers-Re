public class NetworkMenuUI : MenuUI {
    protected override void Awake() {
        base.Awake();

        playButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.LobbyScene);
        });
    }
}
