using System;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleaner<TCustomData> : MonoBehaviour where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
    protected virtual void Awake() {
        if (NetworkManager.Singleton != null) Destroy(NetworkManager.Singleton.gameObject);
        if (MultiplayerManager<TCustomData>.Instance != null) Destroy(MultiplayerManager<TCustomData>.Instance.gameObject);
        if (LobbyManager<TCustomData>.Instance != null) Destroy (LobbyManager<TCustomData>.Instance.gameObject);
    }
}
