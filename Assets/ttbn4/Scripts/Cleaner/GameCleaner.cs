using System;
using Unity.Netcode;
using UnityEngine;

public class GameCleaner<TCustomData> : MonoBehaviour where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
    public static GameCleaner<TCustomData> Instance { get; private set; }

    [SerializeField] DisconnectedUI<TCustomData> disconnectedUI;
    [SerializeField] PauseUI pauseUI;
    [SerializeField] PauseTabUI pauseTabUI;

    protected virtual void Awake() {
        Instance = this;
    }

    protected virtual void Start() {
        disconnectedUI.OnClean += DisconnectedUI_OnClean;
        pauseTabUI.OnClean += PauseTabUI_OnClean;
    }

    protected virtual void DisconnectedUI_OnClean(object sender, EventArgs e) {
        Clean();
    }

    protected virtual void PauseTabUI_OnClean(object sender, EventArgs e) {
        Clean();
    }

    public virtual void Clean() {
        disconnectedUI.OnClean -= DisconnectedUI_OnClean;
        pauseTabUI.OnClean -= PauseTabUI_OnClean;

        disconnectedUI.Clean();
        pauseUI.Clean();
    }
}
