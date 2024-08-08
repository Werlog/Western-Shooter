using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCommand : BaseCommand
{
    public KillCommand() : base("kill") { }

    public override void Execute(string[] args, Player sender)
    {
        Player playerToKill = sender;
        Player killer = null;
        if (args.Length > 0)
        {
            Player player = FindPlayer(args[0]);
            if (player != null)
            {
                playerToKill = player;
            }

            if (args.Length > 1)
            {
                killer = FindPlayer(args[1]);
            }
        }

        playerToKill.Die(killer);
    }
}
