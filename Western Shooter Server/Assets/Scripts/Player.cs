using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public const int PlayerMaxHealth = 100;

    public ushort PlayerID { get; private set; }
    public string Username { get; private set; }

    public GameObject self;

    public int Health { get; private set; }
    public bool IsAlive { get; private set; }
    public bool IsBot { get; private set; }

    public int Score { get; private set; }

    public delegate void PlayerDeathEventHandler(object sender, PlayerDeathEventArgs e);

    public static event PlayerDeathEventHandler DeathEvent;

    public delegate void PlayerDamagedEventHandler(object sender, PlayerDamagedEventArgs e);

    public static event PlayerDamagedEventHandler DamagedEvent;

    public Player(ushort playerID, string username, bool isBot)
    {
        PlayerID = playerID;
        Username = username;

        Health = PlayerMaxHealth;
        IsAlive = true;
        IsBot = isBot;
    }

    public void Damage(int amount, Player damager = null)
    {
        if (!IsAlive) return;

        Health -= amount;

        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClient.playerTakeDamage);
        message.AddUShort(PlayerID);
        message.AddInt(Health);
        message.AddBool(damager != null);
        if (damager != null)
        {
            message.AddUShort(damager.PlayerID);
        }
        NetworkManager.Singleton.Server.SendToAll(message);

        DamagedEvent?.Invoke(this, new PlayerDamagedEventArgs(this, damager));

        if (Health <= 0)
        {
            Die(damager);
        }
    }

    public void Die(Player killer = null, bool respawn = true)
    {
        if (!IsAlive) return;

        IsAlive = false;
        self.SetActive(false);

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.playerDeath);
        message.AddUShort(PlayerID);
        message.AddBool(killer != null);
        if (killer != null)
        {
            message.AddUShort(killer.PlayerID);
        }
        NetworkManager.Singleton.Server.SendToAll(message);

        killer?.AddScore(10);

        if (respawn)
            DeathEvent?.Invoke(this, new PlayerDeathEventArgs(this));
    }

    public void Respawn(Vector3 position)
    {
        IsAlive = true;
        self.transform.position = position;
        self.SetActive(true);

        Health = PlayerMaxHealth;

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.playerRespawn);
        message.AddUShort(PlayerID);
        message.AddInt(Health);
        message.AddVector3(position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public void AddScore(int amount)
    {
        Score += amount;

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.playerScore);
        message.AddUShort(PlayerID);
        message.AddInt(Score);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
public class PlayerDeathEventArgs : EventArgs
{
    public readonly Player player;

    public PlayerDeathEventArgs(Player player)
    {
        this.player = player;
    }
}
public class PlayerDamagedEventArgs : EventArgs
{
    public readonly Player damagedPlayer;
    public readonly Player attacker;

    public PlayerDamagedEventArgs(Player damagedPlayer, Player attacker)
    {
        this.damagedPlayer = damagedPlayer;
        this.attacker = attacker;
    }
}