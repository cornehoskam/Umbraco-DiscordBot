namespace UmbracoDiscord.Bot.Classes.Services;

public class DiscordService : IDiscordService
{
    public List<DiscordBotInstance> SocketClients { get; set; }

    public DiscordService()
    {
        SocketClients = new List<DiscordBotInstance>();
    }
}