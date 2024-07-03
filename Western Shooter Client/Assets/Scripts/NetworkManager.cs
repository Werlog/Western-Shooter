using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Riptide.Utils;
using Riptide.Transports;
using System;

public enum ClientToServer : ushort
{
    username = 0,
    inputs = 2,
    useItem = 3,
}
public enum ServerToClient : ushort
{
    addPlayer = 0,
    spawnPlayer = 1,
    playerPosition = 2,
    tickSync = 3,
    playerSetHeldObject = 4,
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

    public Client Client { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    private void FixedUpdate()
    {
        Client.Update();
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false); ;
        DontDestroyOnLoad(gameObject);
        Client = new Client();
        Client.Disconnected += OnDisconnect;
        Client.Connected += OnConnected;
        Client.ClientDisconnected += OnClientDisconnect;
        Client.ConnectionFailed += OnConnectionFailed;
    }

    public void Connect(string ip, ushort port)
    {
        Client.Connect($"{ip}:{port}");
    }

    private void OnApplicationQuit()
    {
        if (Client.IsConnected)
            Client.Disconnect();
    }

    public void OnConnected(object sender, EventArgs e)
    {
        UIManager.Singleton.SendUsername();
    }

    public void OnDisconnect(object sender, Riptide.DisconnectedEventArgs e)
    {
        foreach (Player p in GameManager.Singleton.players.Values)
        {
            if (p.self == null) continue;
            Destroy(p.self);
        }
        GameManager.Singleton.players.Clear();
        UIManager.Singleton.BackToConnectScreen();
    }

    public void OnClientDisconnect(object sender, ClientDisconnectedEventArgs e)
    {
        if (GameManager.Singleton.players.TryGetValue(e.Id, out Player player))
        {
            if (player.self != null) Destroy(player.self);

            GameManager.Singleton.players.Remove(player.PlayerID);
        }
    }

    public void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
    {
        UIManager.Singleton.BackToConnectScreen();
    }
}
