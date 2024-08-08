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

    public GameObject spectatorCameraPrefab;

    [Header("Player Prefabs")]
    [SerializeField] private GameObject localPlayerPrefab;
    [SerializeField] private GameObject remotePlayerPrefab;

    public Player LocalPlayer { get; private set; }

    private GameObject currentSpectatorCamera;

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

    public void SpawnSpectatorCamera(Vector3 position, Vector3 rotation)
    {
        if (currentSpectatorCamera != null)
        {
            Destroy(currentSpectatorCamera);
        }

        currentSpectatorCamera = Instantiate(spectatorCameraPrefab, position, Quaternion.Euler(rotation));
    }

    public void DespawnSpectatorCamera()
    {
        if (currentSpectatorCamera != null)
        {
            Destroy(currentSpectatorCamera);
        }
    }
}
