namespace Emm.Domain.Repositories;

public interface IQueryContext
{
    IQueryable<T> Query<T>() where T : class;
}