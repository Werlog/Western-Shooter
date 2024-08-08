using Riptide;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
                SendShootMessage(hitPlayer, hit.point);
            }else
            {
                SendShootMessage(null, hit.point);
            }

            Debug.DrawLine(Look.position, hit.point, Color.red, 3f);
        }
    }

    private void SendShootMessage(Player shotPlayer, Vector3 hitPoint)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.playerShoot);
        message.AddUShort(player.PlayerID);
        message.AddVector3(hitPoint);
        message.AddBool(shotPlayer != null);
        if (shotPlayer != null)
            message.AddUShort(shotPlayer.PlayerID);
        NetworkManager.Singleton.Server.SendToAll(message);
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
