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

    public delegate void PlayerDeathEventHandler(object sender, PlayerDeathEventArgs e);

    public event PlayerDeathEventHandler DeathEvent;

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


        if (Health <= 0)
        {
            Die(damager);
        }
    }

    public void Die(Player killer = null)
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
}
public class PlayerDeathEventArgs : EventArgs
{
    public readonly Player player;

    public PlayerDeathEventArgs(Player player)
    {
        this.player = player;
    }
}
