using UnityEngine;

public class ActionableVisual : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] Transform actionable;

    private void Start() {
        tile.OnActionableValueChanged += Tile_OnActionableTile;
    }

    private void Tile_OnActionableTile(object sender, Tile.ActionableTileArgs e) {
        Hide();
        if (e.tile != tile) return;
        if (!e.isActionable) return;
        Show();
    }

    private void Show() {
        actionable.gameObject.SetActive(true);
    }
    private void Hide() {
        actionable.gameObject.SetActive(false);
    }
}
