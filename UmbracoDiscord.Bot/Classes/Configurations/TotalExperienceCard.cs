using Konstrukt.Configuration.Cards;
using UmbracoDiscord.Domain.Context;

namespace UmbracoDiscord.Bot.Classes.Configurations;

// Example
public class TotalExperienceCard : KonstruktCard
{
    public override string Alias => "totalExperience";
    public override string Name => "Total Experience";
    public override string Icon => "icon-trophy";
    public override string Color => "green";
    public override string Suffix => "Exp";

    public override object GetValue(object parentId = null)
    {
        using var context = new UmbracoDiscordDbContext();

        return context.Stats.Sum(x => x.Experience);
    }
}