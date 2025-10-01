using ASM_01.BusinessLayer.DTOs;
using ASM_01.BusinessLayer.Services.Abstract;
using ASM_01.DataAccessLayer.Repositories.Abstract;

namespace ASM_01.BusinessLayer.Services;

public class DealerInventoryService : IDealerInventoryService
{
    private readonly IDealerRepository _dealerRepository;
    private readonly IStockRepository _stockRepository;

    public DealerInventoryService(IDealerRepository dealerRepository, IStockRepository stockRepository)
    {
        _dealerRepository = dealerRepository;
        _stockRepository = stockRepository;
    }
    public async Task<IReadOnlyList<DealerDto>> GetDealers(CancellationToken ct = default)
    {
        var dealers = await _dealerRepository.GetAllDealersAsync();
        return dealers.Select(d => new DealerDto
        {
            DealerId = d.DealerId,
            Name = d.Name,
            Address = d.Address
        })
        .OrderBy(d => d.Name)
        .ToList();
    }

    public async Task<int> GetStockByTrimAsync(int dealerId, int evTrimId, CancellationToken ct = default)
    {
        return await _stockRepository.GetStockQuantityAsync(dealerId, evTrimId);
    }

    public async Task<IReadOnlyList<DealerStockDto>> GetDealerInventoryAsync(int dealerId, CancellationToken ct = default)
    {
        var stocks = await _stockRepository.GetStocksByDealerIdAsync(dealerId);
        return stocks.Select(s => new DealerStockDto
        {
            EvTrimId = s.EvTrimId,
            TrimName = s.EvTrim.TrimName,
            ModelName = s.EvTrim.EvModel.ModelName,
            ModelYear = s.EvTrim.ModelYear,
            Quantity = s.Quantity
        })
        .OrderBy(v => v.ModelName).ThenBy(v => v.TrimName)
        .ToList();
    }

    public async Task<IReadOnlyList<ModelStockDto>> GetInventoryByModelAsync(int dealerId, CancellationToken ct = default)
    {
        var stocks = await _stockRepository.GetStocksByDealerIdAsync(dealerId);
        return stocks
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
            .ToList();
    }
}