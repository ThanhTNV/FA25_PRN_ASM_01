using ASM_01.DataAccessLayer.Entities.Warehouse;

namespace ASM_01.DataAccessLayer.Repositories.Abstract;

public interface IStockRepository
{
    Task<IEnumerable<VehicleStock>> GetAllStocksAsync();
    Task<IEnumerable<VehicleStock>> GetStocksByDealerIdAsync(int dealerId);
    Task<VehicleStock?> GetStockByDealerAndTrimAsync(int dealerId, int trimId);
    Task<VehicleStock> CreateStockAsync(VehicleStock stock);
    Task<VehicleStock> UpdateStockAsync(VehicleStock stock);
    Task<bool> DeleteStockAsync(int id);
    Task<bool> StockExistsAsync(int id);
    Task<int> GetStockQuantityAsync(int dealerId, int trimId);
    Task<bool> UpdateStockQuantityAsync(int dealerId, int trimId, int newQuantity);
    Task<bool> AddStockQuantityAsync(int dealerId, int trimId, int quantity);
}
