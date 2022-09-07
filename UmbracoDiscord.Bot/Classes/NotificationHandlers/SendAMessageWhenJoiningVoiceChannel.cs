using Umbraco.Cms.Core.Events;
using UmbracoDiscord.Bot.Classes.Notifications;

namespace UmbracoDiscord.Bot.Classes.NotificationHandlers;

public class SendAMessageWhenJoiningVoiceChannel : INotificationHandler<JoinedVoiceChannelNotification>
{
    public void Handle(JoinedVoiceChannelNotification notification)
    {
        Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {notification.User.Username} has joined a Voice Channel!");
    }
}