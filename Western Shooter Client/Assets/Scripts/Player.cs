using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public const int PlayerMaxHealth = 100;

    public ushort PlayerID { get; private set; }
    public string Username { get; private set; }
    public bool IsLocal { get; private set; }

    private int _health;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (IsLocal)
            {
                UIManager.Singleton.SetHealthText(_health);
            }
        }
    }
    public bool IsAlive { get; private set; }

    public GameObject self;

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
            GameManager.Singleton.SpawnSpectatorCamera(self.transform.position + Vector3.up, self.transform.eulerAngles);
        }
    }

    public void Respawn(Vector3 position, int health)
    {
        IsAlive = true;
        this.Health = health;

        self.SetActive(true);
        self.transform.position = position;

        if (IsLocal)
            GameManager.Singleton.DespawnSpectatorCamera();
    }
}
