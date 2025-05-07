using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] Button pauseButton;
    [SerializeField] Transform pauseTab;

    private void Awake() {
        pauseButton.onClick.AddListener(() => {
            Show();
        });
    }

    private void Start() {
        Hide();
    }

    private void Show() {
        InputSystem.Instance.SetInactive();
        pauseTab.gameObject.SetActive(true);
    }
    private void Hide() {
        pauseTab.gameObject.SetActive(false);
    }
}
