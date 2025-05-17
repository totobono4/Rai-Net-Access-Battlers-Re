using System;
using UnityEngine;

public class TerminalCardVisual : MonoBehaviour
{
    [SerializeField] private TerminalCard terminalCard;

    [SerializeField] private Transform front, back;

    private void Awake() {
        terminalCard.OnUsedValueChanged += TerminalCard_OnUsedValueChanged;
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
}
