using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCommand : BaseCommand
{
    public SpectatorCommand() : base("spectator") { }

    public override void Execute(string[] args, Player sender)
    {
        sender.Die(null, false);
    }
}
