using Ttbn4.Network.Data;
using Unity.Netcode;
using UnityEngine;

namespace Ttbn4.Network.Base {
    public class BaseMultiplayerManager : MultiplayerManager<EmptyPlayerData> {
        [SerializeField] private NetworkList<PlayerData<EmptyPlayerData>> playerDataNetworkList;
        [SerializeField] private MultiplayerConfigSO baseMultiplayerConfigSO;

        protected override void Awake() {
            base.Awake();

            playerDataNetworkList = new NetworkList<PlayerData<EmptyPlayerData>>();
            playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        }

        protected override MultiplayerManager<EmptyPlayerData> GetMultiplayerManager() {
            return this;
        }

        protected override MultiplayerConfigSO GetMultiplayerConfig() {
            return baseMultiplayerConfigSO;
        }

        protected override void AddPlayerData(PlayerData<EmptyPlayerData> playerData) {
            playerDataNetworkList.Add(playerData);
        }

        protected override void RemovePlayerData(PlayerData<EmptyPlayerData> playerData) {
            playerDataNetworkList.Remove(playerData);
        }

        public override PlayerData<EmptyPlayerData> GetPlayerDataByIndex(int playerIndex) {
            return playerDataNetworkList[playerIndex];
        }

        protected override void SetPlayerDataByIndex(int playerIndex, PlayerData<EmptyPlayerData> playerData) {
            playerDataNetworkList[playerIndex] = playerData;
        }

        public override int GetPlayerCount() {
            return playerDataNetworkList.Count;
        }

        public override void Clean() {
            playerDataNetworkList.OnListChanged -= PlayerDataNetworkList_OnListChanged;

            base.Clean();
        }
    }
}