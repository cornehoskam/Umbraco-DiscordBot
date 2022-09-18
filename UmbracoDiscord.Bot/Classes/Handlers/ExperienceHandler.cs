using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UmbracoDiscord.Bot.Classes.Helpers;
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
        
        var stat = GetOrCreateStats(message, context, out var created);

        stat.ServerName = message.GetServerFromMessage().Name;
        stat.UserName = $"{message.Author.Username}#{message.Author.Discriminator}";

        if (stat.LastMessage is null || stat.LastMessage == DateTime.MinValue)
        {
            AddExperience(stat, 10);
            message.Channel.SendMessageAsync($"{message.Author.Username} has gained 10 experience!");
        }
        else if(stat.LastMessage.Value.AddMinutes(5) < DateTime.UtcNow)
        {
            AddExperience(stat, 10);
            message.Channel.SendMessageAsync($"{message.Author.Username} has gained 10 experience!");
        }

        var entry = created ? context.Stats.Add(stat) : context.Stats.Update(stat);
        
        context.SaveChanges();
        return Task.CompletedTask;
    }

    private static void AddExperience(Stats stat, int experience)
    {
        stat.Experience += experience;
        stat.LastMessage = DateTime.Now.ToUniversalTime();
    }

    private Stats GetOrCreateStats(SocketMessage message, UmbracoDiscordDbContext context, out bool created)
    {
        var stat = context.Stats.FirstOrDefault(x =>
            x.ServerId == message.GetServerFromMessage().Id.ToString() &&
            x.UserId == message.Author.Id.ToString());

        if (stat is not null)
        {
            created = false;
            return stat;
        }
        
        stat = new Stats
        {
            UserId = message.Author.Id.ToString(),
            ServerId = message.GetServerFromMessage()!.Id.ToString(),
        };
        
        created = true;

        return stat;
    }
}