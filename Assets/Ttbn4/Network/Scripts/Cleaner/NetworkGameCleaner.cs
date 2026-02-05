using System;
using Unity.Netcode;
using UnityEngine;

public abstract class NetworkGameCleaner<TCustomData> : GameCleaner where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable
{
    [SerializeField] DisconnectedUI<TCustomData> disconnectedUI;

    protected override void Start() {
        base.Start();
        disconnectedUI.OnClean += DisconnectedUI_OnClean;
    }

    protected virtual void DisconnectedUI_OnClean(object sender, EventArgs e) {
        Clean();
    }

    protected override void Clean() {
        base.Clean();
        disconnectedUI.OnClean -= DisconnectedUI_OnClean;
        disconnectedUI.Clean();
    }
}
