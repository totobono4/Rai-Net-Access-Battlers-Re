using UnityEngine;

public class ActionUsedVisual : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] Transform actioned;

    private void Start() {
        tile.OnActionUsed += ActionUsed;
    }

    private void ActionUsed(object sender, Tile.ActionUsedArgs e) {
        Hide();
        if (e.usedTile == tile && e.isUsed == true) Show();
    }

    private void Show() {
        actioned.gameObject.SetActive(true);
    }

    private void Hide() {
        actioned.gameObject.SetActive(false);
    }
}
