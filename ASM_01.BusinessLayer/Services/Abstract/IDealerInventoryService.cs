using ASM_01.BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_01.BusinessLayer.Services.Abstract
{
    public interface IDealerInventoryService
    {
        public Task<IReadOnlyList<DealerDto>> GetDealers(CancellationToken ct = default);
        public Task<IReadOnlyList<DealerStockDto>> GetDealerInventoryAsync(int dealerId, CancellationToken ct = default);
    }
}
