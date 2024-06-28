using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpdate
{
    public readonly uint Tick;
    public readonly Vector3 Position;

    public TransformUpdate(uint tick, Vector3 position)
    {
        Tick = tick;
        Position = position;
    }
}
