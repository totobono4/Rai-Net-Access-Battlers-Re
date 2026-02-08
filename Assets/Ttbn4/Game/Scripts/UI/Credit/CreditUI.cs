using UnityEngine;
using UnityEngine.UI;

namespace Ttbn4.Game.UI {
    public class CreditUI : MonoBehaviour {
        [SerializeField] Button menuButton;

        private void Awake() {
            menuButton.onClick.AddListener(() => {
                SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
            });
        }
    }
}