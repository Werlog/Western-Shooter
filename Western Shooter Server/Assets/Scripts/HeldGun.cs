using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldGun : HeldObject
{
    [SerializeField] private Gun gun;

    private float sinceShot;

    private void Start()
    {
        sinceShot = gun.shootDelay;
    }

    public override void OnAction(HeldObjectAction action)
    {
        if (action == HeldObjectAction.ATTACK) Shoot();
    }

    public void Shoot()
    {
        if (sinceShot < gun.shootDelay) return;

        if (Physics.Raycast(Look.position, Look.forward, out RaycastHit hit, 200f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("A player has been shot");
            }
        }
    }
}
