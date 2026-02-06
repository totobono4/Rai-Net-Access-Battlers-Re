using System;
using Unity.Netcode;

public struct RaiNetPlayerData : IEquatable<RaiNetPlayerData>, INetworkSerializable {
    public PlayerTeam team;

    public bool Equals(RaiNetPlayerData other) {
        return team == other.team;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref team);
    }
}
