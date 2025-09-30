using ASM_01.BusinessLayer.DTOs;
using ASM_01.DataAccessLayer.Entities.VehicleModels;

namespace ASM_01.BusinessLayer.Services.Abstract
{
    public interface IVehicleService
    {
        public Task<IEnumerable<VehicleDto>> GetAllVehicles();
        public Task<IEnumerable<VehicleModelDto>> GetAllModel();
        public Task<VehicleDto?> GetVehicleById(int id);
        public Task<IEnumerable<VehicleDto>> SearchVehicles(string keyword);
        public Task<IEnumerable<VehicleComparisonDto>> CompareVehicles(int[] vehicleIds);
        public Task<EvModel> CreateVehicleModelAsync(CreateVehicleModelDto dto, CancellationToken ct = default);
        public Task<EvTrim> CreateVehicleTrimAsync(CreateVehicleTrimDto dto, CancellationToken ct = default);
        public Task<EvModel> UpdateVehicleModelStatusAsync(UpdateVehicleModelStatusDto dto, CancellationToken ct = default);
        public Task<TrimPrice> UpdateVehicleTrimPriceAsync(UpdateVehicleTrimPriceDto dto, CancellationToken ct = default);
        public Task<VehicleModelDto?> GetModelAsync(int id);
    }
}