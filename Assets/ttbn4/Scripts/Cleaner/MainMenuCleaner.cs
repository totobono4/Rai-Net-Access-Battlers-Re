using Unity.Netcode;
using UnityEngine;

public class MainMenuCleaner : MonoBehaviour
{
    protected virtual void Awake() {
        if (NetworkManager.Singleton != null) Destroy(NetworkManager.Singleton.gameObject);
        if (MultiplayerManager.Instance != null) Destroy(MultiplayerManager.Instance.gameObject);
        if (LobbyManager.Instance != null) Destroy (LobbyManager.Instance.gameObject);
    }
}
