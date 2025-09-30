using ASM_01.BusinessLayer.Services;
using ASM_01.BusinessLayer.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASM_01.WebApp.Controllers;
[Authorize(Roles = "DISTRIBUTOR")]
public class InventoryController(IDealerInventoryService inventoryService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var dtos = await inventoryService.GetDealers();

        var dealers = dtos.Select(d => new Models.DealerViewModel
        {
            DealerId = d.DealerId,
            Name = d.Name,
            Address = d.Address
        }).ToList();

        return View(dealers);
    }
}
