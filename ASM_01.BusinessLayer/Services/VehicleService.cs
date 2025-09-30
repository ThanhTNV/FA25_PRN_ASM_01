using ASM_01.BusinessLayer.DTOs;
using ASM_01.BusinessLayer.Services.Abstract;
using ASM_01.DataAccessLayer.Entities.VehicleModels;
using ASM_01.DataAccessLayer.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ASM_01.BusinessLayer.Services;

public class VehicleService(EVRetailsDbContext _db) : IVehicleService
{
    public async Task<IEnumerable<VehicleDto>> GetAllVehicles()
    {
        var trims = await _db.EvTrims
            .Include(t => t.EvModel)
            .Include(t => t.Prices)
            .ToListAsync();

        var vehicles = trims.Select(t =>
        {
            var latestPrice = t.Prices
                .OrderByDescending(p => p.EffectiveDate)
                .FirstOrDefault();

            return new VehicleDto
            {
                ModelId = t.EvModelId,
                VehicleId = t.EvTrimId,
                ModelName = t.EvModel.ModelName,
                TrimName = t.TrimName,
                TrimId = t.EvTrimId,
                ModelYear = t.ModelYear,
                Description = t.Description,
                Status = t.EvModel.Status.ToString(),
                Price = latestPrice?.ListedPrice ?? 0,
                EffectiveDate = latestPrice?.EffectiveDate ?? DateTime.MinValue,
                Specifications = new Dictionary<string, string>()
            };
        }).ToList();

        var specs = await _db.TrimSpecs
            .Include(ts => ts.Spec)
            .ToListAsync();

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
        var trim = await _db.EvTrims
            .Include(t => t.EvModel)
            .FirstOrDefaultAsync(t => t.EvTrimId == id);

        if (trim == null)
            return null;

        var latestPrice = await _db.TrimPrices
            .Where(p => p.EvTrimId == id)
            .OrderByDescending(p => p.EffectiveDate)
            .FirstOrDefaultAsync();

        var specs = await _db.TrimSpecs
            .Where(ts => ts.EvTrimId == id)
            .Include(ts => ts.Spec)
            .ToDictionaryAsync(
                ts => ts.Spec.SpecName,
                ts => ts.Value + (ts.Spec.Unit != null ? $" {ts.Spec.Unit}" : "")
            );

        return new VehicleDto
        {
            ModelId = trim.EvModelId,
            VehicleId = trim.EvTrimId,
            ModelName = trim.EvModel.ModelName,
            TrimName = trim.TrimName,
            Description = trim.Description,
            Status = trim.EvModel.Status.ToString(),
            Price = latestPrice?.ListedPrice ?? 0,
            EffectiveDate = latestPrice?.EffectiveDate ?? DateTime.MinValue,
            Specifications = specs
        };
    }

