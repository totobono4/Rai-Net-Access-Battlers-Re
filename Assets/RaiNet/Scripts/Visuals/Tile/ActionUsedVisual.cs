using RaiNet.Game;
using UnityEngine;

namespace RaiNet.Visuals {
    public class ActionUsedVisual : MonoBehaviour {
        [SerializeField] private string ACTION_USED_SHADER_PROPERTY;

        [SerializeField] private Tile tile;
        [SerializeField] private Transform actioned;

        private SpriteRenderer spriteRenderer;

        private void Start() {
            spriteRenderer = actioned.GetComponent<SpriteRenderer>();

            tile.OnActionUsedValueChanged += Tile_OnActionUsed;
        }

        private void Tile_OnActionUsed(object sender, Tile.ActionUsedArgs e) {
            Hide();
            if (e.tile != tile) return;
            if (!e.isUsed) return;
            Show();
        }

        private void OnDestroy() {
            tile.OnActionUsedValueChanged -= Tile_OnActionUsed;
        }

        private void Show() {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetInteger(ACTION_USED_SHADER_PROPERTY, 1);

            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
        private void Hide() {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetInteger(ACTION_USED_SHADER_PROPERTY, 0);

            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}