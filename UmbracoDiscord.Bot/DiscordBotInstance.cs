using System.Reflection;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using UmbracoDiscord.Bot.Classes.Handlers;
using UmbracoDiscord.Bot.Classes.Helpers;
using UmbracoDiscord.ModelsBuilder;

namespace UmbracoDiscord.Bot;

public class DiscordBotInstance
{
    private readonly int _umbracoDiscordClientId;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    
    private UmbracoContextReference? _umbracoCref;
    private CommandHandler _commandHandler;
    private Dictionary<(string command, string serverId), string> CustomCommands { get; set; }
    
    public DiscordBotInstance(int umbracoDiscordClientId, string umbracoDiscordClientToken, IServiceProvider serviceProvider)
    {
        var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
        var socketClient = new DiscordSocketClient(_config);
        CustomCommands = new Dictionary<(string command, string serverId), string>();

        _umbracoDiscordClientId = umbracoDiscordClientId;
        _umbracoContextFactory = serviceProvider.GetRequiredService<IUmbracoContextFactory>();
        var _commandService = serviceProvider.GetRequiredService<CommandService>();

        _commandHandler = new CommandHandler(socketClient, _commandService, serviceProvider);
        
        EnsureNewUmbracoContext();
        
        Task.Run(() => Startup(socketClient, umbracoDiscordClientToken));
    }
    
    private async Task Startup(DiscordSocketClient socketClient, string token)
    {
        CustomCommands = GetCustomUmbracoServerCommands();

        socketClient.MessageReceived += HandleCustomUmbracoMessages;
        socketClient.MessageReceived += ReloadCustomUmbracoMessages;
        socketClient.MessageReceived += DumpCommands;
        
        await _commandHandler.InstallCommandsAsync();
        await socketClient.LoginAsync(TokenType.Bot, token);
        await socketClient.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    
    private Task ReloadCustomUmbracoMessages(SocketMessage arg)
    {
        if (arg.Content.StartsWith("?reload"))
        {
            EnsureNewUmbracoContext();
            var commands = GetCustomUmbracoServerCommands();
            arg.Channel.SendMessageAsync($"Reloading {commands.Count} commands!");
            CustomCommands = commands;
        }

        return Task.CompletedTask;
    }
    
    private Task DumpCommands(SocketMessage arg)
    {
        if (arg.Content.StartsWith("?dump"))
        {
            EnsureNewUmbracoContext();
            var commands = GetCustomUmbracoServerCommands();
            var sb = new StringBuilder();
            sb.Append("Currently registered commands:");
            sb.Append("\r\n");
            foreach (var com in commands)
            {
                sb.Append("-");
                sb.Append(com.Key.command);
                sb.Append("\r\n");
            }
            arg.Channel.SendMessageAsync(sb.ToString());
        }

        return Task.CompletedTask;
    }

    private void EnsureNewUmbracoContext()
    {
        _umbracoCref?.Dispose();
        _umbracoCref = _umbracoContextFactory.EnsureUmbracoContext();
    }

    private Dictionary<(string command, string serverId), string> GetCustomUmbracoServerCommands()
    {
        if (_umbracoCref == null)
        {
            return new Dictionary<(string command, string serverId), string>();
        }
        
        var umbracoDiscordClient = _umbracoCref.UmbracoContext.Content.GetById(_umbracoDiscordClientId); 
        var dictionary = new Dictionary<(string command, string serverId), string>();
        foreach (var server in umbracoDiscordClient.Children.OfType<UmbracoDiscordServer>())
        {
            var umbracoCommands = server
                .Children.OfType<CommandCollection>()
                .FirstOrDefault()?
                .Children?.OfType<CustomCommand>().ToList();
            
            if (umbracoCommands == null || !umbracoCommands.Any()) continue;
            
            foreach (var command in umbracoCommands)
            {
                dictionary.Add(($"?{command.Command}", server.ServerID)!, command.Response!);
            }
        }
        return dictionary;
    }
    
    private Task HandleCustomUmbracoMessages(SocketMessage message)
    {
        var initialString = message.Content.Split()[0];
        var key = (initialString, message.GetServerFromMessage()!.Id.ToString());
        if (CustomCommands.ContainsKey(key))
        {
            var command = CustomCommands[key];
            message.Channel.SendMessageAsync(command);
        }
        return Task.CompletedTask;
    }
}