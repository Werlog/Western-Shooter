using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableObjectManager : MonoBehaviour
{
    private static HoldableObjectManager _singleton;

    public static HoldableObjectManager Singleton
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
                Debug.LogWarning($"{nameof(HoldableObjectManager)}: Singleton already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    public Dictionary<ushort, HoldableObject> holdableObjects;

    private void Awake()
    {
        holdableObjects = new Dictionary<ushort, HoldableObject>();
        Singleton = this;
    }

    private void Start()
    {
        ReloadHoldableObjects();
    }

    public void ReloadHoldableObjects()
    {
        HoldableObject[] objects = Resources.LoadAll<HoldableObject>("HoldableObjects/");
        foreach (HoldableObject obj in objects)
        {
            holdableObjects.Add(obj.id, obj);
        }
    }
}
