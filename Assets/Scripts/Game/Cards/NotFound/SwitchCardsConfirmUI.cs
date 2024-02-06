using System;
using UnityEngine;
using UnityEngine.UI;

public class SwitchCardsConfirmUI : MonoBehaviour
{
    [SerializeField] private NotFound notFound;

    [SerializeField] private Button switchButton;
    [SerializeField] private Button dontSwitchButton;

    private void Awake() {
        switchButton.onClick.AddListener(() => {
            notFound.Switch(true);
            Hide();
        });
        dontSwitchButton.onClick.AddListener(() => {
            notFound.Switch(false);
            Hide();
        });
    }

    private void Start() {
        notFound.OnSwitchAsking += NotFound_OnSwitchAsking;
        Hide();
    }

    private void NotFound_OnSwitchAsking(object sender, EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
