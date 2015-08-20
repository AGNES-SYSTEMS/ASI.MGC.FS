using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.MGC.FS.Domain.Repositories
{
    public interface IRepository<TEntity> where TEntity:class
    {
        TEntity FindByID(object id);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        void Insert(TEntity entity);
        TEntity Create();
        RepositoryQuery<TEntity> Query();
    }
}
