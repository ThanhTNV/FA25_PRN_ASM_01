using ASM_01.DataAccessLayer.Entities.VehicleModels;

namespace ASM_01.DataAccessLayer.Repositories.Abstract;

public interface IVehicleRepository
{
    // EvModel operations
    Task<IEnumerable<EvModel>> GetAllModelsAsync();
    Task<EvModel?> GetModelByIdAsync(int id);
    Task<EvModel> CreateModelAsync(EvModel model);
    Task<EvModel> UpdateModelAsync(EvModel model);
    Task<bool> DeleteModelAsync(int id);
    Task<bool> ModelExistsAsync(int id);
    Task<bool> ModelNameExistsAsync(string modelName, int? excludeId = null);

    // EvTrim operations
    Task<IEnumerable<EvTrim>> GetAllTrimsAsync();
    Task<EvTrim?> GetTrimByIdAsync(int id);
    Task<IEnumerable<EvTrim>> GetTrimsByModelIdAsync(int modelId);
    Task<EvTrim> CreateTrimAsync(EvTrim trim);
    Task<EvTrim> UpdateTrimAsync(EvTrim trim);
    Task<bool> DeleteTrimAsync(int id);
    Task<bool> TrimExistsAsync(int id);

    // TrimPrice operations
    Task<IEnumerable<TrimPrice>> GetPricesByTrimIdAsync(int trimId);
    Task<TrimPrice?> GetLatestPriceByTrimIdAsync(int trimId);
    Task<TrimPrice> CreatePriceAsync(TrimPrice price);
    Task<bool> DeletePriceAsync(int id);

    // Spec operations
    Task<IEnumerable<Spec>> GetAllSpecsAsync();
    Task<Spec?> GetSpecByIdAsync(int id);
    Task<Spec> CreateSpecAsync(Spec spec);
    Task<Spec> UpdateSpecAsync(Spec spec);
    Task<bool> DeleteSpecAsync(int id);

    // TrimSpec operations
    Task<IEnumerable<TrimSpec>> GetSpecsByTrimIdAsync(int trimId);
    Task<TrimSpec?> GetTrimSpecAsync(int trimId, int specId);
    Task<TrimSpec> CreateTrimSpecAsync(TrimSpec trimSpec);
    Task<TrimSpec> UpdateTrimSpecAsync(TrimSpec trimSpec);
    Task<bool> DeleteTrimSpecAsync(int trimId, int specId);

    // Search operations
    Task<IEnumerable<EvTrim>> SearchTrimsAsync(string keyword);
    Task<IEnumerable<EvModel>> SearchModelsAsync(string keyword);
}
