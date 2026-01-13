using UnityEngine;
using UnityEngine.UI;

public class CreditUI : MonoBehaviour
{
    [SerializeField] Button menuButton;

    private void Awake() {
        menuButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }
}
