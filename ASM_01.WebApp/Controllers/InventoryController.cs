using ASM_01.BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASM_01.WebApp.Controllers;
[Authorize(Roles = "DISTRIBUTOR")]
public class InventoryController : Controller
{
    private readonly DealerInventoryService _inventoryService;
    public InventoryController(DealerInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    public async Task<IActionResult> Index()
    {
        var dtos = await _inventoryService.GetDealers();

        var dealers = dtos.Select(d => new Models.DealerViewModel
        {
            DealerId = d.DealerId,
            Name = d.Name,
            Address = d.Address
        }).ToList();

        return View(dealers);
    }
}
