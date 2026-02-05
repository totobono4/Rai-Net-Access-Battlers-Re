using System;
using Unity.Collections;
using Unity.Netcode;

public struct BasePlayerData : IEquatable<BasePlayerData>, INetworkSerializable
{
    public ulong clientId;
    public FixedString128Bytes playerId;
    public FixedString128Bytes playerName;

    public bool Equals(BasePlayerData other) {
        return
            clientId == other.clientId &&
            playerId == other.playerId &&
            playerName == other.playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerName);
    }
}
