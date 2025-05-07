using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public PlayerTeam team;

    public FixedString128Bytes playerId;
    public FixedString128Bytes playerName;

    public bool Equals(PlayerData other) {
        return
            clientId == other.clientId &&
            team == other.team &&
            playerId == other.playerId &&
            playerName == other.playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref team);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerName);
    }
}
