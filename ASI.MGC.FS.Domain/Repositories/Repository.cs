using ASI.MGC.FS.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ASI.MGC.FS.Domain.Repositories
{
    public class Repository<TEntity>:IRepository<TEntity> where TEntity: class
    {
        internal ASI_MGC_FSEntities dbContext;
        internal DbSet<TEntity> dbSet;

        public Repository(ASI_MGC_FSEntities _dbContext)
        { 
            if (_dbContext == null)
            {
                throw new ArgumentNullException("dbContext");
            }
            dbSet = _dbContext.Set<TEntity>();
            dbContext = _dbContext;
        }

        public virtual TEntity FindByID(object ID)
        {
            return dbSet.Find(ID);
        }

        public virtual void Update(TEntity entity)
        {
             if (dbContext.Entry(entity).State == System.Data.Entity.EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public virtual void Delete(object ID)
        {
            var entity = dbSet.Find(ID);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            if (dbContext.Entry(entity).State == System.Data.Entity.EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbContext.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
            dbSet.Remove(entity);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public virtual RepositoryQuery<TEntity> Query()
        {
            var repositoryGetFluentHelper =
                new RepositoryQuery<TEntity>(this);

            return repositoryGetFluentHelper;
        }

        internal IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, List<Expression<Func<TEntity, object>>> includeProperties = null, int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> query = dbSet;
            if (includeProperties != null)
                includeProperties.ForEach(i => { query = query.Include(i); });
            if (filter != null)
                query = query.Where(filter);
            if (orderBy != null)
                query = orderBy(query);
            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }
        public IEnumerable<TEntity> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return dbContext.Database.SqlQuery<TEntity>(query, parameters);
        }
    }
}
