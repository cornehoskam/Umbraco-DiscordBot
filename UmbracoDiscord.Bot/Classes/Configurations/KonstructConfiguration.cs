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
                        .AddField(p => p.ServerName).SetHeading("Server")
                        .AddField(p => p.UserName).SetHeading("User")
                        .AddField(p => p.Experience).SetHeading("Experience")
                    )
                )
            )
        );
    }
}

public class StatsRepository : KonstruktRepository<StatsModel, int>
{
    private readonly UmbracoDiscordDbContext _dbContext;

    public StatsRepository(KonstruktRepositoryContext context, UmbracoDiscordDbContext dbContext) : base(context)
    {
        _dbContext = dbContext;
    }

    protected override int GetIdImpl(StatsModel entity)
    {
        return int.Parse(entity.Id);
    }

    protected override StatsModel GetImpl(int id)
    {
        return new StatsModel(_dbContext.Stats.First(x => x.Id == id));
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
        return _dbContext.Stats.Select(x => new StatsModel(x));
    }

    protected override PagedResult<StatsModel> GetPagedImpl(int pageNumber, int pageSize, Expression<Func<StatsModel, bool>> whereClause, Expression<Func<StatsModel, object>> orderBy,
        SortDirection orderByDirection)
    {
        return new PagedResult<StatsModel>(_dbContext.Stats.Count(), pageNumber, pageSize)
        {
            Items = _dbContext.Stats.Skip(pageSize * (pageNumber-1)).Take(pageSize).ToList().Select(x => new StatsModel(x))
        };
    }

    protected override long GetCountImpl(Expression<Func<StatsModel, bool>> whereClause)
    {
        return _dbContext.Stats.Count();
    }
}