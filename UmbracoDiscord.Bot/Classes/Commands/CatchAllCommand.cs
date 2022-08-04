using Discord.Commands;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class CatchAllCommand : ModuleBase<SocketCommandContext>
{
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    public const string CatchAllCommandIdentifier = "459493c0-c5fb-424e-80e4-90d924075983";

    public CatchAllCommand(IUmbracoContextFactory umbracoContextFactory)
    {
        _umbracoContextFactory = umbracoContextFactory;
    }

    [Command(CatchAllCommandIdentifier)]
    [Summary("Get the total amount of umbraco nodes")]
    public Task CatchAll()
    {
        //Every command will enter here

        return Task.FromException(new Exception());
    }
}