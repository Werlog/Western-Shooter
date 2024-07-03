using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeldObject : MonoBehaviour
{
    public Transform cameraLook;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnAction(HeldObjectAction.ATTACK);
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnAction(HeldObjectAction.START_USE);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            OnAction(HeldObjectAction.END_USE);
        }
    }
    public abstract void OnAction(HeldObjectAction action);
}
public enum HeldObjectAction : byte
{
    ATTACK = 0,
    START_USE = 1,
    END_USE = 2,
}