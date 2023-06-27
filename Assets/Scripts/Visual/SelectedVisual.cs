using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
    [SerializeField] private Tile tile;
    [SerializeField] private Transform selected;

    private void Start() {
        tile.OnSelectedTile += SelectedTile;
    }

    private void SelectedTile(object sender, Tile.SelectedTileArgs e) {
        Hide();
        if (e.selectedTile == tile && e.isSelected) Show();
    }

    private void Show() {
        selected.gameObject.SetActive(true);
    }

    private void Hide() {
        selected.gameObject.SetActive(false);
    }
}
