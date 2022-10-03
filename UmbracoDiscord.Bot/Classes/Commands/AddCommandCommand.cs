using Discord;
using Discord.Commands;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using UmbracoDiscord.ModelsBuilder;

namespace UmbracoDiscord.Bot.Classes.Commands;

public class AddCommandCommand : ModuleBase<SocketCommandContext>
{
    
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IContentService _contentService;

    public AddCommandCommand(IUmbracoContextFactory umbracoContextFactory, IContentService contentService)
    {
        _umbracoContextFactory = umbracoContextFactory;
        _contentService = contentService;
    }
    
    [Command("addCommand")]
    [RequireBotPermission(GuildPermission.Administrator)]
    public async Task AddCommandAsync(string command, [Remainder]string response)
    {
        var context = _umbracoContextFactory.EnsureUmbracoContext();
        var rootNode = context.UmbracoContext.Content?.GetAtRoot().FirstOrDefault();
        var commandCollection = rootNode?
            .ChildrenOfType(UmbracoDiscordServer.ModelTypeAlias)
            ?.FirstOrDefault(x => (x as UmbracoDiscordServer)?.ServerID == Context.Guild.Id.ToString())
            ?.FirstChildOfType(CommandCollection.ModelTypeAlias);

        if (commandCollection is not null)
        {
            var commandNode = _contentService.Create(command, commandCollection.Id, CustomCommand.ModelTypeAlias);
            
            commandNode.SetValue("command", command.ToLower());
            commandNode.SetValue("response", response);
            
            _contentService.SaveAndPublish(commandNode);
            
            await ReplyAsync($"Added command {command.ToLower()} to the server!");
        }
        else
        {
            await ReplyAsync("Something went wrong adding command!");
        }
    }
}