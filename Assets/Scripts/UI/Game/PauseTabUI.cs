using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PauseTabUI : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button forfaitButton;

    private void Awake() {
        continueButton.onClick.AddListener(() => {
            Hide();
            InputSystem.Instance.SetActive();
        });
        forfaitButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
