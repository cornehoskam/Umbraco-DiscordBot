﻿using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using UmbracoDiscord.Bot.Classes.Commands;

namespace UmbracoDiscord.Bot.Classes.Handlers;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _serviceProvider;

    // Retrieve client and CommandService instance via ctor
    public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider)
    {
        _commands = commands;
        _client = client;
        _serviceProvider = serviceProvider;
    }
    
    public async Task InstallCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;
        
        await _commands.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(), _serviceProvider);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix('?', ref argPos) || 
              message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(_client, message);

        //Use a general Catch All command first.
        var customCommand = _commands.Search(CatchAllCommand.CatchAllCommandIdentifier).Commands.FirstOrDefault();
        var result = await customCommand.ExecuteAsync(context, new List<object>(), new List<object>(), _serviceProvider);
        
        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        if (!result.IsSuccess)
        {
            await _commands.ExecuteAsync(
                context: context, 
                argPos: argPos,
                services: _serviceProvider);
        }
    }
}