using ASM_01.BusinessLayer.DTOs;
using ASM_01.BusinessLayer.Services.Abstract;
using ASM_01.DataAccessLayer.Entities.Warehouse;
using ASM_01.DataAccessLayer.Enums;
using ASM_01.DataAccessLayer.Repositories.Abstract;

namespace ASM_01.BusinessLayer.Services;

public class DistributionManagementService : IDistributionManagementService
{
    private readonly IStockRepository _stockRepository;
    private readonly IDistributionRequestRepository _distributionRequestRepository;
    private readonly IDealerRepository _dealerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DistributionManagementService(
        IStockRepository stockRepository,
        IDistributionRequestRepository distributionRequestRepository,
        IDealerRepository dealerRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _stockRepository = stockRepository;
        _distributionRequestRepository = distributionRequestRepository;
        _dealerRepository = dealerRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task AddOrUpdateStockAsync(int dealerId, int evTrimId, int deltaQuantity, CancellationToken ct = default)
    {
        if (deltaQuantity == 0) return;

        var currentQuantity = await _stockRepository.GetStockQuantityAsync(dealerId, evTrimId);
        var newQuantity = currentQuantity + deltaQuantity;

        if (newQuantity < 0)
            throw new InvalidOperationException("Resulting stock cannot be negative.");

        await _stockRepository.UpdateStockQuantityAsync(dealerId, evTrimId, newQuantity);
    }

    public async Task<DistributionRequest> CreateRequestAsync(int dealerId, int evTrimId, int requestedQty, CancellationToken ct = default)
    {
        if (requestedQty <= 0) throw new ArgumentOutOfRangeException(nameof(requestedQty));

        // Validate references exist
        var dealerExists = await _dealerRepository.DealerExistsAsync(dealerId);
        if (!dealerExists) throw new InvalidOperationException("Dealer not found.");

        var trimExists = await _vehicleRepository.TrimExistsAsync(evTrimId);
        if (!trimExists) throw new InvalidOperationException("Trim not found.");

        var req = new DistributionRequest
        {
            DealerId = dealerId,
            EvTrimId = evTrimId,
            RequestedQuantity = requestedQty,
            Status = DistributionStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        return await _distributionRequestRepository.CreateRequestAsync(req);
    }

    public async Task<DistributionRequest> ApproveRequestAsync(int requestId, int approvedQty, CancellationToken ct = default)
    {
        if (approvedQty < 0) throw new ArgumentOutOfRangeException(nameof(approvedQty));

        var req = await _distributionRequestRepository.GetRequestByIdAsync(requestId);
        if (req == null)
            throw new InvalidOperationException("Request not found.");

        if (req.Status != DistributionStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved.");

        if (approvedQty == 0)
        {
            // Treat as reject if approvedQty=0
            req.Status = DistributionStatus.Rejected;
            req.ApprovedQuantity = 0;
            req.ApprovedAt = DateTime.UtcNow;
        }
        else
        {
            req.Status = DistributionStatus.Approved;
            req.ApprovedQuantity = approvedQty;
            req.ApprovedAt = DateTime.UtcNow;
        }

        return await _distributionRequestRepository.UpdateRequestAsync(req);
    }

    public async Task<DistributionRequest> RejectRequestAsync(int requestId, string? reason = null, CancellationToken ct = default)
    {
        var req = await _distributionRequestRepository.GetRequestByIdAsync(requestId);
        if (req == null)
            throw new InvalidOperationException("Request not found.");

        if (req.Status != DistributionStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected.");

        req.Status = DistributionStatus.Rejected;
        req.ApprovedQuantity = 0;
        req.ApprovedAt = DateTime.UtcNow;

        // You could add a Reason field to DistributionRequest if you want to persist this
        return await _distributionRequestRepository.UpdateRequestAsync(req);
    }

    public async Task<DistributionRequest> CompleteRequestAsync(int requestId, CancellationToken ct = default)
    {
        var req = await _distributionRequestRepository.GetRequestByIdAsync(requestId);
        if (req == null)
            throw new InvalidOperationException("Request not found.");

        if (req.Status != DistributionStatus.Approved)
            throw new InvalidOperationException("Only approved requests can be completed.");

        var qty = req.ApprovedQuantity ?? 0;
        if (qty <= 0)
            throw new InvalidOperationException("Approved quantity must be > 0 to complete.");

        // Increase dealer stock (transactional)
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await AddOrUpdateStockAsync(req.DealerId, req.EvTrimId, qty, ct);

            req.Status = DistributionStatus.Completed;
            await _distributionRequestRepository.UpdateRequestAsync(req);

            await _unitOfWork.CommitTransactionAsync();
            return req;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IReadOnlyList<DistributionRequestDto>> GetRequestsByDealerIdAsync(int dealerId, CancellationToken ct = default)
    {
        var requests = await _distributionRequestRepository.GetRequestsByDealerIdAsync(dealerId);

        var dtos = requests.Select(r => new DistributionRequestDto
        {
            RequestId = r.DistributionRequestId,
            ModelName = r.EvTrim?.EvModel?.ModelName ?? "N/A",
            TrimName = r.EvTrim?.TrimName ?? "N/A",
            RequestQuantity = r.RequestedQuantity,
            ApprovedQuantity = r.ApprovedQuantity ?? 0,
            ModelYear = r.EvTrim?.ModelYear ?? DateTime.MinValue.Year,
            ApprovalDate = r.ApprovedAt,
            RequestDate = r.RequestedAt,
            Status = r.Status.ToString()
        }).ToList();

        return dtos;
    }
    
    public async Task<IReadOnlyList<DistributionRequestDto>> GetPendingRequestsAsync(CancellationToken ct = default)
    {
        var requests = await _distributionRequestRepository.GetPendingRequestsAsync();
        var dtos = requests.Select(r => new DistributionRequestDto
        {
            RequestId = r.DistributionRequestId,
            ModelName = r.EvTrim?.EvModel?.ModelName ?? "N/A",
            TrimName = r.EvTrim?.TrimName ?? "N/A",
            RequestQuantity = r.RequestedQuantity,
            ApprovedQuantity = r.ApprovedQuantity ?? 0,
            ModelYear = r.EvTrim?.ModelYear ?? DateTime.MinValue.Year,
            ApprovalDate = r.ApprovedAt,
            RequestDate = r.RequestedAt,
            DealerName = r.Dealer?.Name ?? "N/A",
            Status = r.Status.ToString()
        }).ToList();
        return dtos;
    }

    public async Task<IEnumerable<DistributionRequestDto>> GetAllRequestsAsync(CancellationToken ct = default)
    {
        var requests = await _distributionRequestRepository.GetAllRequestsAsync();
        var dtos = requests.Select(r => new DistributionRequestDto
        {
            RequestId = r.DistributionRequestId,
            ModelName = r.EvTrim?.EvModel?.ModelName ?? "N/A",
            TrimName = r.EvTrim?.TrimName ?? "N/A",
            RequestQuantity = r.RequestedQuantity,
            ApprovedQuantity = r.ApprovedQuantity ?? 0,
            ModelYear = r.EvTrim?.ModelYear ?? DateTime.MinValue.Year,
            ApprovalDate = r.ApprovedAt,
            RequestDate = r.RequestedAt,
            DealerName = r.Dealer?.Name ?? "N/A",
            Status = r.Status.ToString()
        }).ToList();

        return dtos;
    }
}

