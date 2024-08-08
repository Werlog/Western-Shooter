using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCommand
{
    public string CommandName { get; private set; }

    public BaseCommand(string commandName)
    {
        CommandName = commandName;
    }

    public abstract void Execute(string[] args, Player sender);

    protected Player FindPlayer(string arg)
    {
        Player returnPlayer = null;
        foreach (Player p in GameManager.Singleton.players.Values)
        {
            if (p.Username == arg)
            {
                returnPlayer = p;
                break;
            }
        }

        return returnPlayer;
    }
}
