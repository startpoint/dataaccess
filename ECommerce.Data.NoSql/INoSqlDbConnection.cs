using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Data.NoSql
{
    public interface INoSqlDbConnection<T>: IDisposable where T : class
    {
        Task<IList<T>> Where(Func<T, bool> func);
        Task AddAsync(T value);
        Task UpdateAsync(T value);
        Task RemoveAsync(string id);
        Task Clear();
        void DropDatabase();
    }
}