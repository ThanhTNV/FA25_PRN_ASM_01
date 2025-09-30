using ASM_01.DataAccessLayer.Entities.Warehouse;
using ASM_01.DataAccessLayer.Persistence;
using ASM_01.DataAccessLayer.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ASM_01.DataAccessLayer.Repositories;

public class StockRepository : IStockRepository
{
    private readonly EVRetailsDbContext _context;

    public StockRepository(EVRetailsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VehicleStock>> GetAllStocksAsync()
    {
        return await _context.VehicleStocks
            .Include(s => s.Dealer)
            .Include(s => s.EvTrim)
            .ThenInclude(t => t.EvModel)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<VehicleStock>> GetStocksByDealerIdAsync(int dealerId)
    {
        return await _context.VehicleStocks
            .Include(s => s.Dealer)
            .Include(s => s.EvTrim)
            .ThenInclude(t => t.EvModel)
            .Where(s => s.DealerId == dealerId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<VehicleStock?> GetStockByDealerAndTrimAsync(int dealerId, int trimId)
    {
        return await _context.VehicleStocks
            .Include(s => s.Dealer)
            .Include(s => s.EvTrim)
            .ThenInclude(t => t.EvModel)
            .FirstOrDefaultAsync(s => s.DealerId == dealerId && s.EvTrimId == trimId);
    }

    public async Task<VehicleStock> CreateStockAsync(VehicleStock stock)
    {
        _context.VehicleStocks.Add(stock);
        await _context.SaveChangesAsync();
        return stock;
    }

    public async Task<VehicleStock> UpdateStockAsync(VehicleStock stock)
    {
        _context.VehicleStocks.Update(stock);
        await _context.SaveChangesAsync();
        return stock;
    }

    public async Task<bool> DeleteStockAsync(int id)
    {
        var stock = await _context.VehicleStocks.FindAsync(id);
        if (stock == null) return false;

        _context.VehicleStocks.Remove(stock);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> StockExistsAsync(int id)
    {
        return await _context.VehicleStocks.AnyAsync(s => s.VehicleStockId == id);
    }

    public async Task<int> GetStockQuantityAsync(int dealerId, int trimId)
    {
        var stock = await _context.VehicleStocks
            .FirstOrDefaultAsync(s => s.DealerId == dealerId && s.EvTrimId == trimId);
        return stock?.Quantity ?? 0;
    }

    public async Task<bool> UpdateStockQuantityAsync(int dealerId, int trimId, int newQuantity)
    {
        var stock = await _context.VehicleStocks
            .FirstOrDefaultAsync(s => s.DealerId == dealerId && s.EvTrimId == trimId);

        if (stock == null) return false;

        stock.Quantity = newQuantity;
        _context.VehicleStocks.Update(stock);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddStockQuantityAsync(int dealerId, int trimId, int quantity)
    {
        var stock = await _context.VehicleStocks
            .FirstOrDefaultAsync(s => s.DealerId == dealerId && s.EvTrimId == trimId);

        if (stock == null)
        {
            // Create new stock record
            stock = new VehicleStock
            {
                DealerId = dealerId,
                EvTrimId = trimId,
                Quantity = quantity
            };
            _context.VehicleStocks.Add(stock);
        }
        else
        {
            // Update existing stock
            stock.Quantity += quantity;
            _context.VehicleStocks.Update(stock);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
