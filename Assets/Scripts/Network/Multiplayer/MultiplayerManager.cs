using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    private const string PLAYERPREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    public static MultiplayerManager Instance { get; private set; }

    [SerializeField] private MultiplayerConfigSO multiplayerConfigSO;

    private int maxPlayerCount;
    private int minPlayerCount;
    private List<PlayerTeam> playerTeams;

    private NetworkList<PlayerData> playerDataNetworkList;

    private string playerName;

    public EventHandler OnPlayerDataNetworkListChanged;

    public EventHandler<UpdatePlayersArgs> OnUpdatePlayers;
    public class UpdatePlayersArgs : EventArgs {
        public List<PlayerData> playerDataList;
    }

    public EventHandler OnTryToConnect;
    public EventHandler OnConnectionFailed;

    public EventHandler OnHostDisconnect;
    public EventHandler OnClientDisconnect;

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(this);

        maxPlayerCount = multiplayerConfigSO.GetMaxPlayerCount();
        minPlayerCount = multiplayerConfigSO.GetMinPlayerCount();
        playerTeams = multiplayerConfigSO.GetPlayerTeams();

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

        playerName = PlayerPrefs.GetString(PLAYERPREFS_PLAYER_NAME_MULTIPLAYER, GenerateGuestName());
    }

    private string GenerateGuestName() {
        return "Guest-" + UnityEngine.Random.Range(1000, 10000).ToString();
    }

    public int GetMaxPlayerCount() {
        return maxPlayerCount;
    }

    public int GetMinPlayerCount() {
        return minPlayerCount;
    }

    public void SetPlayerName(string playerName) {
        this.playerName = playerName;

        PlayerPrefs.SetString(PLAYERPREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }

    public string GetPlayerName() {
        return playerName;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        response.Approved = true;
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId) {
        PlayerTeam team = playerTeams[playerDataNetworkList.Count];

        playerDataNetworkList.Add(new PlayerData {
            clientId = clientId,
            team = team,
        });
    }

    private void DisconnectClient(ulong clientId) {
        foreach (PlayerData playerData in playerDataNetworkList) if (playerData.clientId == clientId) playerDataNetworkList.Remove(playerData);
        OnClientDisconnect?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
        DisconnectClient(clientId);
    }

    public void KickPlayerByClientId(ulong clientId) {
        NetworkManager.Singleton.DisconnectClient(clientId);
        DisconnectClient(clientId);
    }

    public void StartClient() {
        OnTryToConnect?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;

        if (SceneManager.GetActiveScene().name == SceneLoader.Scene.LobbyScene.ToString()) {
            OnConnectionFailed?.Invoke(this, EventArgs.Empty);
        }

        OnHostDisconnect?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong obj) {
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        SetPlayerNameServerRpc(playerName);
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        int playerDataIndex = GetPlayerDataIndexFromClientId(clientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    private void SetPlayerNameServerRpc(FixedString128Bytes playerName, ServerRpcParams serverRpcParams = default) {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        int playerDataIndex = GetPlayerDataIndexFromClientId(clientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private int GetPlayerDataIndexFromClientId(ulong clientId) {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientId == clientId) {
                return i;
            }
        }
        return -1;
    }

    public PlayerTeam GetClientTeamById(ulong clientId) {
        return playerDataNetworkList[GetPlayerDataIndexFromClientId(clientId)].team;
    }

    public List<ulong> GetClientIdsByTeam(PlayerTeam team) {
        List<ulong> ids = new List<ulong>();
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.team == team) {
                ids.Add(playerData.clientId);
            }
        }
        return ids;
    }

    public bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataByIndex(int playerIndex) {
        return playerDataNetworkList[playerIndex];
    }

    public int GetPlayerCount() {
        return playerDataNetworkList.Count;
    }

    public void Clean() {
        if (NetworkManager.Singleton.IsHost) {
            NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Server_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Client_OnClientConnectedCallback;
        }
        else {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Client_OnClientConnectedCallback;
        }

        playerDataNetworkList.OnListChanged -= PlayerDataNetworkList_OnListChanged;

        NetworkManager.Shutdown();
    }
}
