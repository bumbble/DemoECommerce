using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eCommerce.SharedLibrary.Responses;

namespace eCommerce.SharedLibrary.Interface
{
    public interface IGenericInterface<T> where T : class
    {
        Task<Response> CreateAsync(T entity); 
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FindByIdAsync(int id);
        Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetByAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
    }
}
