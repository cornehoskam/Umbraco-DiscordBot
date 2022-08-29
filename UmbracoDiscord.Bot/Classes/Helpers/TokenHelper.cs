using Discord.Commands;
using Discord.WebSocket;

namespace UmbracoDiscord.Bot.Classes.Helpers;

public static class TokenHelper
{
    public static string ParseTokens(this string input, SocketCommandContext context)
    {
        return input.ParseAuthor(context)
            .ParseChannel(context)
            .ParseGuild(context);
    }
    
    private static string ParseAuthor(this string input, SocketCommandContext context)
    {
        var user = context.User as SocketGuildUser;
        return input.Replace("{author.username}", user.Username, StringComparison.InvariantCultureIgnoreCase)
            .Replace("{author.isbot}", user.IsBot ? "yes" : "no", StringComparison.InvariantCultureIgnoreCase)
            .Replace("{author.status}", user.Status.ToString(), StringComparison.InvariantCultureIgnoreCase)
            .Replace("{author.roles}", user.Roles.Count.ToString(), StringComparison.InvariantCultureIgnoreCase)
            .Replace("{author.isstreaming}", user.IsStreaming ? "yes" : "no", StringComparison.InvariantCultureIgnoreCase);
    }
    
    private static string ParseChannel(this string input, SocketCommandContext context)
    {
        return input.Replace("{channel.name}", context.Channel.Name, StringComparison.InvariantCultureIgnoreCase);
    }
    
    private static string ParseGuild(this string input, SocketCommandContext context)
    {
        return input.Replace("{guild.membercount}", context.Guild.MemberCount.ToString(),
            StringComparison.InvariantCultureIgnoreCase);
    }
}