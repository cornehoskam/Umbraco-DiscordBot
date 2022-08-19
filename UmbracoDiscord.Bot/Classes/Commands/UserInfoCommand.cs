using Discord.Commands;
using Discord.WebSocket;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class UserInfoCommand : ModuleBase<SocketCommandContext>
{
    [Command("userinfo")]
    [Summary
        ("Returns info about the current user, or the user parameter, if one passed.")]
    [Alias("user", "whois")]
    public async Task UserInfoAsync(
        [Summary("The (optional) user to get info from")]
        SocketUser? user = null)
    {
        var userInfo = user ?? Context.Client.CurrentUser;
        await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
    }
}