using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
    [SerializeField] Tile Reference;
    [SerializeField] Transform selected;

    private void Start() {
        if (Reference.TryGetComponent<NormalTile>(out NormalTile normalTile)) {
            normalTile.OnSelectedTile += SelectedTile;
        }
    }

    private void SelectedTile(object sender, NormalTile.SelectedTileArgs e) {
        if (e.selectedTile == Reference && e.isSelected) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        selected.gameObject.SetActive(true);
    }

    private void Hide() {
        selected.gameObject.SetActive(false);
    }
}
