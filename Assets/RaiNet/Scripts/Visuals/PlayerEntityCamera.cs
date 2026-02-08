using RaiNet.Game;
using Unity.Netcode;
using UnityEngine;

namespace RaiNet.Visuals {
    public class PlayerEntityCamera : NetworkBehaviour {
        [SerializeField] private PlayerEntity playerEntity;
        [SerializeField] private Transform cameraOrigin;

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();

            PlayerController.OnTeamChanged += PlayerController_OnTeamChanged;
        }

        private void PlayerController_OnTeamChanged(object sender, PlayerController.TeamChangedArgs e) {
            if (!sender.Equals(PlayerController.LocalInstance)) return;
            if (playerEntity.GetTeam() != e.team) return;

            Camera.main.transform.position = cameraOrigin.position;
            Camera.main.transform.rotation = cameraOrigin.rotation;
        }

        public override void OnDestroy() {
            base.OnDestroy();

            PlayerController.OnTeamChanged -= PlayerController_OnTeamChanged;
        }
    }
}