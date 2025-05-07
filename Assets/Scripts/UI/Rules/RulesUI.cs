using UnityEngine;
using UnityEngine.UI;

public class RulesUI : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }
}
