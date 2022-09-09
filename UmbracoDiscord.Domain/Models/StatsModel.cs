using UmbracoDiscord.Domain.Entities;

namespace UmbracoDiscord.Domain.Models;

public class StatsModel
{
    public string Id { get; set; }
    
    public string ServerId { get; set; }
    
    public string? ServerName { get; set; }
    
    public string UserId { get; set; }
    
    public string? UserName { get; set; }
    
    public string Experience { get; set; }
    
    public StatsModel(Stats stats)
    {
        Id = stats.Id.ToString();
        ServerId = System.Text.Encoding.Default.GetString(stats.ServerId!);
        if (stats.ServerName is not null) ServerName = System.Text.Encoding.Default.GetString(stats.ServerName);
        UserId = System.Text.Encoding.Default.GetString(stats.UserId!);
        if(stats.UserName is not null) UserName = System.Text.Encoding.Default.GetString(stats.UserName!);
        Experience = System.Text.Encoding.Default.GetString(stats.Experience!);
    }
}