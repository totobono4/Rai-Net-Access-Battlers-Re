using System;
using Unity.Netcode;

namespace Ttbn4.Network.Data {
    public struct PlayerData<TCustomData> : IEquatable<PlayerData<TCustomData>>, INetworkSerializable where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
        public BasePlayerData basePlayerData;
        public TCustomData customData;

        public bool Equals(PlayerData<TCustomData> other) {
            return
                basePlayerData.Equals(other.basePlayerData) &&
                customData.Equals(other.customData);
        }

        public void NetworkSerialize<TSerializer>(BufferSerializer<TSerializer> serializer) where TSerializer : IReaderWriter {
            basePlayerData.NetworkSerialize(serializer);
            customData.NetworkSerialize(serializer);
        }
    }
}