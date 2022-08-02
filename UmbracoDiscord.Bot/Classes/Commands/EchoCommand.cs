using Discord.Commands;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class EchoCommand : ModuleBase<SocketCommandContext>
{
    [Command("echo")]
    [Summary("Echoes a message.")]
    public Task EchoAsync([Remainder] [Summary("The text to echo")] string echo)
        => ReplyAsync(echo); 
}