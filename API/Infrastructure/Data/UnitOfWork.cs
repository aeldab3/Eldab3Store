using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _context;
        private Hashtable _repositories; //To store repository instances. 

        public UnitOfWork(StoreContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();

            }
            // Gets the name of the entity type.
            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                //Defines the type of the generic repository 
                var repositoryType = typeof(GenericRepository<>);
                //Create an instance of the generic repository for the specific entity type
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            //it returns the repository instance from the hashtable
            return (IGenericRepository<TEntity>)_repositories[type];
        }
    }
}
