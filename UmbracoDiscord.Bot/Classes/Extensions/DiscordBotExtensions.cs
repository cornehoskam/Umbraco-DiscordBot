using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Web;
using UmbracoDiscord.Bot.Classes.Services;
using UmbracoDiscord.ModelsBuilder;

namespace UmbracoDiscord.Bot.Classes.Extensions;

public static class DiscordBotExtensions
{
    public static IApplicationBuilder AddDiscordBot(this IApplicationBuilder app)
    {
        var discordService = app.ApplicationServices.GetRequiredService<IDiscordService>();
        if (discordService == null)
        {
            throw new Exception("Failed to retrieve Discord Service");
        }
        
        var umbracoContext = app.ApplicationServices.GetRequiredService<IUmbracoContextFactory>().EnsureUmbracoContext();
        var clients = umbracoContext.UmbracoContext?.Content?
            .GetAtRoot().OfType<UmbracoDiscordClient>().Select(x => (x.Id, x.Token)).ToList();
        umbracoContext.Dispose();

        if (clients == null || !clients.Any()) 
            return app;
        
        foreach (var discordClient in clients)
        {
            discordService.SocketClients.Add(new DiscordBotInstance(discordClient.Token!, app.ApplicationServices));
        }

        return app;
    }
}