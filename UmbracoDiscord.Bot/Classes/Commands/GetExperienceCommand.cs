using Discord.Commands;
using Discord.WebSocket;
using UmbracoDiscord.Domain.Context;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class GetExperienceCommand : ModuleBase<SocketCommandContext>
{
   
    [Command("GetExperience")]
    public async Task GetExperienceAsync(SocketUser? user = null)
    {
        var userInfo = user ?? Context.User;
        await using var context = new UmbracoDiscordDbContext();
        var stat = context.Stats.FirstOrDefault(x =>
            x.UserId == userInfo.Id.ToString() && x.ServerId == Context.Guild.Id.ToString());
        
        if (stat is null)
        {
            await ReplyAsync($"User {userInfo.Username} has no Experience Points!");
        }
        else
        {
            await ReplyAsync($"User {userInfo.Username} has {stat.Experience} Experience Points");
        }
    }
}