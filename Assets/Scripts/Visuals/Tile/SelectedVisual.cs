using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
    [SerializeField] private Tile tile;
    [SerializeField] private Transform selected;

    private void Start() {
        tile.OnSelectedValueChanged += Tile_OnSelectedTile;
    }

    private void Tile_OnSelectedTile(object sender, Tile.SelectedTileArgs e) {
        Hide();
        if (e.tile != tile) return;
        if (!e.isSelected) return;
        Show();
    }

    private void OnDestroy() {
        tile.OnSelectedValueChanged -= Tile_OnSelectedTile;
    }

    private void Show() {
        selected.gameObject.SetActive(true);
    }
    private void Hide() {
        selected.gameObject.SetActive(false);
    }
}
