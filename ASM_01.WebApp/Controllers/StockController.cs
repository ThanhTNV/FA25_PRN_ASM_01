using ASM_01.BusinessLayer.Services;
using ASM_01.BusinessLayer.Services.Abstract;
using ASM_01.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM_01.WebApp.Controllers
{
    [Authorize(Roles = "DEALER, DISTRIBUTOR")]
    public class StockController(IDealerInventoryService _dealerInventoryService) : Controller
    {
        // GET: /Stock  (Dealer views their own stock)
        [Authorize(Roles = "DEALER")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Prefer a dedicated claim like "DealerId" that you set at sign-in.
            var dealerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(dealerIdClaim, out var dealerId))
            {
                // You can also redirect to login or show a friendly error
                return Forbid();
            }

            var dtos = await _dealerInventoryService.GetDealerInventoryAsync(dealerId);

            var viewModel = dtos.Select(d => new DealerStockViewModel
            {
                EvTrimId = d.EvTrimId,
                TrimName = d.TrimName,
                ModelName = d.ModelName,
                ModelYear = d.ModelYear,
                Quantity = d.Quantity
            }).ToList();

            return View(viewModel);
        }

        // GET: /Stock/Dealer/{dealerId}  (Distributor views chosen dealer's stock)
        [Authorize(Roles = "DISTRIBUTOR")]
        public async Task<IActionResult> Dealer(int dealerId)
        {
            var dtos = await _dealerInventoryService.GetDealerInventoryAsync(dealerId);

            var viewModel = dtos.Select(d => new DealerStockViewModel
            {
                EvTrimId = d.EvTrimId,
                TrimName = d.TrimName,
                ModelName = d.ModelName,
                ModelYear = d.ModelYear,
                Quantity = d.Quantity
            }).ToList();

            return View("Index", viewModel); // reuse the same table view if you like
        }

        // GET: /Stock/Details/{trimId}
        public IActionResult Details(int trimId)
        {
            ViewBag.IsDealer = true;
            return RedirectToAction("Details", "Vehicle", new { id = trimId });
        }
    }
}
