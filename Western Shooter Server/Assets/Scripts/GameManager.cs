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

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Game Settings")]
    [SerializeField] private float respawnDelay = 5f;

    private ushort currentBotId = 64000;

    private void Awake()
    {
        Singleton = this;
    }

    public void JoinBot(string botName)
    {
        Player bot = new Player(NextBotId(), botName, true);
        AddPlayer(bot);
        SpawnPlayer(bot, new Vector3(0f, 15f, 0f));

        bot.self.AddComponent<BotStateMachine>();
    }

    private ushort NextBotId()
    {
        currentBotId++;
        return (ushort)(currentBotId - 1);
    }

    public void RemoveAllBots()
    {
        List<Player> bots = new List<Player>();
        foreach (Player player in players.Values)
        {
            if (player.IsBot)
            {
                bots.Add(player);
            }
        }

        foreach (Player bot in bots)
        {
            RemoveBot(bot);
        }
    }

    public void RemoveBot(Player bot)
    {
        if (!bot.IsBot) return;

        players.Remove(bot.PlayerID);
        Destroy(bot.self);

        Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.removePlayer);
        message.AddUShort(bot.PlayerID);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public void AddPlayer(Player player)
    {
        players.Add(player.PlayerID, player);
        if (!player.IsBot)
            NetworkManager.Singleton.SyncPlayers(player);

        Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.addPlayer);
        message.AddUShort(player.PlayerID);
        message.AddString(player.Username);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public void OnPlayerDeath(object sender, PlayerDeathEventArgs e)
    {
        StartCoroutine(RespawnPlayer(e.player));
    }

    private IEnumerator RespawnPlayer(Player player)
    {
        yield return new WaitForSeconds(respawnDelay);
        if (player.self == null) yield break;

        player.Respawn(new Vector3(0, 10, 0));
    }

    public void SpawnPlayer(Player player, Vector3 position)
    {
        if (player.self != null) return;

        GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
        player.self = playerObject;
        player.self.name = $"{player.Username} (ID: {player.PlayerID})" + (player.IsBot ? " BOT" : "");
        player.self.GetComponent<PlayerMovement>().player = player;

        player.DeathEvent += OnPlayerDeath;

        Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.spawnPlayer);
        message.AddUShort(player.PlayerID);
        message.AddVector3(position);
        message.AddInt(player.Health);
        NetworkManager.Singleton.Server.SendToAll(message);

        PlayerItemHandler itemHandler = player.self.GetComponent<PlayerItemHandler>();
        itemHandler.player = player;
        if (HoldableObjectManager.Singleton.holdableObjects.TryGetValue(0, out HoldableObject holdable))
        {
            itemHandler.EquipHoldable(holdable);
        }
    }
}
