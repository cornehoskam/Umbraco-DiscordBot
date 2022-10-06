using System.Linq.Expressions;
using Konstrukt;
using Konstrukt.Configuration.Builders;
using Konstrukt.Persistence;
using Umbraco.Cms.Core.Models;
using UmbracoDiscord.Domain.Context;
using UmbracoDiscord.Domain.Entities;

namespace UmbracoDiscord.Bot.Classes.Configurations;

public static class KonstructConfiguration
{
    public static void GetConfig(KonstruktConfigBuilder cfg)
    {
        cfg.AddSection("Repositories", sectionConfig =>
            sectionConfig.Tree(treeConfig => treeConfig
                .AddCollection<Stats>(x => x.Id, "Stat", "Stats", "A person entity", "icon-umb-users", "icon-umb-users", collectionConfig => collectionConfig
                    .SetRepositoryType<StatsRepository>()
                    .DisableCreate()
                    .SetNameProperty(x => x.UserName)
                    .ListView(listViewConfig => listViewConfig
                        .AddField(p => p.Id).SetHeading("ID")
                        .AddField(p => p.ServerName).SetHeading("Server Name")
                        .AddField(p => p.Experience).SetHeading("Experience")
                    )
                )
            )
        );
    }
}

public class StatsRepository : KonstruktRepository<Stats, int>
{
    private readonly UmbracoDiscordDbContext _dbContext;

    public StatsRepository(KonstruktRepositoryContext context, UmbracoDiscordDbContext dbContext) : base(context)
    {
        _dbContext = dbContext;
    }

    protected override int GetIdImpl(Stats entity)
    {
        return Int32.Parse(entity.Id.ToString());
    }

    protected override Stats GetImpl(int id)
    {
        return _dbContext.Stats.First(x => x.Id == id);
    }

    protected override Stats SaveImpl(Stats entity)
    {
        throw new NotImplementedException();
    }

    protected override void DeleteImpl(int id)
    {
        throw new NotImplementedException();

    }

    protected override IEnumerable<Stats> GetAllImpl(Expression<Func<Stats, bool>> whereClause, Expression<Func<Stats, object>> orderBy, SortDirection orderByDirection)
    {
        if (orderByDirection == SortDirection.Ascending)
        {
            return _dbContext.Stats.Where(whereClause).OrderBy(orderBy);
        }
        else
        {
            return _dbContext.Stats.Where(whereClause).OrderByDescending(orderBy);
        }
    }

    protected override PagedResult<Stats> GetPagedImpl(int pageNumber, int pageSize, Expression<Func<Stats, bool>>? whereClause, Expression<Func<Stats, object>>? orderBy,
        SortDirection orderByDirection)
    {
        var items = _dbContext.Stats.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        if (whereClause is not null)
        {
            items = items.Where(whereClause);
        }

        if (orderBy is not null && orderByDirection == SortDirection.Ascending)
        {
            items = items.OrderBy(orderBy);
        }
        else if (orderBy is not null && orderByDirection == SortDirection.Descending)
        {
            items = items.OrderByDescending(orderBy);
        }
        
        return new PagedResult<Stats>(_dbContext.Stats.Count(), pageNumber, pageSize)
        {
            Items = items
        };
    }

    protected override long GetCountImpl(Expression<Func<Stats, bool>> whereClause)
    {
        return _dbContext.Stats.Count();
    }
}