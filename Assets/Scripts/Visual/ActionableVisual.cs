using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionableVisual : MonoBehaviour
{
    [SerializeField] Tile Reference;
    [SerializeField] Transform actionable;

    private void Start() {
        if (Reference.TryGetComponent<NormalTile>(out NormalTile normalTile)) {
            normalTile.OnActionableTile += ActionableTile;
        }
    }

    private void ActionableTile(object sender, NormalTile.ActionableTileArgs e) {
        if (e.actionableTile == Reference && e.isActionable == true) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        actionable.gameObject.SetActive(true);
    }

    private void Hide() {
        actionable.gameObject.SetActive(false);
    }
}
