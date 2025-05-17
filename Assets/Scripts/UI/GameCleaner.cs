using UnityEngine;

public class GameCleaner : MonoBehaviour
{
    public static GameCleaner Instance { get; private set; }

    [SerializeField] CardsReadyUI cardsReadyUI;
    [SerializeField] GameOverUI gameOverUI;
    [SerializeField] PauseUI pauseUI;
    [SerializeField] PlayerInfosUI playerInfosUI;
    [SerializeField] DisconnectedUI DisconnectedUI;

    private void Awake() {
        Instance = this;
    }

    public void Clean() {
        GameManager.Instance.Clean();
        cardsReadyUI.Clean();
        gameOverUI.Clean();
        pauseUI.Clean();
        playerInfosUI.Clean();
        DisconnectedUI.Clean();
    }
}
