using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Riptide.Utils;

public enum ClientToServer : ushort
{
    username = 0,
    inputs = 1,
    useItem = 2,
    chatMessage = 3,
}
public enum ServerToClient : ushort
{
    addPlayer = 0,
    spawnPlayer = 1,
    playerPosition = 2,
    tickSync = 3,
    playerSetHeldObject = 4,
    playerTakeDamage = 5,
    playerDeath = 6,
    playerRespawn = 7,
    playerShoot = 8,
}
public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;

    public static NetworkManager Singleton
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
                Debug.LogWarning($"{nameof(NetworkManager)}: Singleton already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    public Server Server { get; private set; }
    [SerializeField] private ushort port = 2589;
    [SerializeField] private ushort maxClientCount = 5;

    private void Awake()
    {
        Singleton = this;
    }

    private void FixedUpdate()
    {
        Server.Update();
    }

    public void SyncPlayers(Player player)
    {
        foreach (Player p in GameManager.Singleton.players.Values)
        {
            if (p.PlayerID == player.PlayerID) continue;

            Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.addPlayer);
            message.AddUShort(p.PlayerID);
            message.AddString(p.Username);
            Singleton.Server.Send(message, player.PlayerID);

            if (p.self != null)
            {
                Message spawnMessage = Message.Create(MessageSendMode.Reliable, ServerToClient.spawnPlayer);
                spawnMessage.AddUShort(p.PlayerID);
                spawnMessage.AddVector3(p.self.transform.position);
                Singleton.Server.Send(spawnMessage, player.PlayerID);

                PlayerItemHandler itemHandler = p.self.GetComponent<PlayerItemHandler>();
                if (itemHandler)
                {
                    Message heldObjectMessage = Message.Create(MessageSendMode.Reliable, ServerToClient.playerSetHeldObject);
                    heldObjectMessage.AddUShort(p.PlayerID);
                    heldObjectMessage.AddUShort(itemHandler.currentHeldObject != null ? itemHandler.currentHeldObject.holdable.id : ushort.MaxValue);
                    Singleton.Server.Send(heldObjectMessage, player.PlayerID);
                }

                if (!p.IsAlive)
                {
                    Message deathMessage = Message.Create(MessageSendMode.Reliable, ServerToClient.playerDeath);
                    deathMessage.AddUShort(p.PlayerID);
                    deathMessage.AddBool(false);
                    Singleton.Server.Send(deathMessage, player.PlayerID);
                }
            }
        }
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        DontDestroyOnLoad(gameObject);
        Server = new Server();
        Server.ClientDisconnected += OnClientDisconnect;
        Server.Start(port, maxClientCount);
    }

    private void OnApplicationQuit()
    {
        if (Server.IsRunning)
            Server.Stop();
    }
    public void OnClientDisconnect(object sender, ServerDisconnectedEventArgs e)
    {
        if (GameManager.Singleton.players.TryGetValue(e.Client.Id, out Player player))
        {
            if (player.self != null)
            {
                Destroy(player.self);
            }
            GameManager.Singleton.players.Remove(player.PlayerID);
        }
    }
}
