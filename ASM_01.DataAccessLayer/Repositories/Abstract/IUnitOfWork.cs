namespace ASM_01.DataAccessLayer.Repositories.Abstract;

public interface IUnitOfWork : IDisposable
{
    IVehicleRepository Vehicles { get; }
    IDealerRepository Dealers { get; }
    IStockRepository Stocks { get; }
    IDistributionRequestRepository DistributionRequests { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
