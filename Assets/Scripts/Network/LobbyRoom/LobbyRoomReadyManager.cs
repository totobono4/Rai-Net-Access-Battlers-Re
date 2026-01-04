using System;
using System.Collections.Generic;
using Unity.Netcode;

public class LobbyRoomReadyManager : NetworkBehaviour
{
    public static LobbyRoomReadyManager Instance { get; private set; }

    private Dictionary<ulong, bool> clientsReady;

    public EventHandler OnClientReadyStateChanged;

    public EventHandler OnStartGame;

    private void Awake() {
        Instance = this;

        clientsReady = new Dictionary<ulong, bool>();
    }

    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        foreach (ulong connectedClientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (clientsReady.ContainsKey(connectedClientId)) {
                UpdateReadyState(connectedClientId, clientsReady[connectedClientId]);
            }
        }
    }

    private void TryStartGame() {
        if (NetworkManager.Singleton.ConnectedClientsList.Count < MultiplayerManager.Instance.GetMinPlayerCount()) return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!clientsReady.ContainsKey(clientId) || !clientsReady[clientId]) return;
        }

        LobbyManager.Instance.DeleteLobby();

        StartGameClientRpc();

        SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
    }

    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
    private void StartGameClientRpc() {
        OnStartGame?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateReadyState(ulong clientId, bool isReady) {
        UpdateReadyStateClientRpc(clientId, isReady);
    }

    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
    private void UpdateReadyStateClientRpc(ulong clientId, bool isReady) {
        clientsReady[clientId] = isReady;

        OnClientReadyStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TogglePlayerReady() {
        TogglePlayerReadyServerRpc();
    }

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    private void TogglePlayerReadyServerRpc(RpcParams rpcParams = default) {
        ulong clientId = rpcParams.Receive.SenderClientId;
        if (!clientsReady.ContainsKey(clientId)) clientsReady[clientId] = true;
        else clientsReady[clientId] = !clientsReady[clientId];
        UpdateReadyStateClientRpc(clientId, clientsReady[clientId]);

        TryStartGame();
    }

    public bool GetPlayerReady(ulong clientId) {
        return clientsReady.ContainsKey(clientId) && clientsReady[clientId];
    }
}
