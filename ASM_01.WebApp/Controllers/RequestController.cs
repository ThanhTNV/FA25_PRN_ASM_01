using ASM_01.BusinessLayer.Services;
using ASM_01.DataAccessLayer.Enums;
using ASM_01.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ASM_01.WebApp.Controllers
{
    [Authorize]
    public class RequestController(DistributionManagementService _distributionManagementService, VehicleService _vehicleService) : Controller
    {
        [Authorize(Roles = "DEALER")]
        public async Task<IActionResult> MyRequests()
        {
            var dealerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(dealerIdClaim, out var dealerId))
            {
                // You can also redirect to login or show a friendly error
                return Forbid();
            }
            var myRequests = await _distributionManagementService.GetRequestsByDealerIdAsync(dealerId);
            var viewModel = myRequests.Select(r => new MyRequestViewModel
            {
                RequestId = r.RequestId,
                TrimName = r.TrimName,
                ModelName = r.ModelName,
                RequestQuantity = r.RequestQuantity,
                ApprovedQuantity = r.ApprovedQuantity,
                RequestDate = r.RequestDate,
                ModelYear = r.ModelYear,
                ApprovalDate = r.ApprovalDate,
                Status = r.Status,
            }).ToList();
            return View(viewModel);
        }

        [Authorize(Roles = "DEALER")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var evModels = await _vehicleService.GetAllVehicles();
            var selectViewModels = evModels.Select(v => new VehicleSelectViewModel
            {
                EvTrimId = v.TrimId,
                TrimName = v.TrimName,
                VehicleName = v.TrimName,
                VehicleId = v.VehicleId,
            }).ToList();
            ViewBag.VehicleList = new SelectList(selectViewModels, "EvTrimId", "VehicleName");

            return View();
        }

        [Authorize(Roles = "DEALER")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] RequestCreateViewModel requestCreateViewModel)
        {
            var dealerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(dealerIdClaim, out var dealerId))
            {
                // You can also redirect to login or show a friendly error
                return Forbid();
            }
            try
            {
                await _distributionManagementService.CreateRequestAsync(
                    dealerId,
                    requestCreateViewModel.EvTrimId,
                    requestCreateViewModel.Quantity);
                TempData["Success"] = "Request created successfully.";
                return RedirectToAction("Index", "Request");
            }
            catch (Exception)
            {
                // Log the exception (ex) as needed
                ModelState.AddModelError(string.Empty, "An error occurred while creating the request.");
                return View(requestCreateViewModel);
            }
        }

        [Authorize(Roles = "DISTRIBUTOR")]
        public async Task<IActionResult> Pending(CancellationToken ct)
        {
            var dtos = await _distributionManagementService.GetPendingRequestsAsync(ct);

            var vms = dtos.Select(r => new PendingRequestViewModel
            {
                RequestId = r.RequestId,
                DealerName = r.DealerName,
                ModelName = r.ModelName,
                TrimName = r.TrimName,
                ModelYear = r.ModelYear,
                RequestQuantity = r.RequestQuantity,
                ApprovedQuantity = r.ApprovedQuantity,
                Status = r.Status.ToString(),
                RequestDate = r.RequestDate
            }).ToList();

            return View(vms); // Views/Request/Pending.cshtml
        }

        [Authorize(Roles = "DISTRIBUTOR")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var dtos = await _distributionManagementService.GetAllRequestsAsync(ct);
            var vms = dtos.Select(r => new DistRequestRowViewModel
                {
                    RequestId = r.RequestId,
                    DealerName = r.DealerName,
                    ModelName = r.ModelName,
                    TrimName = r.DealerName,
                    ModelYear = r.ModelYear,
                    RequestQuantity = r.RequestQuantity,
                    ApprovedQuantity = r.ApprovedQuantity,
                    Status = r.Status.ToString(),
                    RequestDate = r.RequestDate,
                    ApprovalDate = r.ApprovalDate
                }).ToList();

            return View(vms); // Views/Request/Index.cshtml
        }

        [Authorize(Roles = "DISTRIBUTOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int requestId, int approvedQty, CancellationToken ct)
        {
            try
            {
                await _distributionManagementService.ApproveRequestAsync(requestId, approvedQty, ct);
                TempData["Success"] = $"Request #{requestId} approved with {approvedQty} vehicles.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Approval failed: {ex.Message}";
            }

            return RedirectToAction("Pending");
        }

        [Authorize(Roles = "DISTRIBUTOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int requestId, CancellationToken ct)
        {
            try
            {
                await _distributionManagementService.RejectRequestAsync(requestId, null, ct);
                TempData["Success"] = $"Request #{requestId} was rejected.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Rejection failed: {ex.Message}";
            }

            return RedirectToAction("Pending");
        }

        [Authorize(Roles = "DISTRIBUTOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int requestId, CancellationToken ct)
        {
            try
            {
                await _distributionManagementService.CompleteRequestAsync(requestId, ct);
                TempData["Success"] = $"Request #{requestId} marked as completed and stock updated.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Completion failed: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
