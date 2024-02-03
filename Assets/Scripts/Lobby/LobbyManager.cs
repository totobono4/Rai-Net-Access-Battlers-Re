using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private const float HEARTBEAT_DELAY = 15f;

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

    public async void CreateLobby(string lobbyName, bool isPrivate) {
        try {
            OnTryCreateLobby?.Invoke(this, EventArgs.Empty);

            int maxPlayers = MultiplayerManager.Instance.GetMaxPlayerCount();
            CreateLobbyOptions options = new CreateLobbyOptions() {
                IsPrivate = isPrivate,
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

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

            QueryResponse queryRespone = await LobbyService.Instance.QueryLobbiesAsync();

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
