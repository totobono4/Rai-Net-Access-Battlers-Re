using UnityEngine;

public class ActionUsedVisual : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] Transform actioned;

    private void Start() {
        tile.OnActionUsedValueChanged += Tile_OnActionUsed;
    }

    private void Tile_OnActionUsed(object sender, Tile.ActionUsedArgs e) {
        Hide();
        if (e.tile != tile) return;
        if (!e.isUsed) return;
        Show();
    }

    private void Show() {
        actioned.gameObject.SetActive(true);
    }
    private void Hide() {
        actioned.gameObject.SetActive(false);
    }
}
