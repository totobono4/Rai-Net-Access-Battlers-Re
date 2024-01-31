using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectResponseBoxUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button closeButton;

    private void Awake() {
        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start() {
        MultiplayerManager.Instance.OnConnectionFailed += MultiplayerManager_OnConnectionFailed;

        Hide();
    }

    private void MultiplayerManager_OnConnectionFailed(object sender, EventArgs e) {
        messageText.text = NetworkManager.Singleton.DisconnectReason;
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerManager.Instance.OnConnectionFailed -= MultiplayerManager_OnConnectionFailed;
    }
}
