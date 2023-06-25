using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
    [SerializeField] Tile Reference;
    [SerializeField] Transform selected;

    private void Start() {
        if (Reference.TryGetComponent(out PlayTile normalTile)) {
            normalTile.OnSelectedTile += SelectedTile;
        }
    }

    private void SelectedTile(object sender, Tile.SelectedTileArgs e) {
        Hide();
        if (e.selectedTile == Reference && e.isSelected) Show();
    }

    private void Show() {
        selected.gameObject.SetActive(true);
    }

    private void Hide() {
        selected.gameObject.SetActive(false);
    }
}
