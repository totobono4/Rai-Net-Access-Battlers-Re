using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseTabUI : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button mainMenuButton;

    public EventHandler OnClean;

    protected virtual void Awake() {
        continueButton.onClick.AddListener(() => {
            Hide();
        });
        mainMenuButton.onClick.AddListener(() => {
            OnClean?.Invoke(this, EventArgs.Empty);
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }

    public virtual void Show() {
        gameObject.SetActive(true);
    }

    public virtual void Hide() {
        gameObject.SetActive(false);
    }

    public virtual void Clean() {
        Destroy(gameObject);
    }
}
