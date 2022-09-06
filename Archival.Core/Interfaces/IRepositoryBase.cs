using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Core.Interfaces
{
    public interface IRepositoryBase<T>
    {
        Task<T> GetById(int Id);
        Task<T> Add(T entity);
    }
}
