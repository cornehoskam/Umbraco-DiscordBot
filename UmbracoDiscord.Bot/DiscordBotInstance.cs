using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UmbracoDiscord.Bot.Classes.Handlers;
using UmbracoDiscord.Domain.Context;

namespace UmbracoDiscord.Bot;

public class DiscordBotInstance
{
    private readonly CommandHandler _commandHandler;
    private readonly VoiceChannelHandler _voiceChannelHandler;
    private readonly ExperienceHandler _experienceHandler;
    
    public DiscordBotInstance(string umbracoDiscordClientToken, IServiceProvider serviceProvider)
    {
        //Use only to instantiate Handlers & Services
        var socketClient = new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 100, AlwaysDownloadUsers = true});
        
        //Create commandHandler
        var commandService = serviceProvider.GetRequiredService<CommandService>();

        commandService.Log += LogAsync;
        socketClient.Log += LogAsync;
        
        socketClient.ReactionAdded += SocketClientOnReactionAdded;
        socketClient.MessageReceived += SocketClientOnMessageReceived;
        
        _commandHandler = new CommandHandler(socketClient, commandService, serviceProvider);
        
        _voiceChannelHandler = new VoiceChannelHandler(socketClient, commandService, serviceProvider);
        _experienceHandler = new ExperienceHandler(socketClient, serviceProvider);
        
        Task.Run(() => Startup(socketClient, umbracoDiscordClientToken));
        

    }

    private static async Task SocketClientOnMessageReceived(SocketMessage arg)
    {
        if (!arg.Content.StartsWith("rps"))
        {
            return;
        }

        var guess = arg.Content.Substring(4, (arg.Content.Length)-4);
        
        var rand = new Random();
        var bot = rand.Next(1, 4);
        switch (bot)
        {
            case 1:
                await BotChoseRock(arg, guess);

                break;
            case 2:
                await BotChosePaper(arg, guess);

                break;
            case 3:
                await BotChoseScissors(arg, guess);
                break;
        }
    }

    private static async Task BotChoseScissors(SocketMessage arg, string guess)
    {
        await arg.Channel.SendMessageAsync("Bot Chose Scissors");
        switch (guess)
        {
            case "Rock":
                await arg.Channel.SendMessageAsync("Bot Lost");
                break;
            case "Paper":
                await arg.Channel.SendMessageAsync("Bot Won");
                break;
            case "Scissors":
                await arg.Channel.SendMessageAsync("Its a draw!");
                break;
        }
    }

    private static async Task BotChosePaper(SocketMessage arg, string guess)
    {
        await arg.Channel.SendMessageAsync("Bot Chose Paper");
        switch (guess)
        {
            case "Scissors":
                await arg.Channel.SendMessageAsync("Bot Lost");
                break;
            case "Rock":
                await arg.Channel.SendMessageAsync("Bot Won");
                break;
            case "Paper":
                await arg.Channel.SendMessageAsync("Its a draw!");
                break;
        }
    }

    private static async Task BotChoseRock(SocketMessage arg, string guess)
    {
        await arg.Channel.SendMessageAsync("Bot chose Rock!");
        switch (guess)
        {
            case "Paper":
                await arg.Channel.SendMessageAsync("Bot Lost");
                break;
            case "Scissors":
                await arg.Channel.SendMessageAsync("Bot Won");
                break;
            case "Rock":
                await arg.Channel.SendMessageAsync("Its a draw!");
                break;
        }
    }

    private static async Task SocketClientOnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (message.Id == 1024013681484374026 && channel.Id == 1024013310036807773 && reaction.Emote.Name == "✅")
        {
            var role = (reaction.Channel as SocketGuildChannel)!.Guild.Roles.FirstOrDefault(x => x.Name == "accepted");
            if (role is not null)
            {
                await (reaction.User.Value as IGuildUser)!.AddRoleAsync(role);
            }
        }
    }

    private async Task Startup(DiscordSocketClient socketClient, string token)
    {
        //Use to configure the SocketClient Events.
        await _commandHandler.InstallCommandsAsync();
        await _voiceChannelHandler.InstallVoiceChannelEvents();
        await _experienceHandler.InstallMessageReceivedEvents();

        await socketClient.LoginAsync(TokenType.Bot, token);
        await socketClient.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    
    private Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                              + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
        }
        else 
            Console.WriteLine($"[General/{message.Severity}] {message}");

        return Task.CompletedTask;
    }
    
}