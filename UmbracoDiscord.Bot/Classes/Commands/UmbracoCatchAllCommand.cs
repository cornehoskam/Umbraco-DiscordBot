using Discord.Commands;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using UmbracoDiscord.Bot.Classes.Helpers;
using UmbracoDiscord.ModelsBuilder;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class UmbracoCatchAllCommand : ModuleBase<SocketCommandContext>
{
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    public const string CatchAllCommandIdentifier = "459493c0-c5fb-424e-80e4-90d924075983";

    public UmbracoCatchAllCommand(IUmbracoContextFactory umbracoContextFactory)
    {
        _umbracoContextFactory = umbracoContextFactory;
    }

    [Command(CatchAllCommandIdentifier)]
    [Summary("Get the total amount of umbraco nodes")]
    public Task CatchAll()
    {
        var commands = GetCustomUmbracoServerCommands();
        var message = Context.Message;
        
        var initialString = message.Content.Split()[0].ToLower();
        var key = (initialString, message.GetServerFromMessage()!.Id.ToString());
        if (commands.ContainsKey(key))
        {
            ReplyAsync(commands[key].ParseTokens(Context));
        }
        
        return Task.CompletedTask;

    }
    
    private Dictionary<(string command, string serverId), string> GetCustomUmbracoServerCommands()
    {
        var umbracoCref = _umbracoContextFactory.EnsureUmbracoContext();

        var umbracoDiscordClient = umbracoCref.UmbracoContext.Content?.GetAtRoot()
            .FirstOrDefault();
        
        var responseDictionary = new Dictionary<(string command, string serverId), string>();
        var servers = umbracoDiscordClient?.Children?.OfType<UmbracoDiscordServer>().ToList();
        if (servers == null || !servers.Any())
        {
            return responseDictionary;
        }
        
        foreach (var server in servers)
        {
            var umbracoCommands = server
                .Children<CommandCollection>()?
                .FirstOrDefault()?
                .Children<CustomCommand>()?.ToList();
            
            if (umbracoCommands == null || !umbracoCommands.Any()) continue;
            
            foreach (var command in umbracoCommands)
            {
                var key = ($"?{command.Command}".ToLower(), server.ServerID);
                if (!responseDictionary.ContainsKey(key))
                {
                    var value = command.Response;
                    responseDictionary.Add(key!, value!);
                }
            }
        }
        return responseDictionary;
    }
}