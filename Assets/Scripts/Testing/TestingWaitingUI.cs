using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingWaitingUI : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button readyButton;

    private void Awake() {
        menuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
        readyButton.onClick.AddListener(() => {
            LobbyRoomReadyManager.Instance.TogglePlayerReady();
        });
    }
}