using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene {
        MainMenuScene,
        LoadingScene,
        CreditScene,
        LobbyScene,
        LobbyRoomScene,
        GameScene
    }

    private static string targetScene;

    public static void Load(Scene targetScene) {
        Load(targetScene.ToString(), Scene.LoadingScene.ToString());
    }

    public static void Load(string targetScene) {
        Load(targetScene, Scene.LoadingScene.ToString());
    }

    public static void Load(Scene targetScene, string loadingScene) {
        Load(targetScene.ToString(), loadingScene);
    }

    public static void Load(string targetScene, Scene loadingScene) {
        Load(targetScene, loadingScene.ToString());
    }

    public static void Load(Scene targetScene, Scene loadingScene) {
        Load(targetScene.ToString(), loadingScene.ToString());
    }

    public static void Load(string targetScene, string loadingScene) {
        SceneLoader.targetScene = targetScene;
        SceneManager.LoadScene(loadingScene);
    }

    public static void LoadNetwork(Scene targetScene) {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoadNetwork(string targetScene) {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }

    public static void LoaderCallback() {
        SceneManager.LoadScene(targetScene);
    }
}
