using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private static CommandManager _singleton;

    public static CommandManager Singleton
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
                Debug.LogWarning($"{nameof(CommandManager)}: Singleton already exists destroying duplicate");
                Destroy(value);
            }
        }
    }

    public List<BaseCommand> commands = new List<BaseCommand>();

    private void Awake()
    {
        ReloadCommands();
        Singleton = this;
    }

    public void ReloadCommands()
    {
        commands.Clear();

        commands.Add(new KillCommand());
        commands.Add(new SpawnBotCommand());
        commands.Add(new RemoveBotsCommand());
    }

    public void ProcessCommand(string commandMessage, Player fromPlayer)
    {
        commandMessage = commandMessage.Replace("/", "");
        int spaceIndex = commandMessage.IndexOf(" ");

        string commandName;
        if (spaceIndex != -1)
        {
            commandName = commandMessage.Substring(0, spaceIndex);
        }
        else
        {
            commandName = commandMessage;
        }
        if (commandName != null)
        {
            string[] args;
            if (spaceIndex != -1)
                args = commandMessage.Substring(spaceIndex + 1).Split(" ");
            else args = new string[0];
            BaseCommand cmd = FindCommandByName(commandName);
            if (cmd != null)
            {
                cmd.Execute(args, fromPlayer);
            }
        }
    }

    public BaseCommand FindCommandByName(string name)
    {
        foreach (BaseCommand command in commands)
        {
            if (command.CommandName == name)
            {
                return command;
            }
        }

        return null;
    }
}
