using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Holdable Objects/Gun")]
public class Gun : HoldableObject
{
    [Header("Stats")]
    public List<DistanceDamage> damages;
    public float shootDelay = 0.5f;
    public float spread;
    public int bulletCount;
    public float maxMovementError;
}
[Serializable]
public struct DistanceDamage
{
    public float maxDistance;
    public int bodyDamage;
    public int headshotDamage;
}
