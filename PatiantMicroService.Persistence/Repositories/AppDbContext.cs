using Microsoft.EntityFrameworkCore.Storage;
using PatiantMicroService.Persistence.Data.DbContexts;
using PatiantMicroService.ServicesAbstraction.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Persistence.Repositories
{
    public class AppDbContext : IAppDbContext
    {
        private readonly PatientDbContext _context;
        private IDbContextTransaction? _transaction;

        public AppDbContext(PatientDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync()
            => _context.SaveChangesAsync();

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            await _context.SaveChangesAsync();
            await _transaction!.CommitAsync();
            await _transaction.DisposeAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }
    }
}
