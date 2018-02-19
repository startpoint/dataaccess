using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Data.RepositoryStore
{
    public interface IRepositoryStore<T> : IDisposable where T : class, new()
    {
        string Connect(ConnectionOptions connectionOptions);
        Task<T> AddAsync(T value);
        Task<T> UpdateAsync(T value);
        Task<T> RemoveAsync(T value);
        Task<IEnumerable<T>> SearchAsync(Func<T, bool> filter);
        Task<T> SearchASingleItemAsync(Func<T, bool> filter);
    }
}
