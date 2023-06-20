using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverVisual : MonoBehaviour
{
    [SerializeField] Tile Reference;
    [SerializeField] Transform hilight;

    private PlayerController playerController;

    private void Start() {
        playerController = PlayerController.Instance;

        playerController.OnHoverTileChanged += HoverTileChanged;
    }

    private void HoverTileChanged(object sender, PlayerController.HoverTileChangedArgs e) {
        if (e.hoverTile == Reference) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        hilight.gameObject.SetActive(true);
    }

    private void Hide() {
        hilight.gameObject.SetActive(false);
    }
}
