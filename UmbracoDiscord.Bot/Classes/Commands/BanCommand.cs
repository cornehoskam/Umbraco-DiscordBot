using Discord;
using Discord.Commands;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class BanCommand : ModuleBase<SocketCommandContext>
{
   
    [Command("ban")]
    [RequireBotPermission(GuildPermission.BanMembers)]
    [RequireBotPermission(GuildPermission.BanMembers)]
    public async Task BanAsync(IGuildUser? guildUser = null, [Remainder]string? reason = null)
    {
        if (guildUser is null)
        {
            await ReplyAsync($"No user specified!");
            return;
        }

        if (reason is null)
        {
            await guildUser.BanAsync();
            await ReplyAsync($"Banned {guildUser.Username}#{guildUser.Discriminator}");
            return;
        }
        
        await guildUser.BanAsync(reason: reason);
        await ReplyAsync($"Banned {guildUser.Username}#{guildUser.Discriminator} for reason: {reason}");
    }
}