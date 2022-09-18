using UmbracoDiscord.Domain.Entities;

namespace UmbracoDiscord.Domain.Models;

public class StatsModel
{
    public string Id { get; set; }
    
    public string ServerId { get; set; }
    
    public string? ServerName { get; set; }
    
    public string UserId { get; set; }
    
    public string? UserName { get; set; }
    
    public long Experience { get; set; }
    
    public StatsModel(Stats stats)
    {
        Id = stats.Id.ToString();
        ServerId = stats.ServerId;
        ServerName = stats.ServerName;
        UserId = stats.UserId;
        UserName = stats.UserName;
        Experience = stats.Experience;
    }
}