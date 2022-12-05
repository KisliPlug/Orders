using System.Linq.Expressions;
using Bogus;
using Orders.Common;

namespace Order.Tests.Moq;

public class MoqRepository<T> : IRepository<T> where T : class, IEntity
{
    private List<T> _orders;
    private readonly Action<T, T> _update;

    public MoqRepository(Action<T, T> update)
    {
        var faker = new Faker<T>().RuleFor(x => x.Id, f => Guid.NewGuid());
        _orders = new List<T>(faker.Generate(5));
        _update = update;
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await _orders.ToArray().AsTask();
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await _orders.Where(filter.Compile()).ToArray().AsTask();
    }

    public async Task<T> GetAsync(Guid guid)

    {
        return await _orders.FirstOrDefault(x => x.Id.Equals(guid)).AsTask();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        var res = _orders.FirstOrDefault(filter.Compile()).AsTask();
        return await res;
    }

    public Task CreateAsync(T entity)
    {
        _orders.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(T item)
    {
        if (_orders.FirstOrDefault(x => x.Id == item.Id) is not { } data)
        {
            return Task.CompletedTask;
        }

        _update(data, item);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid id)
    {
        if (_orders.FirstOrDefault(x => x.Id == id) is not { } data)
        {
            return Task.CompletedTask;
        }

        _orders = _orders.Where(x => x.Id != id).ToList();
        return Task.CompletedTask;
    }
}

public static class Extensions
{
    public static Task<T> AsTask<T>(this T r)
    {
        return Task.FromResult(r);
    }
}
