using RaiNet.Data;
using RaiNet.Network.Data;
using System.Collections.Generic;
using Totobono4.Network;
using Totobono4.Network.Data;
using Unity.Netcode;
using UnityEngine;

namespace RaiNet.Network {
    public class RaiNetMultiplayerManager : MultiplayerManager<RaiNetPlayerData> {
        [SerializeField] private NetworkList<PlayerData<RaiNetPlayerData>> playerDataNetworkList;
        [SerializeField] private RaiNetMultiplayerConfigSO raiNetMultiplayerConfigSO;

        private List<PlayerTeam> playerTeams;

        protected override void Awake() {
            base.Awake();

            playerDataNetworkList = new NetworkList<PlayerData<RaiNetPlayerData>>();
            playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

            playerTeams = raiNetMultiplayerConfigSO.GetPlayerTeams();
        }

        protected override MultiplayerManager<RaiNetPlayerData> GetMultiplayerManager() {
            return this;
        }

        protected override MultiplayerConfigSO GetMultiplayerConfig() {
            return raiNetMultiplayerConfigSO;
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

        public override void Clean() {
            playerDataNetworkList.OnListChanged -= PlayerDataNetworkList_OnListChanged;

            base.Clean();
        }
    }
}