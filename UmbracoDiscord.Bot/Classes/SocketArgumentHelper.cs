using Discord.WebSocket;

namespace UmbracoDiscord.Bot.Classes
{
    public static class SocketArgumentHelper
    {
        public static SocketUser GetAuthorFromMessage(this SocketMessage message)
        {
            return message.Author;
        }

        public static SocketGuildChannel? GetChannelFromMessage(this SocketMessage message)
        {
            return message.Channel as SocketGuildChannel;
        }

        public static SocketGuild? GetServerFromMessage(this SocketMessage message)
        {
            return message.GetChannelFromMessage()?.Guild;
        }
    }
}
