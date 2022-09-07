using Discord;
using Discord.Commands;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class KickCommand : ModuleBase<SocketCommandContext>
{
   
    [Command("kick")]
    [RequireBotPermission(GuildPermission.KickMembers)]
    [RequireBotPermission(GuildPermission.KickMembers)]
    public async Task KickAsync(IGuildUser? guildUser = null, [Remainder]string? reason = null)
    {
        if (guildUser is null)
        {
            await ReplyAsync($"No user specified!");
            return;
        }

        if (reason is null)
        {
            await guildUser.KickAsync(reason);
            await ReplyAsync($"Kicked {guildUser.Username}#{guildUser.Discriminator}");
            return;
        }
        
        await guildUser.KickAsync(reason);
        await ReplyAsync($"Kicked {guildUser.Username}#{guildUser.Discriminator} for reason: {reason}");
    }
}