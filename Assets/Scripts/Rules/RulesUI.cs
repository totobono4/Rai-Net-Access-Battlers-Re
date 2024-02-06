using UnityEngine;
using UnityEngine.UI;

public class RulesUI : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            Debug.Log("kkkk");
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }
}
