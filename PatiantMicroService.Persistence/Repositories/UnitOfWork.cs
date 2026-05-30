using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PatiantMicroService.Domain.Contracts;
using PatiantMicroService.Domain.Entities;
using PatiantMicroService.Persistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PatientDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        private readonly Dictionary<Type, object> _repositories = [];

        public UnitOfWork(PatientDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
        {
            var entityType = typeof(TEntity);

            if (_repositories.TryGetValue(entityType, out object? repository))
                return (IGenericRepository<TEntity, TKey>)repository;

            var newRepository = new GenericRepository<TEntity, TKey>(_dbContext);
            _repositories[entityType] = newRepository;

            return newRepository;
        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        // =========================
        // 🔥 Transaction Handling
        // =========================

        //public async Task<IDbContextTransaction> BeginTransactionAsync()
        //{
        //    _transaction = await _dbContext.Database.BeginTransactionAsync();
        //    return _transaction;
        //}

        //public async Task CommitAsync()
        //{
        //    try
        //    {
        //        await _dbContext.SaveChangesAsync();

        //        if (_transaction != null)
        //        {
        //            await _transaction.CommitAsync();
        //            await _transaction.DisposeAsync();
        //            _transaction = null;
        //        }
        //    }
        //    catch
        //    {
        //        await RollbackAsync();
        //        throw;
        //    }
        //}

        //public async Task RollbackAsync()
        //{
        //    if (_transaction != null)
        //    {
        //        await _transaction.RollbackAsync();
        //        await _transaction.DisposeAsync();
        //        _transaction = null;
        //    }
        //}
    }
}
