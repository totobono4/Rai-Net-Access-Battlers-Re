using System;
using UnityEditor;
using UnityEngine;

public class HoverVisual : MonoBehaviour {
    [SerializeField] private string HILIGHT_SHADER_PROPERTY;

    [SerializeField] private Tile tile;
    [SerializeField] private Transform highlight;

    private SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = highlight.GetComponent<SpriteRenderer>();

        if (PlayerController.LocalInstance == null) PlayerController.OnAnyPlayerSpawned += PlayerController_OnAnyPlayerSpawned;
        else PlayerController.LocalInstance.OnHoverTileChanged += PlayerController_OnHoverTileChanged;
    }

    private void PlayerController_OnAnyPlayerSpawned(object sender, EventArgs e) {
        if (PlayerController.LocalInstance == null) return;
        PlayerController.OnAnyPlayerSpawned -= PlayerController_OnAnyPlayerSpawned;
        PlayerController.LocalInstance.OnHoverTileChanged += PlayerController_OnHoverTileChanged;
    }

    private void PlayerController_OnHoverTileChanged(object sender, PlayerController.HoverTileChangedArgs e) {
        Hide();
        if (e.tile != tile) return;
        Show();
    }

    private void OnDestroy() {
        if (PlayerController.LocalInstance) PlayerController.LocalInstance.OnHoverTileChanged -= PlayerController_OnHoverTileChanged;
    }

    private void Show() {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetInteger(HILIGHT_SHADER_PROPERTY, 1);

        spriteRenderer.SetPropertyBlock(propertyBlock);

    }
    private void Hide() {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetInteger(HILIGHT_SHADER_PROPERTY, 0);

        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
}
