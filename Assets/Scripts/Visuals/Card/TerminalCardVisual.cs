using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalCardVisual : CardVisual
{
    [SerializeField] private TerminalCard terminalCard;

    [SerializeField] private Transform front, back;

    protected override void Awake() {
        base.Awake();

        terminalCard.OnUsedValueChanged += TerminalCard_OnUsedValueChanged;
        terminalCard.OnClean += TerminalCard_OnClean;
    }

    protected override List<SpriteRenderer> InitializeSpriteRenderers() {
        return new List<SpriteRenderer>() {
            front.GetComponent<SpriteRenderer>(),
            back.GetComponent<SpriteRenderer>(),
        };
    }

    private void TerminalCard_OnUsedValueChanged(object sender, EventArgs e) {
        ShowFront();
        HideBack();
        if (!terminalCard.IsUsed()) return;
        ShowBack();
        HideFront();
    }

    private void ShowFront() {
        front.gameObject.SetActive(true);
    }
    private void HideFront() {
        front.gameObject.SetActive(false);
    }

    private void ShowBack() {
        back.gameObject.SetActive(true);
    }
    private void HideBack() {
        back.gameObject.SetActive(false);
    }

    private void TerminalCard_OnClean(object sender, EventArgs e) {
        terminalCard.OnUsedValueChanged -= TerminalCard_OnUsedValueChanged;
        terminalCard.OnClean -= TerminalCard_OnClean;
    }
}
