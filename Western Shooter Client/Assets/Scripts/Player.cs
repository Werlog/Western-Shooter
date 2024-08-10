using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public const int PlayerMaxHealth = 100;

    public ushort PlayerID { get; private set; }
    public string Username { get; private set; }
    public bool IsLocal { get; private set; }
    public int Score { get; private set; }

    private int _health;

    public int Health
    {
        get => _health;
        set
        {
            if (_health > value && IsLocal)
            {
                int diff = _health - value;
                UIManager.Singleton.PlayHurtAnimation(0.6f * (diff / (float)50));
            }
            _health = value;
            if (IsLocal)
            {
                UIManager.Singleton.SetHealthText(_health);
            }
        }
    }
    public bool IsAlive { get; private set; }

    public GameObject self;

    private Transform orientation;

    public Player(ushort playerID, string username, bool isLocal)
    {
        PlayerID = playerID;
        Username = username;
        IsLocal = isLocal;

        Health = PlayerMaxHealth;
    }

    public void Die(Player killer = null)
    {
        if (self != null) self.SetActive(false);
        IsAlive = false;

        if (IsLocal)
        {
            GameManager.Singleton.SpawnSpectatorCamera(self.transform.position + Vector3.up * 0.7f, orientation.rotation);
        }

        Vector3 direction = Vector3.down * 2;
        if (killer != null)
        {
            direction = (self.transform.position - killer.self.transform.position).normalized * 4f;
            direction.y = 3f;
        }

        GameManager.Singleton.SpawnRagdoll(self.transform.position, orientation.rotation, direction);
        GameManager.Singleton.SpawnPlayerDeathParticles(self.transform.position);
    }

    public void Respawn(Vector3 position, int health)
    {
        IsAlive = true;
        Health = health;

        self.SetActive(true);
        self.transform.position = position;

        if (IsLocal)
            GameManager.Singleton.DespawnSpectatorCamera();
    }

    public void SetupOrientation()
    {
        if (self == null) return;

        if (IsLocal)
        {
            orientation = self.GetComponent<PlayerMovement>().orientation;
        }else
        {
            orientation = self.transform;
        }
    }

    public void SetScore(int score)
    {
        int diff = Score - score;
        Score = score;

        UIManager.Singleton.UpdatePlayerList();
    }
}
