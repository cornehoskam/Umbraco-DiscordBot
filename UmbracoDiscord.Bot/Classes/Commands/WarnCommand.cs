// using Discord;
// using Discord.Commands;
//
// namespace UmbracoDiscord.Bot.Classes.Commands;
//
// public class WarnCommand : ModuleBase<SocketCommandContext>
// {
//     
//     private WarnService _warnService { get; set; }
//     
//     
//     [Command("warn")]
//     [RequireBotPermission(GuildPermission.KickMembers)]
//     [RequireBotPermission(GuildPermission.BanMembers)]
//     public async Task WarnAsync(IGuildUser? guildUser = null, [Remainder]string? reason = null)
//     {
//         if (guildUser is null)
//         {
//             await ReplyAsync("No user specified!");
//             return;
//         }
//
//         var warnCount = _warnService.GetWarnings(guildUser);
//
//         if (warnCount < 3)
//         {
//             await ReplyAsync($"Warned {guildUser.Username}#{guildUser.Discriminator}");
//         }
//         
//         if (warnCount == 3)
//         {
//             await guildUser.KickAsync();
//             await ReplyAsync($"Kicked {guildUser.Username}#{guildUser.Discriminator}");
//         }
//
//         if (warnCount >= 4)
//         {
//             await guildUser.BanAsync();
//             await ReplyAsync($"Banned {guildUser.Username}#{guildUser.Discriminator}");
//         }
//
//         _warnService.RegisterWarning(guildUser);
//     }
// }
//
// public class WarnService
// {
//     public int GetWarnings(IGuildUser user)
//     {
//         return 1;
//     }
//
//     public void RegisterWarning(IGuildUser user)
//     {
//         
//     }
// }