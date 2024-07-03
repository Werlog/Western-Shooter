using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    public Player player;
    [SerializeField] private Transform objectHolder;
    [SerializeField] private Transform cameraLook;
    public HeldObject currentHeldObject;

    public void EquipHoldable(HoldableObject holdable)
    {
        UnEquipHoldable(false);

        currentHeldObject = Instantiate(holdable.playerPrefab, objectHolder).GetComponent<HeldObject>();
        currentHeldObject.holdable = holdable;
        currentHeldObject.Look = cameraLook;
        currentHeldObject.player = player;

        Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.playerSetHeldObject);
        message.AddUShort(player.PlayerID);
        message.AddUShort(holdable.id);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public void UnEquipHoldable(bool sendMessage = true)
    {
        if (currentHeldObject != null) Destroy(currentHeldObject.gameObject);
        currentHeldObject = null;

        if (sendMessage)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.playerSetHeldObject);
            message.AddUShort(player.PlayerID);
            message.AddUShort(ushort.MaxValue);
            NetworkManager.Singleton.Server.SendToAll(message);
        }
    }
}
