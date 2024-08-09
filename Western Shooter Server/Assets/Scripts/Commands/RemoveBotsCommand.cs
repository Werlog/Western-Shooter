using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBotsCommand : BaseCommand
{
    public RemoveBotsCommand() : base("removebots") { }

    public override void Execute(string[] args, Player sender)
    {
        GameManager.Singleton.RemoveAllBots();
    }
}
