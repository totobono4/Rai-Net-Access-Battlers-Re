using UnityEngine;

public class ActionableVisual : MonoBehaviour
{
    [SerializeField] private string ACTIONABLE_SHADER_PROPERTY;

    [SerializeField] private Tile tile;
    [SerializeField] private Transform actionable;

    private SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = actionable.GetComponent<SpriteRenderer>();

        tile.OnActionableValueChanged += Tile_OnActionableTile;
    }

    private void Tile_OnActionableTile(object sender, Tile.ActionableTileArgs e) {
        Hide();
        if (e.tile != tile) return;
        if (!e.isActionable) return;
        Show();
    }

    private void OnDestroy() {
        tile.OnActionableValueChanged -= Tile_OnActionableTile;
    }

    private void Show() {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetInteger(ACTIONABLE_SHADER_PROPERTY, 1);

        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
    private void Hide() {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetInteger(ACTIONABLE_SHADER_PROPERTY, 0);

        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
}
