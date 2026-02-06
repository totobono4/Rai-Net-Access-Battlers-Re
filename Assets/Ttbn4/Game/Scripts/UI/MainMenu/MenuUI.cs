using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] protected Button playButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button quitButton;

    protected virtual void Awake() {
        creditButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.CreditScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }
}
