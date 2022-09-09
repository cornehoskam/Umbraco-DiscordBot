using System.Linq;
using System.Text;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Events;
using UmbracoDiscord.Bot.Classes.Helpers;
using UmbracoDiscord.Bot.Classes.Notifications;
using UmbracoDiscord.Domain.Context;
using UmbracoDiscord.Domain.Entities;

namespace UmbracoDiscord.Bot.Classes.Handlers;

public class ExperienceHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;

    public ExperienceHandler(DiscordSocketClient client, IServiceProvider serviceProvider)
    {
        _client = client;
        _serviceProvider = serviceProvider;
    }
    
    public Task InstallMessageReceivedEvents()
    {
        _client.MessageReceived += message => MessageReceived(message, _serviceProvider);
        
        return Task.CompletedTask;
    }
    
    private Task MessageReceived(SocketMessage message, IServiceProvider serviceProvider)
    {
        if (message.Author.IsBot)
        {
            return Task.CompletedTask;
        }
        
        using var context = new UmbracoDiscordDbContext();
        var update = true;
        var stat = context.Stats.FirstOrDefault(x =>
            x.ServerId == message.GetServerFromMessage()!.Id.ToString() &&
            x.UserId == message.Author.Id.ToString());
        
        if (stat is null)
        {
            update = false;
            stat = new Stats()
            {
                UserId = message.Author.Id.ToString(),
                ServerId = message.GetServerFromMessage()!.Id.ToString(),
            };
        }
        
        stat.ServerName = message.GetServerFromMessage().Name;
        stat.UserName = $"{message.Author.Username}#{message.Author.Discriminator}";
        if (update)
        {
            context.Stats.Update(stat);
        }
        else
        {
            context.Stats.Add(stat);
        }

        stat.Experience += 10;
        context.SaveChanges();
        return Task.CompletedTask;
    }
}