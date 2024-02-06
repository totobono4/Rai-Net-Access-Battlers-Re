using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winStateText;
    [SerializeField] private Button menuButton;

    private void Awake() {
        menuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start() {
        PlayerController.OnPlayerStateChanged += PlayerController_PlayerStateChanged;

        Hide();
    }

    private void PlayerController_PlayerStateChanged(object sender, EventArgs e) {
        if (sender as PlayerController != PlayerController.LocalInstance) return;

        if (PlayerController.LocalInstance.HasWon()) {
            winStateText.text = "You Won!";
            Show();
        }
        if (PlayerController.LocalInstance.HasLose()) {
            winStateText.text = "You Lose...";
            Show();
        }
    }

    private void OnDestroy() {
        PlayerController.OnPlayerStateChanged -= PlayerController_PlayerStateChanged;
    }

    private void Show() {
        InputSystem.Instance.SetInactive();
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
