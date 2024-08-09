using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

public class MessageListener : MonoBehaviour
{
    private static MessageListener _singleton;

    public static MessageListener Singleton
    {
        get => _singleton;

        set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else
            {
                Debug.LogWarning($"{nameof(MessageListener)}: Singleton already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    [MessageHandler((ushort)ServerToClient.addPlayer)]
    private static void OnReceivePlayerAdd(Message message)
    {
        ushort id = message.GetUShort();
        string username = message.GetString();

        Player player = new Player(id, username, NetworkManager.Singleton.Client.Id == id);

        GameManager.Singleton.AddPlayer(player);
    }

    [MessageHandler((ushort)ServerToClient.spawnPlayer)]
    private static void OnReceivePlayerSpawn(Message message)
    {
        ushort playerId = message.GetUShort();
        Vector3 position = message.GetVector3();

        if (GameManager.Singleton.players.TryGetValue(playerId, out Player player))
        {
            GameManager.Singleton.SpawnPlayer(player, position);
        }
    }

    [MessageHandler((ushort)ServerToClient.playerPosition)]
    private static void OnReceivePlayerPosition(Message message)
    {
        ushort playerId = message.GetUShort();

        if (GameManager.Singleton.players.TryGetValue(playerId, out Player player))
        {
            uint requestNumber = message.GetUInt();
            uint tick = message.GetUInt();
            Vector3 position = message.GetVector3();
            Vector3 rotation = message.GetVector3();
            Vector3 velocity = message.GetVector3();
            LowerAnimation animation = (LowerAnimation)message.GetByte();

            if (!player.IsLocal)
            {
                Interpolator interpolator = player.self.GetComponent<Interpolator>();

                player.self.transform.eulerAngles = rotation;
                interpolator.NewUpdate(new TransformUpdate(tick, position, animation));
            }else
            {
                PlayerMovement movement = player.self.GetComponent<PlayerMovement>();
                movement.CheckPrediction(requestNumber, position, velocity);
            }
        }
    }

    [MessageHandler((ushort)ServerToClient.tickSync)]
    private static void OnReceiveTickSync(Message message)
    {
        uint tick = message.GetUInt();
        TickManager.Singleton.OnReceiveTickSync(tick);
    }

    [MessageHandler((ushort)ServerToClient.playerSetHeldObject)]
    private static void OnReceiveSetHeldObject(Message message)
    {
        ushort playerId = message.GetUShort();
        ushort objectId = message.GetUShort();

        if (GameManager.Singleton.players.TryGetValue(playerId, out Player player))
        {
            if (player.IsLocal && HoldableObjectManager.Singleton.holdableObjects.TryGetValue(objectId, out HoldableObject holdable))
            {
                PlayerItemHandler itemHandler = player.self.GetComponent<PlayerItemHandler>();
                if (itemHandler != null)
                {
                    itemHandler.EquipHoldable(holdable);
                }
            }else if (!player.IsLocal)
            {
                // TODO: Implement for remote players
            }
        }
    }

    [MessageHandler((ushort)ServerToClient.playerTakeDamage)]
    private static void OnReceivePlayerTakeDamage(Message message)
    {
        ushort playerId = message.GetUShort();
        
        if (GameManager.Singleton.players.TryGetValue(playerId, out Player player))
        {
            int health = message.GetInt();
            bool hasDamager = message.GetBool();

            #region Damager
            if (hasDamager)
            {
                ushort damagerId = message.GetUShort();
                if (GameManager.Singleton.players.TryGetValue(damagerId, out Player damager))
                {
                    if (damager.IsLocal)
                    {
                        // TODO: Add hitmarkers and other shit
                    }
                }
            }
            #endregion

            player.Health = health;
        }
    }

    [MessageHandler((ushort)ServerToClient.playerDeath)]
    private static void OnReceivePlayerDeath(Message message)
    {
        ushort playerId = message.GetUShort();
        bool hasKiller = message.GetBool();
        if (GameManager.Singleton.players.TryGetValue(playerId, out Player killed))
        {
            if (hasKiller && GameManager.Singleton.players.TryGetValue(message.GetUShort(), out Player killer))
            {
                killed.Die(killer);
                return;
            }

            killed.Die();
        }
    }

    [MessageHandler((ushort)ServerToClient.playerRespawn)]
    private static void OnReceivePlayerRespawn(Message message)
    {
        ushort playerId = message.GetUShort();
        int health = message.GetInt();
        Vector3 position = message.GetVector3();


        if (GameManager.Singleton.players.TryGetValue(playerId, out Player player))
        {
            player.Respawn(position, health);
        }
    }

    [MessageHandler((ushort)ServerToClient.playerShoot)]
    private static void OnReceivePlayerShoot(Message message)
    {
        ushort playerId = message.GetUShort();
        Vector3 hitPosition = message.GetVector3();
        bool didHit = message.GetBool();

        if (GameManager.Singleton.players.TryGetValue(playerId, out Player player))
        {
            if (!didHit)
            {
                GameManager.Singleton.SpawnBulletParticles(hitPosition);
            }else
            {
                GameManager.Singleton.SpawnPlayerHitParticles(hitPosition);
            }

            // TODO: Hit screen for local player and shooting animation for remote players
        }
    }

    [MessageHandler((ushort)ServerToClient.removePlayer)]
    private static void OnReceiveRemovePlayer(Message message)
    {
        ushort playerId = message.GetUShort();

        if (GameManager.Singleton.players.TryGetValue(playerId, out Player player))
        {
            if (player.IsLocal)
            {
                NetworkManager.Singleton.Client.Disconnect();
            }

            if (player.self != null) Destroy(player.self);

            GameManager.Singleton.players.Remove(playerId);
        }
    }
}
