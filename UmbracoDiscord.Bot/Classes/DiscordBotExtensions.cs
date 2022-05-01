using Microsoft.AspNetCore.Builder;

namespace UmbracoDiscord.Bot.Classes;

public static class DiscordBotExtensions
{
    public static IApplicationBuilder AddDiscordBot(this IApplicationBuilder app)
    {
        var services = app.ApplicationServices;
        var discordClient = new BotClient(services);
        return app;
    }
}