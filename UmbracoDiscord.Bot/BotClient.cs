using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using UmbracoDiscord.Bot.Classes;
using UmbracoDiscord.ModelsBuilder;

namespace UmbracoDiscord.Bot;

public class BotClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<DiscordSocketClient> _clients;
    
    public BotClient(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _clients = new List<DiscordSocketClient>();

        var context = _serviceProvider.GetRequiredService<IUmbracoContextFactory>().EnsureUmbracoContext();
        
        var clients = context.UmbracoContext.Content
            .GetAtRoot().OfType<DiscordClient>().ToList();
        
        foreach (var discordClient in clients)
        {
            var socketClient = new DiscordSocketClient();
            
            Task.Run(() => Startup(socketClient, discordClient));
            
            _clients.Add(socketClient);
        }
    }

    private async Task Startup(DiscordSocketClient socketClient, DiscordClient discordClient)
    {
        socketClient.Log += Log;
        socketClient.MessageReceived += LogAllMessages;
        socketClient.MessageReceived += message => HowManyNodes(message, _serviceProvider);

        foreach (var server in discordClient.Children.OfType<DiscordServer>())
        {
            var commandCollection = server.Children.OfType<CommandCollection>().FirstOrDefault();
            if (commandCollection != null)
            {
                foreach (var command in commandCollection.Children?.OfType<CustomCommand>().ToList()!)
                {
                    socketClient.MessageReceived += message => RegisterMessages(message, command);
                }
            }
        }
        
        await socketClient.LoginAsync(TokenType.Bot, discordClient.Token);
        await socketClient.StartAsync();
            
        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private Task RegisterMessages(SocketMessage message, CustomCommand command)
    {
        if (message.Content.StartsWith($"?{command.Command}"))
        {
            message.Channel.SendMessageAsync(command.Response);
        }

        return Task.CompletedTask;
    }

    private Task HowManyNodes(SocketMessage arg, IServiceProvider serviceProvider)
    {
        if (!arg.Content.StartsWith("?getnodes")) 
            return Task.CompletedTask;

        var server = arg.GetServerFromMessage();
        var contentService = serviceProvider.GetRequiredService<IContentService>();
        var nodes = contentService.Count();
        
        arg.Channel.SendMessageAsync($"Nodes: {nodes}");

        return Task.CompletedTask;
    }

    private Task LogAllMessages(SocketMessage arg)
    {
        Console.WriteLine(DateTime.Now.ToShortTimeString() + " - " + arg.Author.Username + ": " +arg.Content);
        return Task.CompletedTask;
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
    
}