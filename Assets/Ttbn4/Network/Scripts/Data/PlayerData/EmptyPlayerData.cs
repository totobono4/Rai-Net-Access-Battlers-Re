using System;
using Unity.Netcode;

namespace Ttbn4.Network.Base {
    public struct EmptyPlayerData : IEquatable<EmptyPlayerData>, INetworkSerializable {
        public bool Equals(EmptyPlayerData other) {
            return true;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            // No data to serialize
        }
    }
}