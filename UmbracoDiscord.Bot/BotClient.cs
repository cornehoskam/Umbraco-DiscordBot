using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using UmbracoDiscord.ModelsBuilder;

namespace UmbracoDiscord.Bot;

public class BotClient
{
    private readonly ServiceProvider _serviceProvider;
    private DiscordSocketClient _client;
    
    public BotClient(IServiceCollection services)
    {
        _client = new DiscordSocketClient();
        _serviceProvider = services.BuildServiceProvider();
        
        var contentService = _serviceProvider.GetRequiredService<IContentService>();
        var clients = contentService.GetRootContent()
            .Where(x => x.ContentType.Alias == DiscordClient.ModelTypeAlias).ToList();
        
        foreach (var client in clients)
        {
            Task.Run(() => Startup(client.GetValue("token").ToString()));
        }
    }

    private async Task Startup(string? token)
    {
        _client.Log += Log;
        _client.MessageReceived += LogAllMessages;
        _client.MessageReceived += message => HowManyNodes(message, _serviceProvider);

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
            
        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    
    private Task HowManyNodes(SocketMessage arg, ServiceProvider serviceProvider)
    {
        if (!arg.Content.StartsWith("?getnodes")) 
            return Task.CompletedTask;
        
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