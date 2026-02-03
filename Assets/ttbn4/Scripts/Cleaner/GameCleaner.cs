using System;
using UnityEngine;

public class GameCleaner : MonoBehaviour
{
    public static GameCleaner Instance { get; private set; }

    [SerializeField] DisconnectedUI disconnectedUI;

    protected virtual void Awake() {
        Instance = this;
    }

    protected virtual void Start() {
        disconnectedUI.OnClean += DisconnectedUI_OnClean;
    }

    protected virtual void DisconnectedUI_OnClean(object sender, EventArgs e) {
        Clean();
    }

    public virtual void Clean() {
        disconnectedUI.OnClean -= DisconnectedUI_OnClean;

        disconnectedUI.Clean();
    }
}
