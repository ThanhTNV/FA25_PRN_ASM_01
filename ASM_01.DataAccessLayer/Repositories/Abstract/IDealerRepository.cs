using ASM_01.DataAccessLayer.Entities.Warehouse;

namespace ASM_01.DataAccessLayer.Repositories.Abstract;

public interface IDealerRepository
{
    Task<IEnumerable<Dealer>> GetAllDealersAsync();
    Task<Dealer?> GetDealerByIdAsync(int id);
    Task<Dealer> CreateDealerAsync(Dealer dealer);
    Task<Dealer> UpdateDealerAsync(Dealer dealer);
    Task<bool> DeleteDealerAsync(int id);
    Task<bool> DealerExistsAsync(int id);
    Task<bool> DealerNameExistsAsync(string name, int? excludeId = null);
}
