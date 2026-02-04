using System;
using Unity.Netcode;

public struct PlayerData<T> : IEquatable<PlayerData<T>>, INetworkSerializable where T : struct, IEquatable<T>, INetworkSerializable {
    public BasePlayerData basePlayerData;
    public T customData;

    public bool Equals(PlayerData<T> other) {
        return
            basePlayerData.Equals(other.basePlayerData) &&
            customData.Equals(other.customData);
    }

    public void NetworkSerialize<TSerializer>(BufferSerializer<TSerializer> serializer) where TSerializer : IReaderWriter {
        basePlayerData.NetworkSerialize(serializer);
        customData.NetworkSerialize(serializer);
    }
}
