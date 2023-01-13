using Konstrukt.Configuration.Cards;
using UmbracoDiscord.Domain.Context;

namespace UmbracoDiscord.Bot.Classes.Configurations;

// Example
public class TotalServers : KonstruktCard
{
    public override string Alias => "totalServers";
    public override string Name => "Total Servers";
    public override string Icon => "icon-stacked-disks";
    public override string Color => "purple";

    public override object GetValue(object parentId = null)
    {
        using var context = new UmbracoDiscordDbContext();

        return context.Stats.ToList().DistinctBy(x => x.ServerId).Count();
    }
}