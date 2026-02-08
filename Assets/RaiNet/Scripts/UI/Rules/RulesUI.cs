using Totobono4.SceneLoader;
using UnityEngine;
using UnityEngine.UI;

namespace RaiNet.UI {
    public class RulesUI : MonoBehaviour {
        [SerializeField] Button mainMenuButton;

        private void Awake() {
            mainMenuButton.onClick.AddListener(() => {
                SceneLoaderCore.Load(SceneLoaderCore.Scene.MainMenuScene);
            });
        }
    }
}