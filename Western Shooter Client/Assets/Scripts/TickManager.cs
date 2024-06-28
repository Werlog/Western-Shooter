using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
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
            }else
            {
                Debug.LogWarning($"{nameof(TickManager)}: Singleton already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    [SerializeField] private int tickRate = 60;
    [SerializeField] private int tickDiffTolerence = 2;
    public readonly uint TicksBetweenPositionUpdates = 2;
    public float TimeBetweenPositionUpdates { get => 1f / (tickRate * TicksBetweenPositionUpdates); }

    public float TimeBetweenTicks { get => 1f / tickRate; }
    public event EventHandler<TickEventArgs> TickEventHandler;
    public uint CurrentTick { get; private set; }

    public uint InterpolationTick { get => CurrentTick - TicksBetweenPositionUpdates; }

    private float sinceTick = 0f;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        CurrentTick = 2;
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
        }
    }
    public void OnReceiveTickSync(uint ServerTick)
    {
        if (Mathf.Abs((int)CurrentTick - (int)ServerTick) > tickDiffTolerence)
        {
            Debug.Log($"Tick Correction {CurrentTick} -> {ServerTick}");
            CurrentTick = ServerTick;
        }
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
