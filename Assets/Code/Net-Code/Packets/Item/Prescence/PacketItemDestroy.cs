﻿namespace Nadis.Net
{
    public struct PacketItemDestroy : IPacketData
    {
        public int PacketID => (int)ServerPacket.DestroyItem;

        public int NetworkID;

        public void Deserialize(PacketBuffer buffer)
        {
            NetworkID = buffer.ReadInt();
        }

        public PacketBuffer Serialize()
        {
            PacketBuffer buffer = new PacketBuffer(PacketID);
            //use buffer.Write(variable); to append data to the packet buffer
            buffer.Write(NetworkID);

            buffer.WriteLength();
            return buffer;
        }
    }
}