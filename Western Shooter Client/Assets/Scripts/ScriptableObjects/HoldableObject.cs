using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableObject : ScriptableObject
{
    public new string name;
    public ushort id;

    [Header("Prefabs")]
    public GameObject firstPersonPrefab;
    public string thirdPersonObjectName;
}
