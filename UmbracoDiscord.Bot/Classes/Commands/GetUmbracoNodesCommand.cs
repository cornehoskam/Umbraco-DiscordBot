using Discord.Commands;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class GetUmbracoNodesCommand : ModuleBase<SocketCommandContext>
{
    private readonly IUmbracoContextFactory _umbracoContextFactory;

    public GetUmbracoNodesCommand(IUmbracoContextFactory umbracoContextFactory)
    {
        _umbracoContextFactory = umbracoContextFactory;
    }

    
    [Command("GetUmbracoNodes")]
    [Summary("Get the total amount of umbraco nodes")]
    public Task GetUmbracoNodes()
    {
        var context = _umbracoContextFactory.EnsureUmbracoContext();
        var rootNodes = context.UmbracoContext.Content?.GetAtRoot();
        var total = 0;
        foreach (var publishedContent in rootNodes)
        {
            total += publishedContent.Descendants().Count() + 1;
        }
        return ReplyAsync($"Umbraco Instance has {total} Content Nodes");
    }
}