using ASM_01.BusinessLayer.DTOs;
using ASM_01.BusinessLayer.Services.Abstract;
using ASM_01.DataAccessLayer.Entities.VehicleModels;
using ASM_01.DataAccessLayer.Repositories.Abstract;

namespace ASM_01.BusinessLayer.Services;

public class VehicleService(IVehicleRepository _vehicleRepository, IUnitOfWork _unitOfWork) : IVehicleService
{
    public async Task<IEnumerable<VehicleDto>> GetAllVehicles()
    {
        var trims = await _vehicleRepository.GetAllTrimsAsync();

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

        var specs = await _vehicleRepository.GetAllSpecsAsync();

        foreach (var v in vehicles)
        {
            var trimSpecs = await _vehicleRepository.GetSpecsByTrimIdAsync(v.VehicleId);
            var vehicleSpecs = trimSpecs.ToDictionary(
                ts => ts.Spec.SpecName,
                ts => ts.Value + (ts.Spec.Unit != null ? $" {ts.Spec.Unit}" : "")
            );

            v.Specifications = vehicleSpecs;
        }
        return vehicles;
    }

    public async Task<VehicleDto?> GetVehicleById(int id)
    {
        var trim = await _vehicleRepository.GetTrimByIdAsync(id);

        if (trim == null)
            return null;

        var latestPrice = await _vehicleRepository.GetLatestPriceByTrimIdAsync(id);
        var trimSpecs = await _vehicleRepository.GetSpecsByTrimIdAsync(id);
        var specs = trimSpecs.ToDictionary(
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
        var trims = await _vehicleRepository.SearchTrimsAsync(keyword);

        var result = new List<VehicleDto>();

        foreach (var trim in trims)
        {
            var latestPrice = await _vehicleRepository.GetLatestPriceByTrimIdAsync(trim.EvTrimId);
            var trimSpecs = await _vehicleRepository.GetSpecsByTrimIdAsync(trim.EvTrimId);
            var specs = trimSpecs.ToDictionary(
                ts => ts.Spec.SpecName,
                ts => ts.Value + (ts.Spec.Unit != null ? $" {ts.Spec.Unit}" : "")
            );

            result.Add(new VehicleDto
            {
                ModelId = trim.EvModelId,
                VehicleId = trim.EvTrimId,
                ModelName = trim.EvModel.ModelName,
                TrimName = trim.TrimName,
                Description = trim.Description,
                Status = trim.EvModel.Status.ToString(),
                ModelYear = trim.ModelYear,
                Price = latestPrice?.ListedPrice ?? 0,
                EffectiveDate = latestPrice?.EffectiveDate ?? DateTime.MinValue,
                Specifications = specs
            });
        }

        return result;
    }

    public async Task<IEnumerable<VehicleComparisonDto>> CompareVehicles(int[] vehicleIds)
    {
        var result = new List<VehicleComparisonDto>();

        foreach (var vehicleId in vehicleIds)
        {
            var trim = await _vehicleRepository.GetTrimByIdAsync(vehicleId);
            if (trim == null) continue;

            var latestPrice = await _vehicleRepository.GetLatestPriceByTrimIdAsync(vehicleId);
            var trimSpecs = await _vehicleRepository.GetSpecsByTrimIdAsync(vehicleId);
            var specs = trimSpecs.ToDictionary(
                ts => ts.Spec.SpecName,
                ts => ts.Value + (ts.Spec.Unit != null ? $" {ts.Spec.Unit}" : "")
            );

            result.Add(new VehicleComparisonDto
            {
                VehicleId = trim.EvTrimId,
                ModelName = trim.EvModel.ModelName,
                TrimName = trim.TrimName,
                Price = latestPrice?.ListedPrice ?? 0,
                EffectiveDate = latestPrice?.EffectiveDate ?? DateTime.MinValue,
                Specifications = specs
            });
        }

        return result;
    }

    public async Task<EvModel> CreateVehicleModelAsync(CreateVehicleModelDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.ModelName))
            throw new ArgumentException("Model name is required.");

        var exists = await _vehicleRepository.ModelNameExistsAsync(dto.ModelName);
        if (exists)
            throw new InvalidOperationException("A model with this name already exists.");

        var model = new EvModel
        {
            ModelName = dto.ModelName,
            Description = dto.Description,
            Status = dto.Status
        };

        return await _vehicleRepository.CreateModelAsync(model);
    }

    public async Task<EvTrim> CreateVehicleTrimAsync(CreateVehicleTrimDto dto, CancellationToken ct = default)
    {
        // Validate model exists
        var model = await _vehicleRepository.GetModelByIdAsync(dto.EvModelId);
        if (model == null)
            throw new InvalidOperationException("Vehicle model not found.");

        var trim = new EvTrim
        {
            EvModelId = dto.EvModelId,
            TrimName = dto.TrimName,
            ModelYear = dto.ModelYear,
            Description = dto.Description
        };

        var createdTrim = await _vehicleRepository.CreateTrimAsync(trim);

        if (dto.ListedPrice.HasValue)
        {
            var price = new TrimPrice
            {
                EvTrimId = createdTrim.EvTrimId,
                ListedPrice = dto.ListedPrice.Value,
                EffectiveDate = DateTime.UtcNow
            };
            await _vehicleRepository.CreatePriceAsync(price);
        }

        return createdTrim;
    }

    public async Task<EvModel> UpdateVehicleModelStatusAsync(UpdateVehicleModelStatusDto dto, CancellationToken ct = default)
    {
        // Find the model
        var model = await _vehicleRepository.GetModelByIdAsync(dto.EvModelId);
        if (model == null)
            throw new InvalidOperationException("Vehicle model not found.");

        // Update status
        model.Status = dto.Status;
        model.Description = dto.Description;

        return await _vehicleRepository.UpdateModelAsync(model);
    }
    
    public async Task<TrimPrice> UpdateVehicleTrimPriceAsync(UpdateVehicleTrimPriceDto dto, CancellationToken ct = default)
    {
        if (dto.NewListedPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.NewListedPrice), "Price must be greater than zero.");

        // Validate trim exists
        var trim = await _vehicleRepository.GetTrimByIdAsync(dto.EvTrimId);
        if (trim == null)
            throw new InvalidOperationException("EV Trim not found.");

        // Create new price record (we keep history)
        var newPrice = new TrimPrice
        {
            EvTrimId = dto.EvTrimId,
            ListedPrice = dto.NewListedPrice,
            EffectiveDate = dto.EffectiveDate ?? DateTime.UtcNow
        };

        return await _vehicleRepository.CreatePriceAsync(newPrice);
    }

    public async Task<VehicleModelDto?> GetModelAsync(int id)
    {
        var model = await _vehicleRepository.GetModelByIdAsync(id);

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
        var models = await _vehicleRepository.GetAllModelsAsync();
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
