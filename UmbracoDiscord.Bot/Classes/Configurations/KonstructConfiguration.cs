using System.Linq.Expressions;
using Konstrukt;
using Konstrukt.Configuration.Builders;
using Konstrukt.Persistence;
using Umbraco.Cms.Core.Models;
using UmbracoDiscord.Domain.Context;
using UmbracoDiscord.Domain.Models;

namespace UmbracoDiscord.Bot.Classes.Configurations;

public static class KonstructConfiguration
{
    public static void GetConfig(KonstruktConfigBuilder cfg)
    {
        cfg.AddSection("Repositories", sectionConfig =>
            sectionConfig.Tree(treeConfig => treeConfig
                .AddCollection<StatsModel>(x => x.Id, "Stat", "Stats", "A person entity", "icon-umb-users", "icon-umb-users", collectionConfig => collectionConfig
                    .SetRepositoryType<StatsRepository>()
                    .DisableCreate()
                    .SetNameProperty(x => x.Id)
                    .ListView(listViewConfig => listViewConfig
                        .AddField(p => p.ServerId).SetHeading("Server ID")
                        .AddField(p => p.UserId).SetHeading("User ID")
                        .AddField(p => p.Experience).SetHeading("Experience")
                    )
                )
            )
        );
    }
}

public class StatsRepository : KonstruktRepository<StatsModel, int>
{
    public StatsRepository(KonstruktRepositoryContext context) : base(context)
    {
    }

    protected override int GetIdImpl(StatsModel entity)
    {
        return int.Parse(entity.Id);
    }

    protected override StatsModel GetImpl(int id)
    {
        using var context = new UmbracoDiscordDbContext();
        return new StatsModel(context.Stats.First(x => x.Id == id));
    }

    protected override StatsModel SaveImpl(StatsModel entity)
    {
        throw new NotImplementedException();
    }

    protected override void DeleteImpl(int id)
    {
        throw new NotImplementedException();

    }

    protected override IEnumerable<StatsModel> GetAllImpl(Expression<Func<StatsModel, bool>> whereClause, Expression<Func<StatsModel, object>> orderBy, SortDirection orderByDirection)
    {
        using var context = new UmbracoDiscordDbContext();
        return context.Stats.Select(x => new StatsModel(x));
    }

    protected override PagedResult<StatsModel> GetPagedImpl(int pageNumber, int pageSize, Expression<Func<StatsModel, bool>> whereClause, Expression<Func<StatsModel, object>> orderBy,
        SortDirection orderByDirection)
    {
        using var context = new UmbracoDiscordDbContext();
        return new PagedResult<StatsModel>(context.Stats.Count(), pageNumber, pageSize)
        {
            Items = context.Stats.Skip(pageSize * (pageNumber-1)).Take(pageSize).ToList().Select(x => new StatsModel(x))
        };
    }

    protected override long GetCountImpl(Expression<Func<StatsModel, bool>> whereClause)
    {
        using var context = new UmbracoDiscordDbContext();
        return context.Stats.Count();
    }
}