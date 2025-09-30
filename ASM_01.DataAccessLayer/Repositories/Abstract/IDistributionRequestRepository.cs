using ASM_01.DataAccessLayer.Entities.Warehouse;
using ASM_01.DataAccessLayer.Enums;

namespace ASM_01.DataAccessLayer.Repositories.Abstract;

public interface IDistributionRequestRepository
{
    Task<IEnumerable<DistributionRequest>> GetAllRequestsAsync();
    Task<DistributionRequest?> GetRequestByIdAsync(int id);
    Task<IEnumerable<DistributionRequest>> GetRequestsByDealerIdAsync(int dealerId);
    Task<IEnumerable<DistributionRequest>> GetRequestsByStatusAsync(DistributionStatus status);
    Task<IEnumerable<DistributionRequest>> GetPendingRequestsAsync();
    Task<DistributionRequest> CreateRequestAsync(DistributionRequest request);
    Task<DistributionRequest> UpdateRequestAsync(DistributionRequest request);
    Task<bool> DeleteRequestAsync(int id);
    Task<bool> RequestExistsAsync(int id);
    Task<int> GetRequestCountByDealerAsync(int dealerId);
    Task<int> GetRequestCountByStatusAsync(DistributionStatus status);
}
