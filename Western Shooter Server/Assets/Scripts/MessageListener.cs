using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageListener : MonoBehaviour
{
    private static MessageListener _singleton;

    public static MessageListener Singleton
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
                Debug.LogWarning($"{nameof(MessageListener)}: Singleton already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    [MessageHandler((ushort)ClientToServer.username)]
    private static void OnReceiveClientUsername(ushort fromClientId, Message message)
    {
        string username = message.GetString();
        Player player = new Player(fromClientId, username);
        GameManager.Singleton.AddPlayer(player);
        GameManager.Singleton.SpawnPlayer(player, new Vector3(0, 15, 0));
        TickManager.Singleton.SendTickSync(player);
    }

    [MessageHandler((ushort)ClientToServer.inputs)]
    private static void OnReceiveClientInputs(ushort fromClientId, Message message)
    {
        if (GameManager.Singleton.players.TryGetValue(fromClientId, out Player player))
        {
            uint requestNumber = message.GetUInt();
            Vector3 rotation = message.GetVector3();
            bool[] inputs = message.GetBools(5);
            PlayerMovement movement = player.self.GetComponent<PlayerMovement>();

            movement.ReceiveInputs(new ClientInputs(inputs, rotation, requestNumber));
        }
    }
}
