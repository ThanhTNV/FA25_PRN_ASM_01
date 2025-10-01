using ASM_01.BusinessLayer.DTOs;
using ASM_01.DataAccessLayer.Entities.Warehouse;

namespace ASM_01.BusinessLayer.Services.Abstract
{
    public interface IDistributionManagementService
    {
        public Task AddOrUpdateStockAsync(int dealerId, int evTrimId, int deltaQuantity, CancellationToken ct = default);
        public Task<DistributionRequest> CreateRequestAsync(int dealerId, int evTrimId, int requestedQty, CancellationToken ct = default);
        public Task<DistributionRequest> ApproveRequestAsync(int requestId, int approvedQty, CancellationToken ct = default);
        public Task<DistributionRequest> RejectRequestAsync(int requestId, string? reason = null, CancellationToken ct = default);
        public Task<DistributionRequest> CompleteRequestAsync(int requestId, CancellationToken ct = default);
        public Task<IReadOnlyList<DistributionRequestDto>> GetRequestsByDealerIdAsync(int dealerId, CancellationToken ct = default);
        public Task<IReadOnlyList<DistributionRequestDto>> GetPendingRequestsAsync(CancellationToken ct = default);
        public Task<IEnumerable<DistributionRequestDto>> GetAllRequestsAsync(CancellationToken ct = default);
    }
}