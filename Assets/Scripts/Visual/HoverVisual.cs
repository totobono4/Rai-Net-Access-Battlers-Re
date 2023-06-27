using UnityEngine;

public class HoverVisual : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] Transform hilight;

    private PlayerController playerController;

    private void Start() {
        playerController = PlayerController.Instance;

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