    public async Task<IEnumerable<VehicleDto>> SearchVehicles(string keyword)
    {
        keyword = keyword.ToLower();

        var trims = await _db.EvTrims
            .Include(t => t.EvModel)
            .Where(t =>
                t.EvModel.ModelName.ToLower().Contains(keyword) ||
                t.TrimName.ToLower().Contains(keyword))
            .ToListAsync();

        var trimIds = trims.Select(t => t.EvTrimId).ToList();

        var prices = await _db.TrimPrices
            .Where(p => trimIds.Contains(p.EvTrimId))
            .GroupBy(p => p.EvTrimId)
            .Select(g => g.OrderByDescending(p => p.EffectiveDate).FirstOrDefault())
            .ToListAsync();

        var priceDict = prices
            .Where(p => p != null)
            .ToDictionary(p => p!.EvTrimId, p => p);

        var trimSpecs = await _db.TrimSpecs
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

        var result = trims.Select(t =>
        {
            var latestPrice = priceDict.ContainsKey(t.EvTrimId) ? priceDict[t.EvTrimId] : null;
            var specs = specsDict.ContainsKey(t.EvTrimId) ? specsDict[t.EvTrimId] : new Dictionary<string, string>();

            return new VehicleDto
            {
                ModelId = t.EvModelId,
                VehicleId = t.EvTrimId,
                ModelName = t.EvModel.ModelName,
                TrimName = t.TrimName,
                Description = t.Description,
                Status = t.EvModel.Status.ToString(),
                ModelYear = t.ModelYear,
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
        var trims = await _db.EvTrims
            .Include(t => t.EvModel)
            .Where(t => vehicleIds.Contains(t.EvTrimId))
            .ToListAsync();

        // 2. Load latest prices for these trims
        var prices = await _db.TrimPrices
            .Where(p => vehicleIds.Contains(p.EvTrimId))
            .GroupBy(p => p.EvTrimId)
            .Select(g => g.OrderByDescending(p => p.EffectiveDate).FirstOrDefault())
            .ToListAsync();

        var priceDict = prices
            .Where(p => p != null)
            .ToDictionary(p => p!.EvTrimId, p => p);

        // 3. Load specs for these trims
        var trimSpecs = await _db.TrimSpecs
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

    public async Task<EvModel> CreateVehicleModelAsync(CreateVehicleModelDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.ModelName))
            throw new ArgumentException("Model name is required.");

        var exists = await _db.EvModels.AnyAsync(m => m.ModelName == dto.ModelName, ct);
        if (exists)
            throw new InvalidOperationException("A model with this name already exists.");

        var model = new EvModel
        {
            ModelName = dto.ModelName,
            Description = dto.Description,
            Status = dto.Status
        };

        _db.EvModels.Add(model);
        await _db.SaveChangesAsync(ct);

        return model;
    }

    public async Task<EvTrim> CreateVehicleTrimAsync(CreateVehicleTrimDto dto, CancellationToken ct = default)
    {
        // Validate model exists
        var model = await _db.EvModels.FindAsync(dto.EvModelId , ct)
                    ?? throw new InvalidOperationException("Vehicle model not found.");

        var trim = new EvTrim
        {
            EvModelId = dto.EvModelId,
            TrimName = dto.TrimName,
            ModelYear = dto.ModelYear,
            Description = dto.Description
        };

        _db.EvTrims.Add(trim);
        await _db.SaveChangesAsync(ct);

        if (dto.ListedPrice.HasValue)
        {
            var price = new TrimPrice
            {
                EvTrimId = trim.EvTrimId,
                ListedPrice = dto.ListedPrice.Value,
                EffectiveDate = DateTime.UtcNow
            };
            _db.TrimPrices.Add(price);
            await _db.SaveChangesAsync(ct);
        }

        return trim;
    }

    public async Task<EvModel> UpdateVehicleModelStatusAsync(UpdateVehicleModelStatusDto dto, CancellationToken ct = default)
    {
        // Find the model
        var model = await _db.EvModels
            .FirstOrDefaultAsync(m => m.EvModelId == dto.EvModelId, ct)
            ?? throw new InvalidOperationException("Vehicle model not found.");

        // Update status
        model.Status = dto.Status;
        model.Description = dto.Description;

        _db.EvModels.Update(model);
        await _db.SaveChangesAsync(ct);

        return model;
    }
    
    public async Task<TrimPrice> UpdateVehicleTrimPriceAsync(UpdateVehicleTrimPriceDto dto, CancellationToken ct = default)
    {
        if (dto.NewListedPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.NewListedPrice), "Price must be greater than zero.");

        // Validate trim exists
        var trim = await _db.EvTrims
            .FirstOrDefaultAsync(t => t.EvTrimId == dto.EvTrimId, ct)
            ?? throw new InvalidOperationException("EV Trim not found.");

        // Create new price record (we keep history)
        var newPrice = new TrimPrice
        {
            EvTrimId = dto.EvTrimId,
            ListedPrice = dto.NewListedPrice,
            EffectiveDate = dto.EffectiveDate ?? DateTime.UtcNow
        };

        _db.TrimPrices.Add(newPrice);
        await _db.SaveChangesAsync(ct);

        return newPrice;
    }

    public async Task<VehicleModelDto?> GetModelAsync(int id)
    {
        var model = await _db.EvModels
            .Include(m => m.Trims)
            .ThenInclude(t => t.Prices)
            .FirstOrDefaultAsync(m => m.EvModelId == id);

        if (model == null) return null;

        return new VehicleModelDto
        {
            EvModelId = model.EvModelId,
            ModelName = model.ModelName,
            Description = model.Description,
            ModelYear = model.Trims.Max(t => t.ModelYear),
            Status = model.Status
        };
    }
    public async Task<IEnumerable<VehicleModelDto>> GetAllModel()
    {
        var models = await _db.EvModels
            .Include(m => m.Trims)
            .ThenInclude(t => t.Prices)
            .ToListAsync();
        return models.Select(model => new VehicleModelDto
        {
            EvModelId = model.EvModelId,
            ModelName = model.ModelName,
            Description = model.Description,
            ModelYear = model.Trims.Max(t => t.ModelYear),
            Status = model.Status
        });
    }
}
