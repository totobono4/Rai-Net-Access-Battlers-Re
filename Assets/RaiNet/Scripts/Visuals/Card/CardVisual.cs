using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardVisual : MonoBehaviour {
    [SerializeField] private string SELECTED_SHADER_PROPERTY;

    [SerializeField] private Card card;

    private Tile tileParent;
    protected List<SpriteRenderer> spriteRenderers;

    protected virtual void Awake() {
        spriteRenderers = InitializeSpriteRenderers();
        spriteRenderers.ForEach(sr => sr.material = new Material(sr.material));

        card.OnTileParentChanged += Card_OnTileParentChanged;
        card.OnClean += Card_OnClean;
    }

    protected abstract List<SpriteRenderer> InitializeSpriteRenderers();

    private void Card_OnTileParentChanged(object sender, Card.TileParentChangedArgs e) {
        HideSelected();
        if (tileParent != null) tileParent.OnSelectedValueChanged -= TileParent_OnSelectedValueChanged;
        tileParent = e.tile;
        tileParent.OnSelectedValueChanged += TileParent_OnSelectedValueChanged;
    }

    private void TileParent_OnSelectedValueChanged(object sender, Tile.SelectedTileArgs e) {
        HideSelected();
        if (e.tile != tileParent) return;
        if (!e.isSelected) return;
        ShowSelected();
    }

    private void ShowSelected() {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetInteger(SELECTED_SHADER_PROPERTY, 1);

        spriteRenderers.ForEach(sr => sr.SetPropertyBlock(propertyBlock));
    }

    private void HideSelected() {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetInteger(SELECTED_SHADER_PROPERTY, 0);

        spriteRenderers.ForEach(sr => sr.SetPropertyBlock(propertyBlock));
    }

    private void Card_OnClean(object sender, EventArgs e) {
        if (tileParent != null) tileParent.OnSelectedValueChanged -= TileParent_OnSelectedValueChanged;
        card.OnTileParentChanged -= Card_OnTileParentChanged;
        card.OnClean -= Card_OnClean;

        spriteRenderers.ForEach(sr => Destroy(sr.material));
        spriteRenderers.Clear();
    }
}
