using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using UmbracoDiscord.Bot.Classes.Helpers;
using UmbracoDiscord.ModelsBuilder;

namespace UmbracoDiscord.Bot;

public class DiscordBotInstance
{
    private readonly int _umbracoDiscordClientId;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    
    private UmbracoContextReference? _umbracoCref;
    private Dictionary<string, string> CustomCommands { get; set; }
    
    public DiscordBotInstance(int umbracoDiscordClientId, string umbracoDiscordClientToken, IServiceProvider serviceProvider)
    {
        var socketClient = new DiscordSocketClient();
        CustomCommands = new Dictionary<string, string>();

        _umbracoDiscordClientId = umbracoDiscordClientId;
        _umbracoContextFactory = serviceProvider.GetRequiredService<IUmbracoContextFactory>();

        EnsureNewUmbracoContext();
        
        Task.Run(() => Startup(socketClient, umbracoDiscordClientToken));
    }
    
    private async Task Startup(DiscordSocketClient socketClient, string token)
    {
        CustomCommands = GetCustomUmbracoServerCommands();

        socketClient.MessageReceived += HandleCustomUmbracoMessages;
        socketClient.MessageReceived += ReloadCustomUmbracoMessages;
        
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

    private void EnsureNewUmbracoContext()
    {
        _umbracoCref?.Dispose();
        _umbracoCref = _umbracoContextFactory.EnsureUmbracoContext();
    }

    private Dictionary<string, string> GetCustomUmbracoServerCommands()
    {
        if (_umbracoCref == null)
        {
            return new Dictionary<string, string>();
        }
        
        var umbracoDiscordClient = _umbracoCref.UmbracoContext.Content.GetById(_umbracoDiscordClientId); 
        foreach (var server in umbracoDiscordClient.Children.OfType<UmbracoDiscordServer>())
        {
            var umbracoCommands = server
                .Children.OfType<CommandCollection>()
                .FirstOrDefault()?
                .Children?.OfType<CustomCommand>().ToList();
            
            if (umbracoCommands == null || !umbracoCommands.Any()) continue;

            return umbracoCommands.ToDictionary(command => $"?{command.Command}", command => command.Response)!;
        }

        return new Dictionary<string, string>();
    }
    
    private Task HandleCustomUmbracoMessages(SocketMessage message)
    {
        var initialString = message.Content.Split()[0];
        if (CustomCommands.ContainsKey(initialString))
        {
            var command = CustomCommands[initialString];
            message.Channel.SendMessageAsync(command);
        }
        return Task.CompletedTask;
    }
}