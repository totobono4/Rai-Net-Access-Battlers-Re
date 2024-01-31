using System;
using UnityEngine;

public class HoverVisual : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] Transform hilight;

    private PlayerController playerController;

    private void Start() {
        if (PlayerController.LocalInstance == null) PlayerController.OnAnyPlayerSpawned += PlayerController_OnAnyPlayerSpawned;
        else SetPlayerController();
    }

    private void PlayerController_OnAnyPlayerSpawned(object sender, EventArgs e) {
        if (PlayerController.LocalInstance == null) return;

        SetPlayerController();

        PlayerController.OnAnyPlayerSpawned -= PlayerController_OnAnyPlayerSpawned;
    }

    private void SetPlayerController() {
        playerController = PlayerController.LocalInstance;

        playerController.OnHoverTileChanged += PlayerController_OnHoverTileChanged;
    }

    private void PlayerController_OnHoverTileChanged(object sender, PlayerController.HoverTileChangedArgs e) {
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
