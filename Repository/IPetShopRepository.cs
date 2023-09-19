using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetShop.Repository
{
    public interface IPetShopRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(string? id, T entity);
        Task<bool> CreateEntity(T entity);
        Task<bool> UpdateEntity(string? id, T entity);
        Task<bool> DeleteEntity(string? id);
    }
}