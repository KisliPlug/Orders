using System.Linq.Expressions;
using MongoDB.Driver;

namespace Orders.Common.MongoDb;

public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> _dbCollection;

    private readonly FilterDefinitionBuilder<T> _filterDefinitionBuilder = Builders<T>.Filter;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await _dbCollection.Find(_filterDefinitionBuilder.Empty).ToListAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbCollection.Find(filter).ToListAsync();
    }

    public async Task<T> GetAsync(Guid guid)
    {
        var filter = _filterDefinitionBuilder.Eq(x => x.Id, guid);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        await _dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T item)
    {
        var filter = _filterDefinitionBuilder.Eq(x => x.Id, item.Id);
        if (await _dbCollection.Find(filter).FirstOrDefaultAsync() is not { } existing)
        {
            throw new ArgumentNullException(nameof(item));
        }

        await _dbCollection.ReplaceOneAsync(filter, item);
    }

    public async Task RemoveAsync(Guid id)
    {
        var filter = _filterDefinitionBuilder.Eq(x => x.Id, id);
        await _dbCollection.DeleteManyAsync(filter);
    }
}
