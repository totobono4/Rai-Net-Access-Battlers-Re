using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button quitButton;

    protected virtual void Awake() {
        playButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.LobbyScene);
        });
        creditButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.CreditScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }
}
