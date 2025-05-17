using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PauseTabUI : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button forfeitButton;

    private void Awake() {
        continueButton.onClick.AddListener(() => {
            Hide();
            InputSystem.Instance.SetActive();
        });
        forfeitButton.onClick.AddListener(() => {
            GameCleaner.Instance.Clean();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void CleanPauseTabUI() {
        Destroy(gameObject);
    }
}
