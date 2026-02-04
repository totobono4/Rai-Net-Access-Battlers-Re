using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameConfigSO gameConfigSO;

    private Transform playerPrefab;
    private List<PlayerTeam> playerTeams;
    private List<PlayerTeam> playOrder;

    private int playCursor;
    private PlayerTeam teamPriority;

    public EventHandler<PlayerGivePriorityArgs> OnPlayerGivePriority;
    public class PlayerGivePriorityArgs : EventArgs {
        public PlayerTeam team;
        public int actionTokens;
    }

    public EventHandler<GameOverArgs> OnGameOver;
    public class GameOverArgs : EventArgs {
        public PlayerTeam team;
        public bool hasWon;
    }

    private void Awake() {
        Instance = this;

        playerPrefab = gameConfigSO.GetPlayerPrefab();
        playerTeams = gameConfigSO.GetPlayerTeams();
        playOrder = gameConfigSO.GetPlayOrder();
        playCursor = 0;
        teamPriority = PlayerTeam.None;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (!IsServer) return;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }

        GameBoard.Instance.Initialize();
    }

    private bool IsGameOver(out PlayerTeam team, out bool hasWon) {
        team = default;
        hasWon = default;

        foreach (PlayerEntity playerEntity in GameBoard.Instance.GetPlayerEntities()) {
            if (!playerEntity.GetWin(out bool winState)) continue;

            team = playerEntity.GetTeam();
            hasWon = winState;
            return true;
        }

        return false;
    }

    public void PassPriority(int actionTokens) {
        int playerActionTokens = actionTokens;

        if (actionTokens <= 0) {
            teamPriority = playOrder[playCursor % playOrder.Count];
            playerActionTokens = gameConfigSO.GetActionTokens();
            playCursor++;
        }

        if (!IsGameOver(out PlayerTeam team, out bool hasWon)) {
            OnPlayerGivePriority?.Invoke(this, new PlayerGivePriorityArgs {
                team = teamPriority,
                actionTokens = playerActionTokens
            });
            return;
        }

        OnGameOver?.Invoke(this, new GameOverArgs {
            team = team,
            hasWon = hasWon
        });
    }

    /*
    public List<ulong> GetClientIdsByTeam(PlayerTeam team) {
        return MultiplayerManager.Instance.GetClientIdsByTeam(team);
    }
    */

    public void Clean() {
        GameBoard.Instance.Clean();
        // MultiplayerManager.Instance.Clean();

        Destroy(gameObject);
    }
}
