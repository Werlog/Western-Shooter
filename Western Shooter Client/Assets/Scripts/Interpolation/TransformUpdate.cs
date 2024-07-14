using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpdate
{
    public readonly uint Tick;
    public readonly Vector3 Position;
    public readonly LowerAnimation Animation;

    public TransformUpdate(uint tick, Vector3 position, LowerAnimation animation)
    {
        Tick = tick;
        Position = position;
        Animation = animation;
    }
}
