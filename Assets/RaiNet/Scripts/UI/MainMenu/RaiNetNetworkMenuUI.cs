using UnityEngine;
using UnityEngine.UI;

public class RaiNetNetworkMenuUI : NetworkMenuUI
{
    [SerializeField] private string rulesSceneName;
    [SerializeField] private Button rulesButton;

    protected override void Awake() {
        base.Awake();

        rulesButton.onClick.AddListener(() => {
            SceneLoader.Load(rulesSceneName);
        });
    }
}
