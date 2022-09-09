using Discord;
using Discord.Commands;
using UmbracoDiscord.Domain.Context;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class TestCommand : ModuleBase<SocketCommandContext>
{
   
    [Command("test")]
    public async Task TestAsync()
    {
        await using var context = new UmbracoDiscordDbContext();
        await ReplyAsync($"Database row count: {context.Stats.Count()}");
    }
}