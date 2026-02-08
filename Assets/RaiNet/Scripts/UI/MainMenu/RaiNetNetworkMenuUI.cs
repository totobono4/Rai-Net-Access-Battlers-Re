using Totobono4.Network.UI;
using Totobono4.SceneLoader;
using UnityEngine;
using UnityEngine.UI;

namespace RaiNet.UI {
    public class RaiNetNetworkMenuUI : NetworkMenuUI {
        [SerializeField] private string rulesSceneName;
        [SerializeField] private Button rulesButton;

        protected override void Awake() {
            base.Awake();

            rulesButton.onClick.AddListener(() => {
                SceneLoaderCore.Load(rulesSceneName);
            });
        }
    }
}