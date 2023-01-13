using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UmbracoDiscord.Bot.Classes.Helpers;
using UmbracoDiscord.Domain.Context;
using UmbracoDiscord.Domain.Entities;

namespace UmbracoDiscord.Bot.Classes.Handlers;

public class ExperienceHandler
{
    private const int ExperienceOnMessage = 10;
    private const int TimeoutInMinutes = 0; // 0 = no timeout
    
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

        if (IsFirstMessage(stat) || (stat.LastMessage.HasValue && stat.LastMessage.Value.AddMinutes(TimeoutInMinutes) < DateTime.UtcNow))
        {
            AddExperience(stat, message, ExperienceOnMessage);
            LevelUp(stat, message, context);
        }

        var entry = created ? context.Stats.Add(stat) : context.Stats.Update(stat);
        
        context.SaveChanges();
        return Task.CompletedTask;
    }

    private void LevelUp(Stats stat, SocketMessage message, UmbracoDiscordDbContext context)
    {
        if(stat.Experience < 100) return;
        if (stat.Experience >= 100 && stat.Experience < 200) SetLevel(stat, message, 1);
        if (stat.Experience >= 200 && stat.Experience < 300) SetLevel(stat, message, 2);
    }

    private void SetLevel(Stats stat, SocketMessage message, int i)
    {
        var role = message.GetServerFromMessage().Roles.FirstOrDefault(x => x.Name == $"Level {i}");
        if (role == null) return;
        
        var user = message.GetServerFromMessage().GetUser(message.Author.Id);
        if (user == null || user.Roles.Contains(role)) return;
        
        user.AddRoleAsync(role);
        message.Channel.SendMessageAsync($"Congratulations {message.Author.Mention}! You have reached level {i}!");
    }


    private static void AddExperience(Stats stat, SocketMessage message,  int experience)
    {
        stat.Experience += experience;
        stat.LastMessage = DateTime.Now.ToUniversalTime();
        //message.Channel.SendMessageAsync($"{message.Author.Username} has gained 10 experience!");
    }

    private static bool IsFirstMessage(Stats stat)
    {
        return stat.LastMessage is null || stat.LastMessage == DateTime.MinValue;
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