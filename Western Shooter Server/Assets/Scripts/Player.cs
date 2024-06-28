using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public ushort PlayerID { get; private set; }
    public string Username { get; private set; }

    public GameObject self;

    public Player(ushort playerID, string username)
    {
        PlayerID = playerID;
        Username = username;
    }
}
