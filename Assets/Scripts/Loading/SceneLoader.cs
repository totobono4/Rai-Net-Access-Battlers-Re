using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene {
        MenuScene,
        LoadingScene,
        LobbyScene,
        WaitingScene,
        GameScene
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene) {
        SceneLoader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadNetwork(Scene targetScene) {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback() {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
