using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    [SerializeField] private MultiplayerConfigSO multiplayerConfigSO;

    private int maxPlayerCount;
    private int minPlayerCount;
    private List<Team> playerTeams;

    private NetworkList<PlayerData> playerDataNetworkList;

    public EventHandler OnPlayerDataNetworkListChanged;

    public EventHandler OnTryToConnect;
    public EventHandler OnConnectionFailed;

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(this);

        maxPlayerCount = multiplayerConfigSO.GetMaxPlayerCount();
        minPlayerCount = multiplayerConfigSO.GetMinPlayerCount();
        playerTeams = multiplayerConfigSO.GetPlayerTeams();

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    public int GetMinPlayerCount() {
        return minPlayerCount;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.WaitingScene.ToString()) {
            response.Approved = false;
            response.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsList.Count >= maxPlayerCount) {
            response.Approved = false;
            response.Reason = "Game is full";
            return;
        }
        
        response.Approved = true;
    }

    private void NetworkManager_OnClientConnected(ulong clientId) {
        Team team = playerTeams[playerDataNetworkList.Count];

        playerDataNetworkList.Add(new PlayerData {
            clientId = clientId,
            team = team,
        });
    }

    private void NetworkManager_OnClientDisconnect(ulong clientId) {
        foreach (PlayerData playerData in playerDataNetworkList) if (playerData.clientId == clientId) playerDataNetworkList.Remove(playerData);
    }

    public void StartClient() {
        OnTryToConnect?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        OnConnectionFailed?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
    }

    public bool TryGetClientTeamById(ulong clientId, out Team team) {
        team = default;
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (clientId == playerData.clientId) {
                team = playerData.team;
                return true;
            }
        }
        return false;
    }

    public bool TryGetClientIdByTeam(Team team, out ulong clientId) {
        clientId = default;
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (team == playerData.team) {
                clientId = playerData.clientId;
                return true;
            }
        }
        return false;
    }
}
