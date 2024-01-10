using UnityEngine;

public class PlayerEntityCamera : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private PlayerEntity playerEntity;
    [SerializeField] private Transform cameraOrigin;

    private void Start() {
        //playerController = PlayerController.Instance;

        //if (playerEntity.GetTeam() != playerController.GetTeam()) return;

        Camera.main.transform.position = cameraOrigin.position;
        Camera.main.transform.rotation = cameraOrigin.rotation;
    }
}
