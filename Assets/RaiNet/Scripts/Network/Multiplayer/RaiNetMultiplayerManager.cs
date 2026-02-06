using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RaiNetMultiplayerManager : MultiplayerManager<RaiNetPlayerData>
{
    [SerializeField] private NetworkList<PlayerData<RaiNetPlayerData>> playerDataNetworkList;
    [SerializeField] private RaiNetMultiplayerConfigSO raiNetMultiplayerConfigSO;

    private List<PlayerTeam> playerTeams;

    protected override MultiplayerConfigSO GetMultiplayerConfig() {
        return raiNetMultiplayerConfigSO;
    }

    protected override void InitializeNetworkList() {
        playerDataNetworkList = new NetworkList<PlayerData<RaiNetPlayerData>>();
    }

    protected override void InitializeCustomData() {
        playerTeams = raiNetMultiplayerConfigSO.GetPlayerTeams();
    }

    protected override void SubNetworkList() {
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    protected override void UnsubNetworkList() {
        playerDataNetworkList.OnListChanged -= PlayerDataNetworkList_OnListChanged;
    }

    protected override void AddPlayerData(PlayerData<RaiNetPlayerData> playerData) {
        PlayerTeam team = playerTeams[GetPlayerCount()];
        playerData.customData.team = team;

        playerDataNetworkList.Add(playerData);
    }

    protected override void RemovePlayerData(PlayerData<RaiNetPlayerData> playerData) {
        playerDataNetworkList.Remove(playerData);
    }

    public override PlayerData<RaiNetPlayerData> GetPlayerDataByIndex(int playerIndex) {
        return playerDataNetworkList[playerIndex];
    }

    protected override void SetPlayerDataByIndex(int playerIndex, PlayerData<RaiNetPlayerData> playerData) {
        playerDataNetworkList[playerIndex] = playerData;
    }

    public override int GetPlayerCount() {
        return playerDataNetworkList.Count;
    }

    public PlayerTeam GetClientTeamById(ulong clientId) {
        return playerDataNetworkList[GetPlayerDataIndexFromClientId(clientId)].customData.team;
    }

    public List<ulong> GetClientIdsByTeam(PlayerTeam team) {
        List<ulong> ids = new List<ulong>();
        foreach (PlayerData<RaiNetPlayerData> playerData in playerDataNetworkList) {
            if (playerData.customData.team == team) {
                ids.Add(playerData.basePlayerData.clientId);
            }
        }
        return ids;
    }
}
