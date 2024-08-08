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
                Player hitPlayer = hit.collider.GetComponent<PlayerMovement>().player;

                float distance = Vector3.Distance(Look.position, hit.point);

                hitPlayer.Damage(GetDamage(distance), player);
            }

            Debug.DrawLine(Look.position, hit.point, Color.red, 5f);
        }
    }

    private int GetDamage(float distance)
    {
        for (int i = 0; i < gun.damages.Count; i++)
        {
            DistanceDamage damage = gun.damages[i];
            if (distance > damage.maxDistance) continue;

            return damage.bodyDamage;
        }

        return gun.damages[gun.damages.Count - 1].bodyDamage;
    }
}
