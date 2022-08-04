using Discord.Commands;
using Discord.WebSocket;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.UmbracoContext;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class GetUmbracoNodesCommand : ModuleBase<SocketCommandContext>
{
    public IUmbracoContextFactory _umbracoContextFactory { get; set; }
    
    [Command("test")]
    [Summary("do a test thingy")]
    public Task TestAsync()
    {
        return ReplyAsync("test");
    }
}