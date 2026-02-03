using UnityEngine;
using UnityEngine.UI;

public class RaiNetMenuUI : MenuUI
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
