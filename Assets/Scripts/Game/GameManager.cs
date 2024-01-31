using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameConfigSO gameConfigSO;

    private Transform playerPrefab;
    private List<Team> playOrder;

    private int playCursor;
    private Team teamPriority;

    public EventHandler<PlayerGivePriorityArgs> OnPlayerGivePriority;
    public class PlayerGivePriorityArgs : EventArgs {
        public Team team;
        public int actionTokens;
    }

    private void Awake() {
        Instance = this;

        playerPrefab = gameConfigSO.GetPlayerPrefab();
        playOrder = gameConfigSO.GetPlayOrder();
        playCursor = 0;
        teamPriority = Team.None;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }

        GameBoard.Instance.Initialize();

        PassPriority(0);
    }

    private void NetworkManager_OnClientDisconnect(ulong clientId) {
        throw new NotImplementedException();
    }

    public void PassPriority(int actionTokens) {
        int playerActionTokens = actionTokens;

        if (actionTokens <= 0) {
            teamPriority = playOrder[playCursor % playOrder.Count];
            playerActionTokens = gameConfigSO.GetActionTokens();
            playCursor++;
        }

        OnPlayerGivePriority?.Invoke(this, new PlayerGivePriorityArgs { team = teamPriority, actionTokens = playerActionTokens });
    }

    public bool TryGetClientIdByTeam(Team team, out ulong clientId) {
        return MultiplayerManager.Instance.TryGetClientIdByTeam(team, out clientId);
    }
}
