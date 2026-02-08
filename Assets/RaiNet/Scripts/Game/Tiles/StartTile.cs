using RaiNet.Data;
using System;
using Unity.Netcode;

namespace RaiNet.Game {
    public class StartTile : BoardTile {
        public EventHandler<OnlineCardPlacedArgs> OnOnlineCardPlaced;
        public class OnlineCardPlacedArgs : EventArgs {
            public StartTile startTile;
            public bool onlineCardPlaced;
        }

        private NetworkVariable<bool> onlineCardPlaced;

        protected override void Awake() {
            base.Awake();

            onlineCardPlaced = new NetworkVariable<bool>(false);
            onlineCardPlaced.OnValueChanged += OnlineCardPlacedValueChanged;
        }

        private void OnlineCardPlacedValueChanged(bool previousValue, bool newValue) {
            OnOnlineCardPlaced?.Invoke(this, new OnlineCardPlacedArgs {
                startTile = this,
                onlineCardPlaced = newValue,
            });
        }

        public bool IsOnlineCardPlaced() {
            return onlineCardPlaced.Value;
        }

        private void SetOnlineCardPlaced() {
            onlineCardPlaced.Value = true;
        }

        private void UnsetOnlineCardPlaced() {
            onlineCardPlaced.Value = false;
        }

        public void TryPlaceOnlineCard(OnlineCardState onlineCardState, PlayerTeam team) {
            if (GameBoard.Instance.TryPlaceOnlineCard(this, onlineCardState, team)) SetOnlineCardPlaced();
            else UnsetOnlineCardPlaced();
        }

        public override void Clean() {
            onlineCardPlaced.OnValueChanged -= OnlineCardPlacedValueChanged;
        }
    }
}