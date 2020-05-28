﻿using System.Collections.Generic;
using System.Diagnostics;

namespace Nadis.Net.Client
{
    public static class ClientPacketHandler
    {
        private static Dictionary<int, PacketHandlerData> handlers;

        public static void Initialize()
        {
            handlers = new Dictionary<int, PacketHandlerData>();
            PopulateHandlers();
        }
        public static void Handle(int packetID, PacketBuffer buffer)
        {
            if(handlers.ContainsKey(packetID) == false)
            {
                UnityEngine.Debug.LogErrorFormat("Failed to Handle Packet With ID of '{0}', No Handler Exists.", packetID);
                return;
            }
            
            handlers[packetID].Invoke(buffer);
        }
        /*
        public static void Handle(int packetID, JobPacketBuffer buffer)
        {
            if (handlers.ContainsKey(packetID) == false)
            {
                UnityEngine.Debug.LogErrorFormat("Failed to Handle Packet With ID of '{0}', No Handler Exists.", packetID);
                return;
            }

            handlers[packetID].Invoke(buffer);
        }
        */
        public static void SubscribeTo(int packetID, PacketHandlerData.ReceiveCallback callback)
        {
            if (handlers.ContainsKey(packetID) == false) return;

            handlers[packetID].Subscribe(callback);
        }
        public static void UnSubscribeFrom(int packetID, PacketHandlerData.ReceiveCallback callback)
        {
            if (handlers.ContainsKey(packetID) == false) return;

            handlers[packetID].UnSubscribe(callback);
        }

        //The Created Handlers should only utilize SharedPacketID & ServerPacketID
        //You should never need to use ClientPacketID here.
        private static void PopulateHandlers()
        {
            CreateHandler((int)ServerPacket.PlayerConnection, new PacketPlayerConnection(), (IPacketData packet) => {
                PacketPlayerConnection data = (PacketPlayerConnection)packet;
                
                PlayerPopulatorSystem.SpawnPlayer(data);
                if(data.playerIsLocal)
                {
                    Log.Txt("CLIENT :: Attempting To Establish UDP Connection");
                    Client.Local.UDP.Connect(Client.Local.TCP.LocalPort, data.playerID);
                }
            });

            CreateHandler((int)ServerPacket.PlayerUDPConnected, new PacketUDPConnected(), (IPacketData packet) =>
            {
                Client.Local.UDP.connected = true;
            });

            CreateHandler((int)SharedPacket.PlayerPosition, new PacketPlayerPosition(), null);
            CreateHandler((int)SharedPacket.PlayerRotation, new PacketPlayerRotation(), null);
            CreateHandler((int)SharedPacket.PlayerDisconnected, new PacketDisconnectPlayer(), null);
            CreateHandler((int)ServerPacket.PlayerInventoryData, new PacketPlayerInventoryData(), (IPacketData packet) =>
            {
                PacketPlayerInventoryData data = (PacketPlayerInventoryData)packet;
                Inventory.CreateInventory(data.playerID, data.size);
            });

            CreateHandler((int)ServerPacket.SpawnItem, new PacketItemSpawn(), (IPacketData packet) =>
            {
                Inventory.instance.ServerSpawnItem(packet);
            });
            CreateHandler((int)SharedPacket.ItemPosition, new PacketItemPosition(), null);
            CreateHandler((int)SharedPacket.ItemPickup, new PacketItemPickup(), (IPacketData packet) =>
            {
                PacketItemPickup data = (PacketItemPickup)packet;
                Inventory.MoveItemToInventory(data.NetworkID, data.PlayerID);
            });
            CreateHandler((int)ServerPacket.DestroyItem, new PacketItemDestroy(), (IPacketData packet) => 
            {
                Inventory.instance.ServerDestroyItem(packet);
            });
            CreateHandler((int)SharedPacket.ItemVisibility, new PacketItemVisibility(), (IPacketData packet) =>
            {
                Inventory.instance.ServerHideItem(packet);
            });
            CreateHandler((int)SharedPacket.ItemDrop, new PacketItemDrop(), (IPacketData packet) =>
            {
                Inventory.instance.ServerDropItem(packet);
            });

            CreateHandler((int)ServerPacket.DamagePlayer, new PacketAlterPlayerHealth(), (IPacketData packet) =>
            {
                PlayerManager.DamagePlayer((PacketAlterPlayerHealth)packet);
            });
            CreateHandler((int)SharedPacket.PlayerAnimatorMoveData, new PacketPlayerAnimatorMoveData(), null);
            CreateHandler((int)SharedPacket.PlayerAnimatorEventData, new PacketPlayerAnimatorEvent(), null);

            CreateHandler((int)ServerPacket.KillPlayer, new PacketKillPlayer(), (IPacketData packet) => 
            {
                PlayerManager.KillPlayer((PacketKillPlayer)packet);
            });

            CreateHandler((int)ServerPacket.AlterPowerLevel, new PacketAlterPlayerPower(), (IPacketData packet) => 
            {
                PlayerManager.AlterPlayerPowerLevel((PacketAlterPlayerPower)packet);
            });

            CreateHandler((int)ServerPacket.UnitData, new PacketUnitData(), (IPacketData packet) =>
            {
                Log.Txt("Spawn Unit");
                PlayerPopulatorSystem.SpawnUnit((PacketUnitData)packet);
            });

            CreateHandler((int)ServerPacket.UnitPosition, new PacketUnitPosition(), null);
            CreateHandler((int)ServerPacket.UnitRotation, new PacketUnitRotation(), null);
            CreateHandler((int)ServerPacket.UnitAnimationState, new PacketUnitAnimationState(), null);
        }

        private static void CreateHandler(int packetID, IPacketData packetType,
            PacketHandlerData.ReceiveCallback callback = null)
        {
            PacketHandlerData handler = new PacketHandlerData(packetType, callback);
            handlers.Add(packetID, handler);
        }


    }
}
