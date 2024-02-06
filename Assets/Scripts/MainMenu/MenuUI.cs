using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button rulesButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        playButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.LobbyScene);
        });
        rulesButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.RulesScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }
}
