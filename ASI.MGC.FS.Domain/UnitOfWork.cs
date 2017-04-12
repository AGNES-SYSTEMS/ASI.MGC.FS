using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.MGC.FS.DataAccess;
using ASI.MGC.FS.Domain.Repositories;
using System.Data.Common;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace ASI.MGC.FS.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private Hashtable _repositories;
        private ASI_MGC_FSEntities _dbContext;
        DbContextTransaction Transaction;
        public UnitOfWork()
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
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        public void Truncate(string tableName)
        {
            string command = "TRUNCATE TABLE[" + tableName + "]";
            _dbContext.Database.ExecuteSqlCommand(command);
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
        public DbContextTransaction BeginTransaction()
        {
            if (null == Transaction)
            {
                if (_dbContext.Database.Connection.State != ConnectionState.Open)
                {
                    _dbContext.Database.Connection.Open();
                }
                this.Transaction = _dbContext.Database.BeginTransaction();
            }
            return Transaction;
        }
    }
}
