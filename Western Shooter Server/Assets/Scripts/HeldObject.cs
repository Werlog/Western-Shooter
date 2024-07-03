using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeldObject : MonoBehaviour
{
    [HideInInspector]
    public HoldableObject holdable;
    [HideInInspector]
    public Transform Look;
    public Player player;

    public abstract void OnAction(HeldObjectAction action);
}
public enum HeldObjectAction : byte
{
    ATTACK = 0,
    START_USE = 1,
    END_USE = 2,
}