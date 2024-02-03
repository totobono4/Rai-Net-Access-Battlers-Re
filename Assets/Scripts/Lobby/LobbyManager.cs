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

public class LobbyManager : MonoBehaviour
{
    private const float HEARTBEAT_DELAY = 15f;
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    public static LobbyManager Instance { get; private set; }

    private float heartbeatTimer;
    private Lobby currentLobby;

    public EventHandler OnTryCreateLobby;
    public EventHandler<LobbyServiceExceptionArgs> OnCreateLobbyFailed;

    public EventHandler OnTryQuickJoinLobby;
    public EventHandler<LobbyServiceExceptionArgs> OnQuickJoinLobbyFailed;

    public EventHandler OnTryRefreshingLobbies;
    public EventHandler OnRefreshingLobbiesSuccess;
    public EventHandler<LobbyServiceExceptionArgs> OnRefreshingLobbiesFailed;

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

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeAuthenticationServices();

        heartbeatTimer = HEARTBEAT_DELAY;
        currentLobby = null;
    }

    private void Update() {
        if (IsLobbyHost()) LobbyHeartBeat();
    }

    private void OnApplicationQuit() {
        if (IsLobbyHost()) {
            DeleteLobby();
            NetworkManager.Singleton.Shutdown();
        }
        else LeaveLobby();
    }

    private async void InitializeAuthenticationServices() {
        if (UnityServices.State == ServicesInitializationState.Initialized) return;

        InitializationOptions options = new InitializationOptions();
        options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private bool LobbyExists() {
        return currentLobby != null;
    }

    private async void LobbyHeartBeat() {
        if (!LobbyExists()) return;

        heartbeatTimer -= Time.deltaTime;

        if (heartbeatTimer > 0) return;

        heartbeatTimer = HEARTBEAT_DELAY;
        await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
    }

    public bool IsLobbyHost() {
        if (!LobbyExists()) return false;
        return currentLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async Task<Allocation> AllocateRelay() {
        try {
            return await RelayService.Instance.CreateAllocationAsync(MultiplayerManager.Instance.GetMaxPlayerCount() - 1);
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation) {
        try {
            return await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode) {
        try {
            return await RelayService.Instance.JoinAllocationAsync(joinCode);
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate) {
        try {
            OnTryCreateLobby?.Invoke(this, EventArgs.Empty);

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            int maxPlayers = MultiplayerManager.Instance.GetMaxPlayerCount();
            CreateLobbyOptions options = new CreateLobbyOptions() {
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject> {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            MultiplayerManager.Instance.StartHost();
            SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyRoomScene);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, new LobbyServiceExceptionArgs {
                lobbyServiceException = e
            });
        }
    }

    public async void QuickJoinLobby() {
        try {
            OnTryQuickJoinLobby?.Invoke(this, EventArgs.Empty);

            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = currentLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnQuickJoinLobbyFailed?.Invoke(this, new LobbyServiceExceptionArgs {
                lobbyServiceException = e
            });
        }
    }

    public async void RefreshLobbies() {
        try {
            OnTryRefreshingLobbies?.Invoke(this, EventArgs.Empty);

            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryRespone = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnRefreshingLobbiesSuccess?.Invoke(this, EventArgs.Empty);
            OnRefreshLobbiesUpdate?.Invoke(this, new RefreshLobbiesUpdateArgs { lobbies = queryRespone.Results });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnRefreshingLobbiesFailed?.Invoke(this, new LobbyServiceExceptionArgs {
                lobbyServiceException = e
            });
        }
    }

    public async void JoinLobbyById(string lobbyId) {
        try {
            OnTryJoinLobbyById?.Invoke(this, EventArgs.Empty);

            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = currentLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinLobbyByIdFailed?.Invoke(this, new LobbyServiceExceptionArgs {
                lobbyServiceException = e
            });
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        try {
            OnTryJoinLobbyByCode?.Invoke(this, EventArgs.Empty);

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = currentLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinLobbyByCodeFailed?.Invoke(this, new LobbyServiceExceptionArgs {
                lobbyServiceException = e
            });
        }
    }

    public async void DeleteLobby() {
        if (!LobbyExists()) return;

        try {
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);

            currentLobby = null;
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby() {
        if (!LobbyExists()) return;

        try {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);

            currentLobby = null;
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void KickPlayer(string playerId) {
        if (!LobbyExists()) return;

        try {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public string GetLobbyName() {
        if (!LobbyExists()) return null;
        return currentLobby.Name;
    }

    public string GetLobbyCode() {
        if (!LobbyExists()) return null;
        return currentLobby.LobbyCode;
    }
}
