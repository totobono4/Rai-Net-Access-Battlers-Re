using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class MultiplayerManager<TCustomData> : NetworkBehaviour where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
    private const string PLAYERPREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    public static MultiplayerManager<TCustomData> Instance { get; private set; }

    [SerializeField] private MultiplayerConfigSO multiplayerConfigSO;

    private int maxPlayerCount;
    private int minPlayerCount;
    // private List<PlayerTeam> playerTeams;

    public EventHandler<UpdatePlayersArgs> OnUpdatePlayers;
    public class UpdatePlayersArgs : EventArgs {
        public List<PlayerData<TCustomData>> playerDataList;
    }

    private string playerName;

    public EventHandler OnPlayerDataNetworkListChanged;

    public EventHandler OnTryToConnect;
    public EventHandler OnConnectionFailed;

    public EventHandler OnHostDisconnect;
    public EventHandler OnClientDisconnect;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(this);

        maxPlayerCount = multiplayerConfigSO.GetMaxPlayerCount();
        minPlayerCount = multiplayerConfigSO.GetMinPlayerCount();
        // playerTeams = multiplayerConfigSO.GetPlayerTeams();

        playerName = PlayerPrefs.GetString(PLAYERPREFS_PLAYER_NAME_MULTIPLAYER, GenerateGuestName());

        InitializeNetworkList();
        SubNetworkList();
    }

    protected abstract void InitializeNetworkList();
    protected abstract void SubNetworkList();
    protected abstract void UnsubNetworkList();

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

    protected void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData<TCustomData>> changeEvent) {
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
        AddPlayerData(new PlayerData<TCustomData> {
            basePlayerData = new BasePlayerData {
                clientId = clientId
            },
        });
    }

    protected abstract void AddPlayerData(PlayerData<TCustomData> playerData);
    protected abstract void RemovePlayerData(PlayerData<TCustomData> playerData);
    public abstract PlayerData<TCustomData> GetPlayerDataByIndex(int playerIndex);
    protected abstract void SetPlayerDataByIndex(int playerIndex, PlayerData<TCustomData> playerData);
    public abstract int GetPlayerCount();

    private void DisconnectClient(ulong clientId) {
        int playerCount = GetPlayerCount();
        for (int i = 0; i < playerCount; i++) {
            PlayerData<TCustomData> playerData = GetPlayerDataByIndex(i);
            if (playerData.basePlayerData.clientId != clientId) continue;
            RemovePlayerData(playerData);

        }

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

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    private void SetPlayerIdServerRpc(string playerId, RpcParams rpcParams = default) {
        ulong clientId = rpcParams.Receive.SenderClientId;
        int playerDataIndex = GetPlayerDataIndexFromClientId(clientId);

        PlayerData<TCustomData> playerData = GetPlayerDataByIndex(playerDataIndex);
        playerData.basePlayerData.playerId = playerId;
        SetPlayerDataByIndex(playerDataIndex, playerData);
    }

    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    private void SetPlayerNameServerRpc(FixedString128Bytes playerName, RpcParams rpcParams = default) {
        ulong clientId = rpcParams.Receive.SenderClientId;
        int playerDataIndex = GetPlayerDataIndexFromClientId(clientId);

        PlayerData<TCustomData> playerData = GetPlayerDataByIndex(playerDataIndex);
        playerData.basePlayerData.playerName = playerName;
        SetPlayerDataByIndex(playerDataIndex, playerData);
    }

    private int GetPlayerDataIndexFromClientId(ulong clientId) {
        for (int i = 0; i < GetPlayerCount(); i++) {
            if (GetPlayerDataByIndex(i).basePlayerData.clientId == clientId) {
                return i;
            }
        }
        return -1;
    }

    /*
    public PlayerTeam GetClientTeamById(ulong clientId) {
        return playerDataNetworkList[GetPlayerDataIndexFromClientId(clientId)].team;
    }
    */

    /*
    public List<ulong> GetClientIdsByTeam(PlayerTeam team) {
        List<ulong> ids = new List<ulong>();
        foreach (BasePlayerData playerData in playerDataNetworkList) {
            if (playerData.team == team) {
                ids.Add(playerData.clientId);
            }
        }
        return ids;
    }
    */

    public bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < GetPlayerCount();
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

        UnsubNetworkList();

        NetworkManager.Shutdown();
    }
}
