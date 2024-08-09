using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //This method is a generic method that returns a repository for a given entity type TEntity.
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

        //used to commit all the changes made within the unit of work to the database
        Task<int> Complete();
    }
}
