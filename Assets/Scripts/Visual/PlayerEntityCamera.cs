using Unity.Netcode;
using UnityEngine;

public class PlayerEntityCamera : NetworkBehaviour {

    private GameBoard gameBoard;
    [SerializeField] private PlayerEntity playerEntity;
    [SerializeField] private Transform cameraOrigin;

    private void Start() {
        gameBoard = GameBoard.Instance;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        PlayerController.OnTeamChanged += LocalTeamChanged;
    }

    private void LocalTeamChanged(object sender, PlayerController.OnTeamChangedArgs e) {
        if (!sender.Equals(PlayerController.LocalInstance)) return;
        if (playerEntity.GetTeam() != e.team) return;

        Camera.main.transform.position = cameraOrigin.position;
        Camera.main.transform.rotation = cameraOrigin.rotation;
    }
}
