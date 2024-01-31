using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleaner : MonoBehaviour
{
    private void Awake() {
        if (NetworkManager.Singleton != null) Destroy(NetworkManager.Singleton.gameObject);
        if (MultiplayerManager.Instance != null) Destroy(MultiplayerManager.Instance.gameObject);
    }
}
