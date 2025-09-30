using ASM_01.DataAccessLayer.Entities.Warehouse;
using ASM_01.DataAccessLayer.Enums;
using ASM_01.DataAccessLayer.Persistence;
using ASM_01.DataAccessLayer.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ASM_01.DataAccessLayer.Repositories;

public class DistributionRequestRepository : IDistributionRequestRepository
{
    private readonly EVRetailsDbContext _context;

    public DistributionRequestRepository(EVRetailsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DistributionRequest>> GetAllRequestsAsync()
    {
        return await _context.DistributionRequests
            .Include(r => r.Dealer)
            .Include(r => r.EvTrim)
            .ThenInclude(t => t.EvModel)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DistributionRequest?> GetRequestByIdAsync(int id)
    {
        return await _context.DistributionRequests
            .Include(r => r.Dealer)
            .Include(r => r.EvTrim)
            .ThenInclude(t => t.EvModel)
            .FirstOrDefaultAsync(r => r.DistributionRequestId == id);
    }

    public async Task<IEnumerable<DistributionRequest>> GetRequestsByDealerIdAsync(int dealerId)
    {
        return await _context.DistributionRequests
            .Include(r => r.Dealer)
            .Include(r => r.EvTrim)
            .ThenInclude(t => t.EvModel)
            .Where(r => r.DealerId == dealerId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<DistributionRequest>> GetRequestsByStatusAsync(DistributionStatus status)
    {
        return await _context.DistributionRequests
            .Include(r => r.Dealer)
            .Include(r => r.EvTrim)
            .ThenInclude(t => t.EvModel)
            .Where(r => r.Status == status)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<DistributionRequest>> GetPendingRequestsAsync()
    {
        return await GetRequestsByStatusAsync(DistributionStatus.Pending);
    }

    public async Task<DistributionRequest> CreateRequestAsync(DistributionRequest request)
    {
        _context.DistributionRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<DistributionRequest> UpdateRequestAsync(DistributionRequest request)
    {
        _context.DistributionRequests.Update(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<bool> DeleteRequestAsync(int id)
    {
        var request = await _context.DistributionRequests.FindAsync(id);
        if (request == null) return false;

        _context.DistributionRequests.Remove(request);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RequestExistsAsync(int id)
    {
        return await _context.DistributionRequests.AnyAsync(r => r.DistributionRequestId == id);
    }

    public async Task<int> GetRequestCountByDealerAsync(int dealerId)
    {
        return await _context.DistributionRequests
            .CountAsync(r => r.DealerId == dealerId);
    }

    public async Task<int> GetRequestCountByStatusAsync(DistributionStatus status)
    {
        return await _context.DistributionRequests
            .CountAsync(r => r.Status == status);
    }
}
