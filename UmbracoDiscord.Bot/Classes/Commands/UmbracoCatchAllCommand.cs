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
        
        var initialString = message.Content.Split()[0];
        var key = (initialString, message.GetServerFromMessage()!.Id.ToString());
        if (commands.ContainsKey(key))
        {
            var command = commands[key];
            ReplyAsync(command);
        }
        
        return Task.CompletedTask;

    }
    
    private Dictionary<(string command, string serverId), string> GetCustomUmbracoServerCommands()
    {
        var _umbracoCref = _umbracoContextFactory.EnsureUmbracoContext();
        if (_umbracoCref == null)
        {
            return new Dictionary<(string command, string serverId), string>();
        }

        var umbracoDiscordClient = _umbracoCref.UmbracoContext.Content.GetAtRoot()
            .FirstOrDefault();
        
        var dictionary = new Dictionary<(string command, string serverId), string>();
        foreach (var server in umbracoDiscordClient.Children.OfType<UmbracoDiscordServer>())
        {
            var umbracoCommands = server
                .Children.OfType<CommandCollection>()
                .FirstOrDefault()?
                .Children?.OfType<CustomCommand>().ToList();
            
            if (umbracoCommands == null || !umbracoCommands.Any()) continue;
            
            foreach (var command in umbracoCommands)
            {
                dictionary.Add(($"?{command.Command}", server.ServerID)!, command.Response!);
            }
        }
        return dictionary;
    }
}