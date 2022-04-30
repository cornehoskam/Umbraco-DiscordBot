using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using UmbracoDiscord.ModelsBuilder;

namespace UmbracoDiscord.Bot.Classes;

public static class DiscordBotExtensions
{
    public static IServiceCollection AddDiscordBot(this IServiceCollection services)
    {
        var discordClient = new BotClient(services);
        return services;
    }
}