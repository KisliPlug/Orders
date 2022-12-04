using System.Collections.Immutable;
using System.Linq.Expressions;
using Bogus;
using Orders.Common;

namespace Order.Tests;

public class MoqRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly List<T> _orders;

    public MoqRepository()
    {
        var faker = new Faker<T>();
        _orders = new List<T>(faker.Generate(500));
    }

    public Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyCollection<T>>(_orders);
    }

    public Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        var filterF = filter.Compile();
        var filteredResult = _orders.Where(filterF).ToImmutableList().AsTask();
        return filteredResult;
    }

    public async Task<T> GetAsync(Guid guid)

    {
        return _orders.FirstOrDefault(x => x.Id.Equals(guid))!;
    }

    public Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        var filterF = filter.Compile();
        var filteredResult = _orders.FirstOrDefault(filterF);
        return filteredResult.AsTask();
    }

    public Task CreateAsync(T entity)
    {
        _orders.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(T item)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}

public static class Extensions
{
    public static Task<T> AsTask<T>(this T r)
    {
        return Task.FromResult(r);
    }
}