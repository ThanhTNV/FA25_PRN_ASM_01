using ASM_01.BusinessLayer.DTOs;
using ASM_01.DataAccessLayer.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ASM_01.BusinessLayer.Services;

public class VehicleService(EVRetailsDbContext dbContext)
{
    public async Task<IEnumerable<VehicleDto>> GetAllVehicles()
    {
        var trims = await dbContext.EvTrims
    .Include(t => t.EvModel)
    .Include(t => t.Prices)
    .ToListAsync();

        // Step 1: Project base info (without specs)
        var vehicles = trims.Select(t =>
        {
            var latestPrice = t.Prices
                .OrderByDescending(p => p.EffectiveDate)
                .FirstOrDefault();

            return new VehicleDto
            {
                VehicleId = t.EvTrimId,
                ModelName = t.EvModel.ModelName,
                TrimName = t.TrimName,
                Price = latestPrice?.ListedPrice ?? 0,
                EffectiveDate = latestPrice?.EffectiveDate ?? DateTime.MinValue,
                Specifications = new Dictionary<string, string>() // fill later
            };
        }).ToList();

        // Step 2: Load specifications separately
        var specs = await dbContext.TrimSpecs
            .Include(ts => ts.Spec)
            .ToListAsync();

        // Step 3: Merge specs into vehicles
        foreach (var v in vehicles)
        {
            var vehicleSpecs = specs
                .Where(ts => ts.EvTrimId == v.VehicleId)
                .ToDictionary(
                    ts => ts.Spec.SpecName,
                    ts => ts.Value + (ts.Spec.Unit != null ? $" {ts.Spec.Unit}" : "")
                );

            v.Specifications = vehicleSpecs;
        }
        return vehicles;
    }

    public async Task<VehicleDto?> GetVehicleById(int id)
    {
        // 1. Load trim and model
        var trim = await dbContext.EvTrims
            .Include(t => t.EvModel)
            .FirstOrDefaultAsync(t => t.EvTrimId == id);

        if (trim == null)
            return null;

        // 2. Load latest price
        var latestPrice = await dbContext.TrimPrices
            .Where(p => p.EvTrimId == id)
            .OrderByDescending(p => p.EffectiveDate)
            .FirstOrDefaultAsync();

        // 3. Load specifications
        var specs = await dbContext.TrimSpecs
            .Where(ts => ts.EvTrimId == id)
            .Include(ts => ts.Spec)
            .ToDictionaryAsync(
                ts => ts.Spec.SpecName,
                ts => ts.Value + (ts.Spec.Unit != null ? $" {ts.Spec.Unit}" : "")
            );

        // 4. Map into DTO
        return new VehicleDto
        {
            VehicleId = trim.EvTrimId,
            ModelName = trim.EvModel.ModelName,
            TrimName = trim.TrimName,
            Price = latestPrice?.ListedPrice ?? 0,
            EffectiveDate = latestPrice?.EffectiveDate ?? DateTime.MinValue,
            Specifications = specs
        };
    }


    public async Task<IEnumerable<VehicleDto>> SearchVehicles(string keyword)
    {
        keyword = keyword.ToLower();

        // 1. Get matching trims + models
        var trims = await dbContext.EvTrims
            .Include(t => t.EvModel)
            .Where(t =>
                t.EvModel.ModelName.ToLower().Contains(keyword) ||
                t.TrimName.ToLower().Contains(keyword))
            .ToListAsync();

        var trimIds = trims.Select(t => t.EvTrimId).ToList();

        // 2. Get latest prices for these trims
        var prices = await dbContext.TrimPrices
            .Where(p => trimIds.Contains(p.EvTrimId))
            .GroupBy(p => p.EvTrimId)
            .Select(g => g.OrderByDescending(p => p.EffectiveDate).FirstOrDefault())
            .ToListAsync();

        var priceDict = prices
            .Where(p => p != null)
            .ToDictionary(p => p!.EvTrimId, p => p);

        // 3. Get specs for these trims
        var trimSpecs = await dbContext.TrimSpecs
            .Where(ts => trimIds.Contains(ts.EvTrimId))
            .Include(ts => ts.Spec)
            .ToListAsync();

        var specsDict = trimSpecs
            .GroupBy(ts => ts.EvTrimId)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(
                    ts => ts.Spec.SpecName,
                    ts => ts.Value + (ts.Spec.Unit != null ? $" {ts.Spec.Unit}" : "")
                )
            );

        // 4. Map DTOs
        var result = trims.Select(t =>
        {
            var latestPrice = priceDict.ContainsKey(t.EvTrimId) ? priceDict[t.EvTrimId] : null;
            var specs = specsDict.ContainsKey(t.EvTrimId) ? specsDict[t.EvTrimId] : new Dictionary<string, string>();

            return new VehicleDto
            {
                VehicleId = t.EvTrimId,
                ModelName = t.EvModel.ModelName,
                TrimName = t.TrimName,
                Price = latestPrice?.ListedPrice ?? 0,
                EffectiveDate = latestPrice?.EffectiveDate ?? DateTime.MinValue,
                Specifications = specs
            };
        });

        return result;
    }


    public async Task<IEnumerable<VehicleComparisonDto>> CompareVehicles(int[] vehicleIds)
    {
        // 1. Load trims + models
        var trims = await dbContext.EvTrims
            .Include(t => t.EvModel)
            .Where(t => vehicleIds.Contains(t.EvTrimId))
            .ToListAsync();

        // 2. Load latest prices for these trims
        var prices = await dbContext.TrimPrices
            .Where(p => vehicleIds.Contains(p.EvTrimId))
            .GroupBy(p => p.EvTrimId)
            .Select(g => g.OrderByDescending(p => p.EffectiveDate).FirstOrDefault())
            .ToListAsync();

        var priceDict = prices
            .Where(p => p != null)
            .ToDictionary(p => p!.EvTrimId, p => p);

        // 3. Load specs for these trims
        var trimSpecs = await dbContext.TrimSpecs
            .Where(ts => vehicleIds.Contains(ts.EvTrimId))
            .Include(ts => ts.Spec)
            .ToListAsync();

        var specsDict = trimSpecs
            .GroupBy(ts => ts.EvTrimId)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(
                    ts => ts.Spec.SpecName,
                    ts => ts.Value + (ts.Spec.Unit != null ? $" {ts.Spec.Unit}" : "")
                )
            );

        // 4. Map DTOs
        var result = trims.Select(t =>
        {
            var latestPrice = priceDict.ContainsKey(t.EvTrimId) ? priceDict[t.EvTrimId] : null;
            var specs = specsDict.ContainsKey(t.EvTrimId) ? specsDict[t.EvTrimId] : new Dictionary<string, string>();

            return new VehicleComparisonDto
            {
                VehicleId = t.EvTrimId,
                ModelName = t.EvModel.ModelName,
                TrimName = t.TrimName,
                Price = latestPrice?.ListedPrice ?? 0,
                EffectiveDate = latestPrice?.EffectiveDate ?? DateTime.MinValue,
                Specifications = specs
            };
        });

        return result;
    }

}
