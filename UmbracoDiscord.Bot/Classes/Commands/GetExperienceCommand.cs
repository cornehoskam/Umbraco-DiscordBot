using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UmbracoDiscord.Domain.Context;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class GetExperienceCommand : ModuleBase<SocketCommandContext>
{
    private readonly IServiceProvider _serviceProvider;

    public GetExperienceCommand(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    [Command("GetExperience")]
    public async Task GetExperienceAsync(SocketUser? user = null)
    {
        var userInfo = user ?? Context.User;
        await using var dbContext = new UmbracoDiscordDbContext();
            
        var stat = dbContext.Stats.FirstOrDefault(x =>
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