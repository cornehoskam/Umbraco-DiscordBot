using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using UmbracoDiscord.Bot.Classes.Handlers;

namespace UmbracoDiscord.Bot;

public class DiscordBotInstance
{
    private readonly CommandHandler _commandHandler;
    private readonly VoiceChannelHandler _voiceChannelHandler;
    
    public DiscordBotInstance(string umbracoDiscordClientToken, IServiceProvider serviceProvider)
    {
        //Use only to instantiate Handlers & Services
        var socketClient = new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 100, AlwaysDownloadUsers = true});
        
        //Create commandHandler
        var commandService = serviceProvider.GetRequiredService<CommandService>();
        commandService.Log += LogAsync;
        socketClient.Log += LogAsync;
        _commandHandler = new CommandHandler(socketClient, commandService, serviceProvider);
        
        _voiceChannelHandler = new VoiceChannelHandler(socketClient, commandService, serviceProvider);
        
        Task.Run(() => Startup(socketClient, umbracoDiscordClientToken));
    }
    
    private async Task Startup(DiscordSocketClient socketClient, string token)
    {
        //Use to configure the SocketClient Events.
        await _commandHandler.InstallCommandsAsync();
        await _voiceChannelHandler.InstallVoiceChannelEvents();

        await socketClient.LoginAsync(TokenType.Bot, token);
        await socketClient.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    
    private Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                              + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
        }
        else 
            Console.WriteLine($"[General/{message.Severity}] {message}");

        return Task.CompletedTask;
    }
    
}