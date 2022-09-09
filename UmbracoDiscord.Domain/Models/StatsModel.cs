using UmbracoDiscord.Domain.Entities;

namespace UmbracoDiscord.Domain.Models;

public class StatsModel
{
    public string Id { get; set; }
    
    public string ServerId { get; set; }
    
    public string UserId { get; set; }
    
    public string Experience { get; set; }
    
    public StatsModel(Stats stats)
    {
        Id = stats.Id.ToString();
        ServerId = System.Text.Encoding.Default.GetString(stats.ServerId!);
        UserId = System.Text.Encoding.Default.GetString(stats.UserId!);
        Experience = System.Text.Encoding.Default.GetString(stats.Experience!);
    }
}