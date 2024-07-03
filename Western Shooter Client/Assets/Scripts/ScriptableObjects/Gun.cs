using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Holdable Objects/Gun")]
public class Gun : HoldableObject
{
    [Header("Stats")]
    public float shootDelay = 0.5f;

    [Header("Visuals")]
    public GameObject blastEffect;
    public bool bulletTracers;
    public GameObject bulletTracerPrefab;

    [Header("Animations")]
    public AnimationClip idleAnimation;
    public AnimationClip shootAnimation;

    [Header("Sounds")]
    public AudioClip shootSound;
}
[Serializable]
public struct DistanceDamage
{
    public float maxDistance;
    public int bodyDamage;
    public int headshotDamage;
}