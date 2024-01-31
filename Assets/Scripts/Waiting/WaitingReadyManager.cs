using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaitingReadyManager : NetworkBehaviour
{
    public static WaitingReadyManager Instance { get; private set; }

    private Dictionary<ulong, bool> clientsReady;

    private void Awake() {
        Instance = this;

        clientsReady = new Dictionary<ulong, bool>();
    }

    private void TryStartGame() {
        if (NetworkManager.Singleton.ConnectedClientsList.Count < MultiplayerManager.Instance.GetMinPlayerCount()) return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!clientsReady.ContainsKey(clientId) || !clientsReady[clientId]) return;
        }

        SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
    }

    public void SetPlayerReady() {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        clientsReady[serverRpcParams.Receive.SenderClientId] = true;

        TryStartGame();
    }

    public void SetPlayerNotReady() {
        SetPlayerNotReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNotReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        clientsReady[serverRpcParams.Receive.SenderClientId] = false;
    }
}
