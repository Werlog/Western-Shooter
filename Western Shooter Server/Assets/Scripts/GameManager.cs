using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _singleton;

    public static GameManager Singleton
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
                Debug.LogWarning($"{nameof(GameManager)}: Singleton already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    public Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();

    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        Singleton = this;
    }

    public void AddPlayer(Player player)
    {
        players.Add(player.PlayerID, player);
        NetworkManager.Singleton.SyncPlayers(player);

        Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.addPlayer);
        message.AddUShort(player.PlayerID);
        message.AddString(player.Username);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public void SpawnPlayer(Player player, Vector3 position)
    {
        if (player.self != null) return;

        GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
        player.self = playerObject;
        player.self.name = $"{player.Username} (ID: {player.PlayerID})";
        player.self.GetComponent<PlayerMovement>().player = player;

        Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.spawnPlayer);
        message.AddUShort(player.PlayerID);
        message.AddVector3(position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
