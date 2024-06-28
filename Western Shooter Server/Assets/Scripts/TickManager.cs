using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    private static TickManager _singleton;

    public static TickManager Singleton
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
                Debug.LogWarning($"{nameof(TickManager)}: Singleton already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    [SerializeField] private int tickRate = 60;
    public float TimeBetweenTicks { get => 1f / tickRate; }
    public event EventHandler<TickEventArgs> TickEventHandler;
    public uint CurrentTick { get; private set; }

    private float sinceTick = 0f;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        sinceTick += Time.deltaTime;
        if (sinceTick >= TimeBetweenTicks)
        {
            CurrentTick++;

            TickEventHandler?.Invoke(this, new TickEventArgs(CurrentTick));
            sinceTick -= TimeBetweenTicks;

            if (CurrentTick % 200 == 0)
            {
                SendTickSync();
            }
        }
    }

    public void SendTickSync(Player player)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClient.tickSync);
        message.AddUInt(CurrentTick);
        NetworkManager.Singleton.Server.Send(message, player.PlayerID);
    }

    public void SendTickSync()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClient.tickSync);
        message.AddUInt(CurrentTick);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
public class TickEventArgs : EventArgs
{
    public readonly uint Tick;
    public TickEventArgs(uint tick)
    {
        Tick = tick;
    }
}
