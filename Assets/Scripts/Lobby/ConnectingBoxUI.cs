using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingBoxUI : MonoBehaviour
{
    private void Start() {
        MultiplayerManager.Instance.OnTryToConnect += MultiplayerManager_OnTryToConect;
        MultiplayerManager.Instance.OnConnectionFailed += MultiplayerManager_OnConnectionFailed;

        Hide();
    }

    private void MultiplayerManager_OnTryToConect(object sender, EventArgs e) {
        Show();
    }

    private void MultiplayerManager_OnConnectionFailed(object sender, EventArgs e) {
        Hide();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerManager.Instance.OnTryToConnect -= MultiplayerManager_OnTryToConect;
        MultiplayerManager.Instance.OnConnectionFailed -= MultiplayerManager_OnConnectionFailed;
    }
}
