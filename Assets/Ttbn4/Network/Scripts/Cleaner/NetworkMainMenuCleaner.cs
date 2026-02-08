using System;
using Ttbn4.Game.Clean;
using Unity.Netcode;

namespace Ttbn4.Network.Clean {
    public abstract class NetworkMainMenuCleaner<TCustomData> : MainMenuCleaner where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
        protected override void Awake() {
            if (NetworkManager.Singleton != null) Destroy(NetworkManager.Singleton.gameObject);
            if (MultiplayerManager<TCustomData>.Instance != null) Destroy(MultiplayerManager<TCustomData>.Instance.gameObject);
            if (LobbyManager<TCustomData>.Instance != null) Destroy(LobbyManager<TCustomData>.Instance.gameObject);
        }
    }
}