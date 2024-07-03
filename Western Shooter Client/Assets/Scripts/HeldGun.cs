using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HeldGun : HeldObject
{
    [SerializeField] private Gun gun;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Animator animator;

    private AudioSource audioSource;
    private bool shootRequest;

    private float sinceShot = 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnAction(HeldObjectAction action)
    {
        if (action == HeldObjectAction.ATTACK)
        {
            shootRequest = true;
        }
    }

    private void FixedUpdate()
    {
        sinceShot += Time.fixedDeltaTime;
        if (shootRequest) Shoot();
    }

    public void Shoot()
    {
        shootRequest = false;
        if (sinceShot < gun.shootDelay) return;

        if (gun.shootSound != null)
        {
            audioSource.clip = gun.shootSound;
            audioSource.Play();
        }
        animator.Play(gun.shootAnimation.name);
        GameObject blastParticle = Instantiate(gun.blastEffect, shootPoint.position, Quaternion.identity);
        blastParticle.transform.SetParent(shootPoint);

        SendShootMessage();
        sinceShot = 0f;
    }

    private void SendShootMessage()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServer.useItem);
        message.AddByte((byte)HeldObjectAction.ATTACK);
        message.AddUInt(TickManager.Singleton.CurrentTick);
        NetworkManager.Singleton.Client.Send(message);
    }
}
