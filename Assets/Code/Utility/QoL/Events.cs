﻿using System;
using UnityEngine;

public enum ItemEventType
{
    Interact = 1,
    Reset = 2,
    Hide = 3,
    Transform = 4,
    Spawn = 5,
    Destroy = 6,
}
public enum InventoryEventType
{
    AddItem = 1,
    RemoveItem = 2,
}
public enum PlayerStatsEventType
{
    AlterHealth = 1,
    AlterPower,
    Die
}

public static class Events
{
    public static InventoryEvents Inventory;
    public static ItemEvents Item;
    public static BipedAnimatorEvents BipedAnimator;
    public static PlayerEvents Player;
    public static PlayerStatEvents PlayerStats;
    public static MapGeneratorEvents MapGenerator;
    public static Notifications Notification;
    public static Networking Net;
    public static AdministratorEvents admin;
}

public struct AdministratorEvents
{
    public delegate void SwitchActivePlayer(int fromID, int toID);
    public SwitchActivePlayer OnSwitchActivePlayer;
}

public struct InventoryEvents
{
    public Action<int> OnInventoryCreated;
    public Action<Item, int> OnItemAddedToInventory;
    public Action<int> OnItemRemovedFromInventory;
}
public struct ItemEvents
{
    
}
public struct BipedAnimatorEvents
{
    public Action<int> OnRightFootBeginStep;
    public Action<int> OnLeftFootBeginStep;

    public Action<int> OnRightFootStepping;
    public Action<int> OnLeftFootStepping;

    public Action<int> OnRightFootFinishStep;
    public Action<int> OnLeftFootFinishStep;

    public Action<int, Vector3, Side, float, AnimatorTarget, bool> SetHandTargetPosition;
    public Action<int> EndCurrentHandTarget;

    public Action<int, bool, float, float, int> OnAnimatorDataSet;

    public Action<int, string, Action> ExecuteAnimation;
    public Action<int, string> EndAnimation;
}
public struct PlayerEvents
{
    public delegate void OnGetPlayerAnimator(int playerID, ref BipedProceduralAnimator animator);
    public delegate void OnGetPlayer(int playerID, ref NetworkedPlayer netPlayer);
    public OnGetPlayerAnimator GetPlayerAnimator;
    public OnGetPlayer GetPlayer;
    public Action<int> UnSubscribe;
}
public struct PlayerStatEvents
{
    public delegate void StatEvent(int playerID, int amount, bool send);
    public delegate void OnStatEvent(int playerID, float percent, bool send);
    public StatEvent Heal;
    public StatEvent Damage;
    public OnStatEvent SetHealth;
    public StatEvent AlterPower;
    public OnStatEvent SetPower;
    public Action<int> Die;

    public OnStatEvent OnAlterHealth;
    public OnStatEvent OnAlterPower;

    public Action<PlayerStats> SetDefaults;
}
public struct MapGeneratorEvents
{
    public Action<Vector3> RegisterSpawnPoint;
    public Action<int> GenerateMap;

    public delegate Vector3 GetMapPosition(float minH, float maxH, float maxAng);
    //public delegate SpawnPoint GetChargePoint(int index);
    //public delegate SpawnPoint[] GetChargePoints();
    //public GetMapPosition GetPosition;
    //public GetChargePoint GetChargePosition;

    public Action<float, float, float, int, int> CreateChargePoints;
    //public GetChargePoints GetChargePositions;
    public Action PlaceChargingStations;

}
public struct Notifications
{
    public delegate void NewNotification(NotificationType type, bool persistent = false);
    public NewNotification New;
    public Action<NotificationType> Remove;
}

public struct Networking
{
    public delegate void ClientDataCallback(int clientToSendAs, IPacketData packet);
    public delegate void ClientDisconnect();

    public ClientDataCallback SendAsClient;
    public ClientDataCallback SendAsClientUnreliable;
    public ClientDisconnect DisconnectClient;

}
