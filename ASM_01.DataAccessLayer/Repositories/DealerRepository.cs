using ASM_01.DataAccessLayer.Entities.Warehouse;
using ASM_01.DataAccessLayer.Persistence;
using ASM_01.DataAccessLayer.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ASM_01.DataAccessLayer.Repositories;

public class DealerRepository : IDealerRepository
{
    private readonly EVRetailsDbContext _context;

    public DealerRepository(EVRetailsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Dealer>> GetAllDealersAsync()
    {
        return await _context.Dealers
            .Include(d => d.VehicleStocks)
            .ThenInclude(vs => vs.EvTrim)
            .ThenInclude(t => t.EvModel)
            .Include(d => d.DistributionRequests)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Dealer?> GetDealerByIdAsync(int id)
    {
        return await _context.Dealers
            .Include(d => d.VehicleStocks)
            .ThenInclude(vs => vs.EvTrim)
            .ThenInclude(t => t.EvModel)
            .Include(d => d.DistributionRequests)
            .FirstOrDefaultAsync(d => d.DealerId == id);
    }

    public async Task<Dealer> CreateDealerAsync(Dealer dealer)
    {
        _context.Dealers.Add(dealer);
        await _context.SaveChangesAsync();
        return dealer;
    }

    public async Task<Dealer> UpdateDealerAsync(Dealer dealer)
    {
        _context.Dealers.Update(dealer);
        await _context.SaveChangesAsync();
        return dealer;
    }

    public async Task<bool> DeleteDealerAsync(int id)
    {
        var dealer = await _context.Dealers.FindAsync(id);
        if (dealer == null) return false;

        _context.Dealers.Remove(dealer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DealerExistsAsync(int id)
    {
        return await _context.Dealers.AnyAsync(d => d.DealerId == id);
    }

    public async Task<bool> DealerNameExistsAsync(string name, int? excludeId = null)
    {
        var query = _context.Dealers.Where(d => d.Name == name);
        if (excludeId.HasValue)
        {
            query = query.Where(d => d.DealerId != excludeId.Value);
        }
        return await query.AnyAsync();
    }
}
