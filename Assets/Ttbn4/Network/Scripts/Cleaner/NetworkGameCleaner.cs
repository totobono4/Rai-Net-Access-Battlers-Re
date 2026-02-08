using System;
using Ttbn4.Game.Clean;
using Ttbn4.Network.UI;
using Unity.Netcode;
using UnityEngine;

namespace Ttbn4.Network.Clean {
    public abstract class NetworkGameCleaner<TCustomData> : GameCleaner where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
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
}