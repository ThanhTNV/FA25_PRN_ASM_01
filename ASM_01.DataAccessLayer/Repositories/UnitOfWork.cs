using ASM_01.DataAccessLayer.Persistence;
using ASM_01.DataAccessLayer.Repositories.Abstract;
using Microsoft.EntityFrameworkCore.Storage;

namespace ASM_01.DataAccessLayer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EVRetailsDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(EVRetailsDbContext context)
    {
        _context = context;
        Vehicles = new VehicleRepository(_context);
        Dealers = new DealerRepository(_context);
        Stocks = new StockRepository(_context);
        DistributionRequests = new DistributionRequestRepository(_context);
    }

    public IVehicleRepository Vehicles { get; }
    public IDealerRepository Dealers { get; }
    public IStockRepository Stocks { get; }
    public IDistributionRequestRepository DistributionRequests { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
