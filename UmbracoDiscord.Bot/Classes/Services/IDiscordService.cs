using Discord.WebSocket;

namespace UmbracoDiscord.Bot.Classes.Services;

public interface IDiscordService
{
    public List<DiscordBotInstance> SocketClients { get; set; }
}