using ASM_01.DataAccessLayer.Entities.Warehouse;
using ASM_01.DataAccessLayer.Enums;
using ASM_01.DataAccessLayer.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ASM_01.BusinessLayer.Services;

public class DistributionManagementService
{
    private readonly EVRetailsDbContext _db;
    public DistributionManagementService(EVRetailsDbContext db)
    {
        _db = db;
    }
    // === Dealer Stock ===
    public async Task AddOrUpdateStockAsync(int dealerId, int evTrimId, int deltaQuantity, CancellationToken ct = default)
    {
        if (deltaQuantity == 0) return;

        var stock = await _db.VehicleStocks
            .FirstOrDefaultAsync(s => s.DealerId == dealerId && s.EvTrimId == evTrimId, ct);

        if (stock == null)
        {
            if (deltaQuantity < 0)
                throw new InvalidOperationException("Cannot reduce stock below zero for a non-existing item.");

            stock = new VehicleStock
            {
                DealerId = dealerId,
                EvTrimId = evTrimId,
                Quantity = deltaQuantity
            };
            _db.VehicleStocks.Add(stock);
        }
        else
        {
            var newQty = stock.Quantity + deltaQuantity;
            if (newQty < 0)
                throw new InvalidOperationException("Resulting stock cannot be negative.");

            stock.Quantity = newQty;
            _db.VehicleStocks.Update(stock);
        }

        await _db.SaveChangesAsync(ct);
    }

    // === Distribution Requests ===

    public async Task<DistributionRequest> CreateRequestAsync(int dealerId, int evTrimId, int requestedQty, CancellationToken ct = default)
    {
        if (requestedQty <= 0) throw new ArgumentOutOfRangeException(nameof(requestedQty));

        // Validate references exist
        var dealerExists = await _db.Dealers.AnyAsync(d => d.DealerId == dealerId, ct);
        if (!dealerExists) throw new InvalidOperationException("Dealer not found.");

        var trimExists = await _db.EvTrims.AnyAsync(t => t.EvTrimId == evTrimId, ct);
        if (!trimExists) throw new InvalidOperationException("Trim not found.");

        var req = new DistributionRequest
        {
            DealerId = dealerId,
            EvTrimId = evTrimId,
            RequestedQuantity = requestedQty,
            Status = DistributionStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        _db.DistributionRequests.Add(req);
        await _db.SaveChangesAsync(ct);
        return req;
    }

    public async Task<DistributionRequest> ApproveRequestAsync(int requestId, int approvedQty, CancellationToken ct = default)
    {
        if (approvedQty < 0) throw new ArgumentOutOfRangeException(nameof(approvedQty));

        var req = await _db.DistributionRequests.FirstOrDefaultAsync(r => r.DistributionRequestId == requestId, ct)
                  ?? throw new InvalidOperationException("Request not found.");

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

        await _db.SaveChangesAsync(ct);
        return req;
    }

    public async Task<DistributionRequest> RejectRequestAsync(int requestId, string? reason = null, CancellationToken ct = default)
    {
        var req = await _db.DistributionRequests.FirstOrDefaultAsync(r => r.DistributionRequestId == requestId, ct)
                  ?? throw new InvalidOperationException("Request not found.");

        if (req.Status != DistributionStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected.");

        req.Status = DistributionStatus.Rejected;
        req.ApprovedQuantity = 0;
        req.ApprovedAt = DateTime.UtcNow;

        // You could add a Reason field to DistributionRequest if you want to persist this
        await _db.SaveChangesAsync(ct);
        return req;
    }

    /// <summary>
    /// Marks an approved request as completed and increases the dealer's stock by the approved quantity.
    /// </summary>
    public async Task<DistributionRequest> CompleteRequestAsync(int requestId, CancellationToken ct = default)
    {
        var req = await _db.DistributionRequests.FirstOrDefaultAsync(r => r.DistributionRequestId == requestId, ct)
                  ?? throw new InvalidOperationException("Request not found.");

        if (req.Status != DistributionStatus.Approved)
            throw new InvalidOperationException("Only approved requests can be completed.");

        var qty = req.ApprovedQuantity ?? 0;
        if (qty <= 0)
            throw new InvalidOperationException("Approved quantity must be > 0 to complete.");

        // Increase dealer stock (transactional)
        using var tx = await _db.Database.BeginTransactionAsync(ct);
        await AddOrUpdateStockAsync(req.DealerId, req.EvTrimId, qty, ct);

        req.Status = DistributionStatus.Completed;
        _db.DistributionRequests.Update(req);
        await _db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);
        return req;
    }

    public async Task<IReadOnlyList<DistributionRequest>> GetRequestsAsync(int dealerId, CancellationToken ct = default)
    {
        return await _db.DistributionRequests
            .AsNoTracking()
            .Where(r => r.DealerId == dealerId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(ct);
    }
}

