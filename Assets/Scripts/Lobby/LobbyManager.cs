using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private const float HEARTBEAT_DELAY = 15f;
    private const float REFRESH_DELAY = 3f;

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private const string KEY_PROTOCOL_VERSION = "ProtocolVersion";
    private const string KEY_HOST_BUILD_VERSION = "HostBuildVersion";

    private bool canQuit;

    public static LobbyManager Instance { get; private set; }

    private float heartbeatTimer;
    private float refreshTimer;
    private Lobby currentLobby;

    public EventHandler OnTryCreateLobby;
    public EventHandler<LobbyServiceExceptionArgs> OnCreateLobbyFailed;

    public EventHandler OnTryQuickJoinLobby;
    public EventHandler<LobbyServiceExceptionArgs> OnQuickJoinLobbyFailed;

    public EventHandler<RefreshLobbiesUpdateArgs> OnRefreshLobbiesUpdate;
    public class RefreshLobbiesUpdateArgs : EventArgs {
        public List<Lobby> lobbies;
    }

    public EventHandler OnTryJoinLobbyById;
    public EventHandler<LobbyServiceExceptionArgs> OnJoinLobbyByIdFailed;

    public EventHandler OnTryJoinLobbyByCode;
    public EventHandler<LobbyServiceExceptionArgs> OnJoinLobbyByCodeFailed;

    public class LobbyServiceExceptionArgs : EventArgs {
        public LobbyServiceException lobbyServiceException;
    }

    public EventHandler<RefusedToJoinLobbyArgs> OnRefusedToJoinLobby;
    public class RefusedToJoinLobbyArgs : EventArgs {
        public string message;
    }

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeAuthenticationServices();

        heartbeatTimer = HEARTBEAT_DELAY;
        refreshTimer = REFRESH_DELAY;
        currentLobby = null;

        canQuit = true;
        Application.wantsToQuit += Application_WantsToQuit;

        AuthenticationService.Instance.SignedIn += AuthentificationService_SignedIn;
    }

    private async void LeaveAndQuit() {
        if (!AuthenticationService.Instance.IsSignedIn) {
            canQuit = true;
            Application.Quit();
        }

        if (!LobbyExists(currentLobby)) {
            canQuit = true;
            Application.Quit();
        }

        try {
            if (IsLobbyHost(currentLobby)) await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
            else await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);

            canQuit = true;
            Application.Quit();
        }
        catch (LobbyServiceException e) {
            Debug.LogException(e);

            canQuit = true;
            Application.Quit();
        }
    }

    private bool Application_WantsToQuit() {
        if (!AuthenticationService.Instance.IsSignedIn) return false;
        if (!canQuit) LeaveAndQuit();
        return canQuit;
    }

    private void AuthentificationService_SignedIn() {
        RefreshLobbies();
    }

    private void Update() {
        if (IsLobbyHost(currentLobby)) HandleLobbyHeartBeat();
        HandleLobbyRefresh();
    }

    private async void InitializeAuthenticationServices() {
        if (UnityServices.State == ServicesInitializationState.Initialized) return;

        InitializationOptions options = new InitializationOptions();
        options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private bool LobbyExists(Lobby lobby) {
        return lobby != null;
    }

    private async void HandleLobbyHeartBeat() {
        try {
            if (!LobbyExists(currentLobby)) return;

            heartbeatTimer -= Time.deltaTime;

            if (heartbeatTimer > 0) return;

            heartbeatTimer = HEARTBEAT_DELAY;
            await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        } catch (LobbyServiceException e) {
            Debug.LogException(e);
        }
    }

    private void HandleLobbyRefresh() {
        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.LobbyScene.ToString()) return;

        if (!AuthenticationService.Instance.IsSignedIn) return;

        refreshTimer -= Time.deltaTime;

        if (refreshTimer > 0) return;

        refreshTimer = REFRESH_DELAY;
        RefreshLobbies();
    }

    public bool IsLobbyHost(Lobby lobby) {
        if (!LobbyExists(lobby)) return false;
        return lobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async Task<Allocation> AllocateRelay() {
        try {
            return await RelayService.Instance.CreateAllocationAsync(MultiplayerManager.Instance.GetMaxPlayerCount() - 1);
        } catch (RelayServiceException e) {
            Debug.LogException(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation) {
        try {
            return await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        } catch (RelayServiceException e) {
            Debug.LogException(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode) {
        try {
            return await RelayService.Instance.JoinAllocationAsync(joinCode);
        } catch (RelayServiceException e) {
            Debug.LogException(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate) {
        try {
            OnTryCreateLobby?.Invoke(this, EventArgs.Empty);

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);
            ushort protocolVersion = NetworkManager.Singleton.NetworkConfig.ProtocolVersion;

            int maxPlayers = MultiplayerManager.Instance.GetMaxPlayerCount();
            CreateLobbyOptions options = new CreateLobbyOptions() {
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject> {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) },
                    { KEY_PROTOCOL_VERSION, new DataObject(DataObject.VisibilityOptions.Public, protocolVersion.ToString()) },
                    { KEY_HOST_BUILD_VERSION, new DataObject(DataObject.VisibilityOptions.Public, Application.version) }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            MultiplayerManager.Instance.StartHost();
            SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyRoomScene);

            canQuit = false;
        } catch (LobbyServiceException e) {
            Debug.LogException(e);
            OnCreateLobbyFailed?.Invoke(this, new LobbyServiceExceptionArgs {
                lobbyServiceException = e
            });
        }
    }

    private bool DataKeyExists(Lobby lobby, string dataKey) {
        if (lobby.Data == null) return false;
        if (!lobby.Data.ContainsKey(dataKey)) return false;
        return true;
    }

    private string GetHostProtocolVersion(Lobby lobby) {
        if (!LobbyExists(lobby)) return "0";
        if (!DataKeyExists(lobby, KEY_PROTOCOL_VERSION)) return "0";
        return lobby.Data[KEY_PROTOCOL_VERSION].Value;
    }

    private bool IsNetworkCompatible(Lobby lobby) {
        if (!LobbyExists(lobby)) return false;
        return GetHostProtocolVersion(lobby) == NetworkManager.Singleton.NetworkConfig.ProtocolVersion.ToString();
    }

    private async void JoinLobby() {
        try {
            if (!IsNetworkCompatible(currentLobby)) {
                OnRefusedToJoinLobby?.Invoke(this, new RefusedToJoinLobbyArgs {
                    message = "Not Network Compatible with host's Version"
                });
                LeaveLobby();
                return;
            }

            string relayJoinCode = currentLobby.Data[KEY_RELAY_JOIN_CODE].Value;    

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerManager.Instance.StartClient();

            canQuit = false;
        } catch (LobbyServiceException e) {
            throw new LobbyServiceException(e);
        }
    }

    public async void QuickJoinLobby() {
        try {
            OnTryQuickJoinLobby?.Invoke(this, EventArgs.Empty);

            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            JoinLobby();
        } catch (LobbyServiceException e) {
            Debug.LogException(e);
            OnQuickJoinLobbyFailed?.Invoke(this, new LobbyServiceExceptionArgs {
                lobbyServiceException = e
            });
        }
    }

    public async void RefreshLobbies() {
        try {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryRespone = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnRefreshLobbiesUpdate?.Invoke(this, new RefreshLobbiesUpdateArgs { lobbies = queryRespone.Results });
        } catch (LobbyServiceException e) {
            Debug.LogException(e);
        }
    }

    public async void JoinLobbyById(string lobbyId) {
        try {
            OnTryJoinLobbyById?.Invoke(this, EventArgs.Empty);

            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            JoinLobby();
        } catch (LobbyServiceException e) {
            Debug.LogException(e);
            OnJoinLobbyByIdFailed?.Invoke(this, new LobbyServiceExceptionArgs {
                lobbyServiceException = e
            });
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        try {
            OnTryJoinLobbyByCode?.Invoke(this, EventArgs.Empty);

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            JoinLobby();
        } catch (LobbyServiceException e) {
            Debug.LogException(e);
            OnJoinLobbyByCodeFailed?.Invoke(this, new LobbyServiceExceptionArgs {
                lobbyServiceException = e
            });
        }
    }

    public async void DeleteLobby() {
        if (!LobbyExists(currentLobby)) return;

        try {
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);

            currentLobby = null;
        } catch (LobbyServiceException e) {
            Debug.LogException(e);
        }
    }

    public async void LeaveLobby() {
        if (!LobbyExists(currentLobby)) return;

        try {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);

            currentLobby = null;
        } catch (LobbyServiceException e) {
            Debug.LogException(e);
        }
    }

    public async void KickPlayer(string playerId) {
        if (!LobbyExists(currentLobby)) return;

        try {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
        } catch (LobbyServiceException e) {
            Debug.LogException(e);
        }
    }

    public string GetLobbyName() {
        if (!LobbyExists(currentLobby)) return null;
        return currentLobby.Name;
    }

    public string GetLobbyCode() {
        if (!LobbyExists(currentLobby)) return null;
        return currentLobby.LobbyCode;
    }

    public string GetLobbyHostBuildVersion(Lobby lobby) {
        if (lobby == null) return null;
        if (!DataKeyExists(lobby, KEY_HOST_BUILD_VERSION)) return "0.1.0b\nor below";
        return lobby.Data[KEY_HOST_BUILD_VERSION].Value;
    }

    public bool IsLobbyNeworkCompatible(Lobby lobby) {
        if (lobby == null) return false;

        return IsNetworkCompatible(lobby);
    }
}
