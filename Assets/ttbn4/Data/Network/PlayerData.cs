using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public FixedString128Bytes playerId;
    public FixedString128Bytes playerName;

    public PlayerTeam team;

    public bool Equals(PlayerData other) {
        return
            clientId == other.clientId &&
            playerId == other.playerId &&
            playerName == other.playerName &&

            team == other.team;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerName);

        serializer.SerializeValue(ref team);
    }
}
