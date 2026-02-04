
using Unity.Netcode;
using UnityEngine;

public class BaseMultiplayerManager : MultiplayerManager<EmptyPlayerData> {

    [SerializeField] private NetworkList<PlayerData<EmptyPlayerData>> playerDataNetworkList;

    protected override void InitializeNetworkList() {
        playerDataNetworkList = new NetworkList<PlayerData<EmptyPlayerData>>();
    }

    protected override void SubNetworkList() {
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    protected override void UnsubNetworkList() {
        playerDataNetworkList.OnListChanged -= PlayerDataNetworkList_OnListChanged;
    }

    protected override void AddPlayerData(PlayerData<EmptyPlayerData> playerData) {
        // ajouter la team dans RaiNet

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
}
