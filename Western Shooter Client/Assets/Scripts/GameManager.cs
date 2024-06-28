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

    [Header("Player Prefabs")]
    [SerializeField] private GameObject localPlayerPrefab;
    [SerializeField] private GameObject remotePlayerPrefab;

    public Player LocalPlayer { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    public void AddPlayer(Player player)
    {
        if (player.IsLocal && LocalPlayer == null)
        {
            LocalPlayer = player;
        }
        players.Add(player.PlayerID, player);
    }

    public void SpawnPlayer(Player player, Vector3 position)
    {
        GameObject playerObject = Instantiate(player.IsLocal ? localPlayerPrefab : remotePlayerPrefab, position, Quaternion.identity);
        player.self = playerObject;
        player.self.name = $"{player.Username} (ID: {player.PlayerID})";

        if (player.IsLocal)
        {
            UIManager.Singleton.DisableConnectScreen();
        }
    }
}
