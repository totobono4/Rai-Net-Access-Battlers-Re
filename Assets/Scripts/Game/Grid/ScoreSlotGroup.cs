using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreSlotGroup : TileMap
{
    List<OnlineCard> onlineCards = new List<OnlineCard>();

    public void AddOnlineCard(OnlineCard onlineCard) {
        int width = GetWidth();

        onlineCard.SetTileParent(GetTile(onlineCards.Count % width, onlineCards.Count / width));
        onlineCards.Add(onlineCard);

        AddOnlineCardClientRpc(onlineCard.GetComponent<NetworkObject>());
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void AddOnlineCardClientRpc(NetworkObjectReference onlineCardNetworkReference) {
        if (!onlineCardNetworkReference.TryGet(out NetworkObject onlineCardNetwork)) return;
        OnlineCard onlineCard = onlineCardNetwork.GetComponent<OnlineCard>();
        onlineCard.SyncCardParent();
    }
}
