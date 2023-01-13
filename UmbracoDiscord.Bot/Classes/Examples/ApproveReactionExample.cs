using Discord;
using Discord.WebSocket;

namespace UmbracoDiscord.Bot.Classes.Examples;

public static class ApproveReactionExample
{
    public static async Task SocketClientOnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (message.Id == 1024013681484374026 && channel.Id == 1024013310036807773 && reaction.Emote.Name == "✅")
        {
            var role = (reaction.Channel as SocketGuildChannel)!.Guild.Roles.FirstOrDefault(x => x.Name == "accepted");
            if (role is not null)
            {
                await (reaction.User.Value as IGuildUser)!.AddRoleAsync(role);
            }
        }
    }
}