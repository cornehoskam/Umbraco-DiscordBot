using Discord.WebSocket;
using Umbraco.Cms.Core.Notifications;

namespace UmbracoDiscord.Bot.Classes.Notifications;

public class JoinedVoiceChannelNotification : INotification
{
    public SocketGuildUser User { get; set; }
    public SocketVoiceState VoiceStateBefore { get; set; }
    public SocketVoiceState VoiceStateAfter { get; set; }
    
    public JoinedVoiceChannelNotification(SocketGuildUser user, SocketVoiceState state1, SocketVoiceState state2)
    {
        
        User = user;
        VoiceStateBefore = state1;
        VoiceStateAfter = state2;
    }
}