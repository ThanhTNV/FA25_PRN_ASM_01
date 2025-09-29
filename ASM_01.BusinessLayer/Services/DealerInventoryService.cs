using ASM_01.BusinessLayer.DTOs;
using ASM_01.DataAccessLayer.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ASM_01.BusinessLayer.Services;

public class DealerInventoryService
{
    private readonly EVRetailsDbContext _db;

    public DealerInventoryService(EVRetailsDbContext db)
    {
        _db = db;
    }
    public async Task<IReadOnlyList<DealerDto>> GetDealers(CancellationToken ct = default)
    {
        return await _db.Dealers
            .AsNoTracking()
            .Select(d => new DealerDto
            {
                DealerId = d.DealerId,
                Name = d.Name,
                Address = d.Address
            })
            .OrderBy(d => d.Name)
            .ToListAsync(ct);
    }

    public async Task<int> GetStockByTrimAsync(int dealerId, int evTrimId, CancellationToken ct = default)
    {
        return await _db.VehicleStocks
            .Where(s => s.DealerId == dealerId && s.EvTrimId == evTrimId)
            .Select(s => (int?)s.Quantity)
            .FirstOrDefaultAsync(ct) ?? 0;
    }

    /// <summary>
    /// Flat inventory for a dealer (by trim).
    /// </summary>
    public async Task<IReadOnlyList<DealerStockDto>> GetDealerInventoryAsync(int dealerId, CancellationToken ct = default)
    {
        return await _db.VehicleStocks
            .AsNoTracking()
            .Where(s => s.DealerId == dealerId)
            .Select(s => new DealerStockDto
            {
                EvTrimId = s.EvTrimId,
                TrimName = s.EvTrim.TrimName,
                ModelName = s.EvTrim.EvModel.ModelName,
                ModelYear = s.EvTrim.ModelYear,
                Quantity = s.Quantity
            })
            .OrderBy(v => v.ModelName).ThenBy(v => v.TrimName)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Inventory grouped by model (sum trims).
    /// </summary>
    public async Task<IReadOnlyList<ModelStockDto>> GetInventoryByModelAsync(int dealerId, CancellationToken ct = default)
    {
        return await _db.VehicleStocks
            .AsNoTracking()
            .Where(s => s.DealerId == dealerId)
            .GroupBy(s => s.EvTrim.EvModel.ModelName)
            .Select(g => new ModelStockDto
            {
                ModelName = g.Key,
                TotalQuantity = g.Sum(x => x.Quantity),
                Trims = g.Select(x => new TrimQty
                {
                    EvTrimId = x.EvTrimId,
                    TrimName = x.EvTrim.TrimName,
                    Quantity = x.Quantity
                }).ToList()
            })
            .OrderBy(v => v.ModelName)
            .ToListAsync(ct);
    }
}