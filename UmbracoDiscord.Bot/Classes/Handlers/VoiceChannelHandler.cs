using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Events;
using UmbracoDiscord.Bot.Classes.Notifications;

namespace UmbracoDiscord.Bot.Classes.Handlers;

public class VoiceChannelHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _serviceProvider;

    // Retrieve client and CommandService instance via ctor
    public VoiceChannelHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider)
    {
        _commands = commands;
        _client = client;
        _serviceProvider = serviceProvider;
    }
    
    public Task InstallVoiceChannelEvents()
    {
        _client.UserVoiceStateUpdated += (user, state, arg3) =>
            SocketClientOnUserVoiceStateUpdated(user, state, arg3, _serviceProvider);
        
        return Task.CompletedTask;
    }
    
    private Task SocketClientOnUserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3, IServiceProvider serviceProvider)
    {
        var eventAggregator = serviceProvider.GetRequiredService<IEventAggregator>();

        if (arg2.VoiceChannel == null && arg3.VoiceChannel != null)
        {
            eventAggregator.Publish(new JoinedVoiceChannelNotification((arg1 as SocketGuildUser)!, arg2, arg3));
        }

        return Task.CompletedTask;
    }
}