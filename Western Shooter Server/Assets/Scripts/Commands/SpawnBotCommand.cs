using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBotCommand : BaseCommand
{
    public SpawnBotCommand() : base("spawnbot") { }

    public override void Execute(string[] args, Player sender)
    {
        int amount = 1;
        if (args.Length > 0 && int.TryParse(args[0], out int newAmount))
        {
            amount = newAmount;
        }

        amount = Mathf.Clamp(amount, 0, 20);

        for (int i = 0; i < amount; i++)
        {
            GameManager.Singleton.JoinBot(BotNames.GetRandomBotName());
        }
    }
}
