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

    [Header("Effect Prefabs")]
    [SerializeField] private GameObject playerHitParticlePrefab;
    [SerializeField] private GameObject playerDeathParticlePrefab;
    [SerializeField] private GameObject playerRagdollPrefab;
    [SerializeField] private GameObject bulletParticlePrefab;

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

        player.SetupOrientation();

        if (player.IsLocal)
        {
            UIManager.Singleton.DisableConnectScreen();
        }
    }

    public void SpawnSpectatorCamera(Vector3 position, Quaternion rotation)
    {
        if (currentSpectatorCamera != null)
        {
            Destroy(currentSpectatorCamera);
        }

        currentSpectatorCamera = Instantiate(spectatorCameraPrefab, position, rotation);
    }

    #region Effect Functions
    public void SpawnRagdoll(Vector3 position, Quaternion rotation, Vector3 initialVelocity)
    {
        GameObject ragdoll = Instantiate(playerRagdollPrefab, position, rotation);
        
        RagdollSimulator simulator = ragdoll.GetComponent<RagdollSimulator>();
        simulator.AddRagdollForce(initialVelocity, true);
    }

    public void SpawnPlayerDeathParticles(Vector3 position)
    {
        Instantiate(playerDeathParticlePrefab, position, Quaternion.identity);
    }

    public void SpawnPlayerHitParticles(Vector3 position)
    {
        Instantiate(playerHitParticlePrefab, position, Quaternion.identity);
    }
    
    public void SpawnBulletParticles(Vector3 position)
    {
        Instantiate(bulletParticlePrefab, position, Quaternion.identity);
    }
    #endregion

    public void DespawnSpectatorCamera()
    {
        if (currentSpectatorCamera != null)
        {
            Destroy(currentSpectatorCamera);
        }
    }
}
