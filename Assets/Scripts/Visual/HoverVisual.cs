using System;
using UnityEngine;

public class HoverVisual : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] Transform hilight;

    private PlayerController playerController;

    private void Start() {
        if (PlayerController.LocalInstance == null) PlayerController.OnAnyPlayerSpawned += PlayerControllerSpawned;
        else SetPlayerController();
    }

    private void PlayerControllerSpawned(object sender, EventArgs e) {
        if (PlayerController.LocalInstance == null) return;

        SetPlayerController();

        PlayerController.OnAnyPlayerSpawned -= PlayerControllerSpawned;
    }

    private void SetPlayerController() {
        playerController = PlayerController.LocalInstance;

        playerController.OnHoverTileChanged += HoverTileChanged;
    }

    private void HoverTileChanged(object sender, PlayerController.HoverTileChangedArgs e) {
        Hide();
        if (e.hoverTile == tile) Show();
    }

    private void Show() {
        hilight.gameObject.SetActive(true);
    }

    private void Hide() {
        hilight.gameObject.SetActive(false);
    }
}
