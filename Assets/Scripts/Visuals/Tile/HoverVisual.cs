using System;
using UnityEngine;

public class HoverVisual : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] Transform hilight;

    private void Start() {
        if (PlayerController.LocalInstance == null) PlayerController.OnAnyPlayerSpawned += PlayerController_OnAnyPlayerSpawned;
        else PlayerController.LocalInstance.OnHoverTileChanged += PlayerController_OnHoverTileChanged;
    }

    private void PlayerController_OnAnyPlayerSpawned(object sender, EventArgs e) {
        if (PlayerController.LocalInstance == null) return;
        PlayerController.OnAnyPlayerSpawned -= PlayerController_OnAnyPlayerSpawned;
        PlayerController.LocalInstance.OnHoverTileChanged += PlayerController_OnHoverTileChanged;
    }

    private void PlayerController_OnHoverTileChanged(object sender, PlayerController.HoverTileChangedArgs e) {
        Hide();
        if (e.tile != tile) return;
        Show();
    }

    private void OnDestroy() {
        if (PlayerController.LocalInstance) PlayerController.LocalInstance.OnHoverTileChanged -= PlayerController_OnHoverTileChanged;
    }

    private void Show() {
        hilight.gameObject.SetActive(true);
    }
    private void Hide() {
        hilight.gameObject.SetActive(false);
    }
}
