using Microsoft.EntityFrameworkCore.Storage;
using PatiantMicroService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;

        // =========================
        // 🔥 Transaction Support
        // =========================

        //Task<IDbContextTransaction> BeginTransactionAsync();

        //Task CommitAsync();

        //Task RollbackAsync();

    }
}
