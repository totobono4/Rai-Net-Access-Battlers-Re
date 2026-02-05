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
        pauseTab.gameObject.GetComponent<PauseTabUI>().Show();
    }
    private void Hide() {
        pauseTab.gameObject.GetComponent<PauseTabUI>().Hide();
    }

    public void Clean() {
        pauseTab.GetComponent<PauseTabUI>().Clean();
        Destroy(gameObject);
    }
}
