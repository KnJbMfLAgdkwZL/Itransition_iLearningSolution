using System.Linq.Expressions;
using DataAccess.Dto;

namespace DataAccess.Interfaces;

public interface IGeneralRepository<T>
{
    Task<T> AddAsync(T model, CancellationToken token);
    Task<T> AddIfNotExistAsync(Expression<Func<T, bool>> condition, T model, CancellationToken token);
    Task<T> AddOrUpdateAsync(Expression<Func<T, bool>> condition, T model, CancellationToken token);
    Task<T> RemoveAsync(T model, CancellationToken token);
    Task<T> RemoveIfExistAsync(Expression<Func<T, bool>> condition, CancellationToken token);
    Task<T> UpdateAsync(T model, CancellationToken token);
    Task<List<T>> GetAllAsync(CancellationToken token);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> condition, CancellationToken token);

    Task<List<T>> GetAllAsync<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy,
        CancellationToken token);

    Task<List<T>> GetAllDescendingAsync<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy,
        CancellationToken token);

    Task<List<T>> GetAllIncludeAsync<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> include,
        CancellationToken token);

    Task<List<T>> GetAllIncludeManyAsync<TKey>(Expression<Func<T, bool>> condition,
        IEnumerable<Expression<Func<T, TKey>>> includes, CancellationToken token);

    Task<List<T>> GetAllIncludeManyAsync<TKey>(Expression<Func<T, bool>> condition,
        IEnumerable<Expression<Func<T, TKey>>> includes, Expression<Func<T, TKey>> orderBy, CancellationToken token);

    Task<PageResult<T>> GetAllIncludeManyAsync<TKey>(Expression<Func<T, bool>> condition,
        IEnumerable<Expression<Func<T, TKey>>> includes, Expression<Func<T, TKey>> orderBy, int page, int pageSize,
        CancellationToken token);

    Task<List<T>> GetAllIncludeManyDescendingAsync<TKey>(Expression<Func<T, bool>> condition,
        IEnumerable<Expression<Func<T, TKey>>> includes, Expression<Func<T, TKey>> orderBy, CancellationToken token);

    Task<T?> GetOneAsync(Expression<Func<T, bool>> condition, CancellationToken token);

    Task<T?> GetOneIncludeAsync<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> include,
        CancellationToken token);

    Task<T?> GetOneIncludeManyAsync<TKey>(Expression<Func<T, bool>> condition,
        IEnumerable<Expression<Func<T, TKey>>> includes, CancellationToken token);

    Task<double?> AverageAsync(Expression<Func<T, bool>> condition, Expression<Func<T, int?>> selector,
        CancellationToken token);

    void DetachEntity(T model);
    Task<List<T>> FullTextSearchQueryAsync(string search, CancellationToken token);
}