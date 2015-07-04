using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.MGC.FS.DataAccess;
using ASI.MGC.FS.Domain.Repositories;

namespace ASI.MGC.FS.Domain
{
    public class UnitOfWork: IUnitOfWork
    {
        private bool _disposed;
        private Hashtable _repositories;
        private ASI_MGC_FSEntities _dbContext;

        public UnitOfWork ()
        {
            ASI_MGC_FSEntities dbContext = new ASI_MGC_FSEntities();

            if (dbContext == null)
                throw new ArgumentNullException("dbContext");
            _dbContext = dbContext;

        }
        public T ExtRepositoryFor<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), _dbContext);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _dbContext.Dispose();
            _disposed = true;
        }

        public Repositories.IRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);
                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                            .MakeGenericType(typeof(T)), _dbContext);
                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }
    }
}
