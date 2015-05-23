using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.MGC.FS.Domain.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity FindByID(object ID);

    }
}
