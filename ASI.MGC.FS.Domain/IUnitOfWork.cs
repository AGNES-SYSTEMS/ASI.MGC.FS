﻿using ASI.MGC.FS.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.MGC.FS.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        T ExtRepositoryFor<T>() where T : class;
        new void Dispose();
        void Save();
        void Truncate(string tableName);
        void Dispose(bool disposing);
        IRepository<T> Repository<T>() where T : class;
        DbContextTransaction BeginTransaction();
    }
}
