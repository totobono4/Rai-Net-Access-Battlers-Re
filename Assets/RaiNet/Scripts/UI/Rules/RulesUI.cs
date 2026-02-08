using UnityEngine;
using UnityEngine.UI;

namespace RaiNet.UI {
    public class RulesUI : MonoBehaviour {
        [SerializeField] Button mainMenuButton;

        private void Awake() {
            mainMenuButton.onClick.AddListener(() => {
                SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
            });
        }
    }
}