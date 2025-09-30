using ASM_01.DataAccessLayer.Entities.VehicleModels;
using ASM_01.DataAccessLayer.Persistence;
using ASM_01.DataAccessLayer.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ASM_01.DataAccessLayer.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly EVRetailsDbContext _context;

    public VehicleRepository(EVRetailsDbContext context)
    {
        _context = context;
    }

    #region EvModel operations
    public async Task<IEnumerable<EvModel>> GetAllModelsAsync()
    {
        return await _context.EvModels
            .Include(m => m.Trims)
            .ThenInclude(t => t.Prices)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<EvModel?> GetModelByIdAsync(int id)
    {
        return await _context.EvModels
            .Include(m => m.Trims)
            .ThenInclude(t => t.Prices)
            .FirstOrDefaultAsync(m => m.EvModelId == id);
    }

    public async Task<EvModel> CreateModelAsync(EvModel model)
    {
        _context.EvModels.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<EvModel> UpdateModelAsync(EvModel model)
    {
        _context.EvModels.Update(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> DeleteModelAsync(int id)
    {
        var model = await _context.EvModels.FindAsync(id);
        if (model == null) return false;

        _context.EvModels.Remove(model);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ModelExistsAsync(int id)
    {
        return await _context.EvModels.AnyAsync(m => m.EvModelId == id);
    }

    public async Task<bool> ModelNameExistsAsync(string modelName, int? excludeId = null)
    {
        var query = _context.EvModels.Where(m => m.ModelName == modelName);
        if (excludeId.HasValue)
        {
            query = query.Where(m => m.EvModelId != excludeId.Value);
        }
        return await query.AnyAsync();
    }
    #endregion

    #region EvTrim operations
    public async Task<IEnumerable<EvTrim>> GetAllTrimsAsync()
    {
        return await _context.EvTrims
            .Include(t => t.EvModel)
            .Include(t => t.Prices)
            .Include(t => t.TrimSpecs)
            .ThenInclude(ts => ts.Spec)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<EvTrim?> GetTrimByIdAsync(int id)
    {
        return await _context.EvTrims
            .Include(t => t.EvModel)
            .Include(t => t.Prices)
            .Include(t => t.TrimSpecs)
            .ThenInclude(ts => ts.Spec)
            .FirstOrDefaultAsync(t => t.EvTrimId == id);
    }

    public async Task<IEnumerable<EvTrim>> GetTrimsByModelIdAsync(int modelId)
    {
        return await _context.EvTrims
            .Include(t => t.EvModel)
            .Include(t => t.Prices)
            .Where(t => t.EvModelId == modelId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<EvTrim> CreateTrimAsync(EvTrim trim)
    {
        _context.EvTrims.Add(trim);
        await _context.SaveChangesAsync();
        return trim;
    }

    public async Task<EvTrim> UpdateTrimAsync(EvTrim trim)
    {
        _context.EvTrims.Update(trim);
        await _context.SaveChangesAsync();
        return trim;
    }

    public async Task<bool> DeleteTrimAsync(int id)
    {
        var trim = await _context.EvTrims.FindAsync(id);
        if (trim == null) return false;

        _context.EvTrims.Remove(trim);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TrimExistsAsync(int id)
    {
        return await _context.EvTrims.AnyAsync(t => t.EvTrimId == id);
    }
    #endregion

    #region TrimPrice operations
    public async Task<IEnumerable<TrimPrice>> GetPricesByTrimIdAsync(int trimId)
    {
        return await _context.TrimPrices
            .Where(p => p.EvTrimId == trimId)
            .OrderByDescending(p => p.EffectiveDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TrimPrice?> GetLatestPriceByTrimIdAsync(int trimId)
    {
        return await _context.TrimPrices
            .Where(p => p.EvTrimId == trimId)
            .OrderByDescending(p => p.EffectiveDate)
            .FirstOrDefaultAsync();
    }

    public async Task<TrimPrice> CreatePriceAsync(TrimPrice price)
    {
        _context.TrimPrices.Add(price);
        await _context.SaveChangesAsync();
        return price;
    }

    public async Task<bool> DeletePriceAsync(int id)
    {
        var price = await _context.TrimPrices.FindAsync(id);
        if (price == null) return false;

        _context.TrimPrices.Remove(price);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region Spec operations
    public async Task<IEnumerable<Spec>> GetAllSpecsAsync()
    {
        return await _context.Specs
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Spec?> GetSpecByIdAsync(int id)
    {
        return await _context.Specs.FindAsync(id);
    }

    public async Task<Spec> CreateSpecAsync(Spec spec)
    {
        _context.Specs.Add(spec);
        await _context.SaveChangesAsync();
        return spec;
    }

    public async Task<Spec> UpdateSpecAsync(Spec spec)
    {
        _context.Specs.Update(spec);
        await _context.SaveChangesAsync();
        return spec;
    }

    public async Task<bool> DeleteSpecAsync(int id)
    {
        var spec = await _context.Specs.FindAsync(id);
        if (spec == null) return false;

        _context.Specs.Remove(spec);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region TrimSpec operations
    public async Task<IEnumerable<TrimSpec>> GetSpecsByTrimIdAsync(int trimId)
    {
        return await _context.TrimSpecs
            .Include(ts => ts.Spec)
            .Where(ts => ts.EvTrimId == trimId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TrimSpec?> GetTrimSpecAsync(int trimId, int specId)
    {
        return await _context.TrimSpecs
            .Include(ts => ts.Spec)
            .FirstOrDefaultAsync(ts => ts.EvTrimId == trimId && ts.SpecId == specId);
    }

    public async Task<TrimSpec> CreateTrimSpecAsync(TrimSpec trimSpec)
    {
        _context.TrimSpecs.Add(trimSpec);
        await _context.SaveChangesAsync();
        return trimSpec;
    }

    public async Task<TrimSpec> UpdateTrimSpecAsync(TrimSpec trimSpec)
    {
        _context.TrimSpecs.Update(trimSpec);
        await _context.SaveChangesAsync();
        return trimSpec;
    }

    public async Task<bool> DeleteTrimSpecAsync(int trimId, int specId)
    {
        var trimSpec = await _context.TrimSpecs
            .FirstOrDefaultAsync(ts => ts.EvTrimId == trimId && ts.SpecId == specId);
        if (trimSpec == null) return false;

        _context.TrimSpecs.Remove(trimSpec);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region Search operations
    public async Task<IEnumerable<EvTrim>> SearchTrimsAsync(string keyword)
    {
        var lowerKeyword = keyword.ToLower();
        return await _context.EvTrims
            .Include(t => t.EvModel)
            .Include(t => t.Prices)
            .Where(t => t.EvModel.ModelName.ToLower().Contains(lowerKeyword) ||
                       t.TrimName.ToLower().Contains(lowerKeyword))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<EvModel>> SearchModelsAsync(string keyword)
    {
        var lowerKeyword = keyword.ToLower();
        return await _context.EvModels
            .Include(m => m.Trims)
            .Where(m => m.ModelName.ToLower().Contains(lowerKeyword))
            .AsNoTracking()
            .ToListAsync();
    }
    #endregion
}
