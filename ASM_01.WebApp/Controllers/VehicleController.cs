using ASM_01.BusinessLayer.DTOs;
using ASM_01.BusinessLayer.Services;
using ASM_01.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASM_01.WebApp.Controllers;

[Authorize]
public class VehicleController : Controller
{
    private readonly VehicleService _vehicleService;

    public VehicleController(VehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    // GET: /Vehicle/Catalog
    public async Task<IActionResult> Catalog(string? searchTerm)
    {
        if (searchTerm != null)
        {
            var searchDtos = await _vehicleService.SearchVehicles(searchTerm);
            var searchResults = searchDtos.Select(dto => new VehicleCatalogViewModel
            {
                VehicleId = dto.VehicleId,
                ModelId = dto.ModelId,
                ModelName = dto.ModelName,
                TrimName = dto.TrimName,
                Price = dto.Price
            }).ToList();
            return View(searchResults); // VehicleCatalog.cshtml
        }
        var dtos = await _vehicleService.GetAllVehicles();
        var vehicleCatalogVMs = dtos.Select(dto => new VehicleCatalogViewModel
        {
            ModelId = dto.ModelId,
            VehicleId = dto.VehicleId,
            ModelName = dto.ModelName,
            TrimName = dto.TrimName,
            Price = dto.Price
        }).ToList();
        return View(vehicleCatalogVMs); // VehicleCatalog.cshtml
    }

    // GET: /Vehicle/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _vehicleService.GetVehicleById(id);
        if (dto == null)
        {
            TempData["Error"] = $"Vehicle with ID {id} not found.";
            return RedirectToAction(nameof(Catalog));
        }

        var vehicleDetails = new VehicleDetailsViewModel
        {
            VehicleId = dto.VehicleId,
            ModelName = dto.ModelName,
            TrimName = dto.TrimName,
            Price = dto.Price,
            EffectiveDate = dto.EffectiveDate,
            Specifications = dto.Specifications
        };

        return View(vehicleDetails); // VehicleDetails.cshtml
    }

    // GET: /Vehicle/Compare?ids=1&ids=2&ids=3
    public async Task<IActionResult> Comparison([FromQuery] int[] ids)
    {
        if (ids == null || ids.Length < 2)
        {
            TempData["Error"] = "Please select at least two vehicles to compare.";
            return RedirectToAction(nameof(Catalog));
        }

        var dtos = await _vehicleService.CompareVehicles(ids);
        var viewModel = new VehicleComparisonViewModel
        {
            Vehicles = [.. dtos]
        };

        // Collect all distinct spec names from all vehicles
        viewModel.SpecificationKeys = [.. dtos
            .SelectMany(v => v.Specifications.Keys)
            .Distinct()
            .OrderBy(k => k)];
        return View(viewModel); // VehicleComparison.cshtml
    }

    [Authorize(Roles = "DISTRIBUTOR")]
    [HttpGet]
    public async Task<IActionResult> EditModel(int? modelId, CancellationToken ct)
    {
        if (modelId == null)
        {
            ViewBag.IsEdit = false;
            return View(new EditVehicleModelViewModel());
        }

        var model = await _vehicleService.GetModelAsync(modelId.Value); // <-- via VehicleService
        if (model == null) return NotFound();

        var vm = new EditVehicleModelViewModel
        {
            EvModelId = model.EvModelId,
            ModelName = model.ModelName,
            Description = model.Description,
            Status = model.Status
        };

        ViewBag.IsEdit = true;
        return View(vm);
    }

    [Authorize(Roles = "DISTRIBUTOR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditModel(EditVehicleModelViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.IsEdit = vm.EvModelId > 0;
            return View(vm);
        }

        if (vm.EvModelId == 0)
        {
            // Create via VehicleService
            var dto = new CreateVehicleModelDto
            {
                ModelName = vm.ModelName,
                Description = vm.Description,
                Status = vm.Status
            };
            await _vehicleService.CreateVehicleModelAsync(dto, ct);
            TempData["Success"] = "Vehicle model created.";
        }
        else
        {
            // Update — using VehicleService only.
            // If you have an UpdateVehicleModelAsync, call it here.
            // With the current service, we can at least update Status:
            await _vehicleService.UpdateVehicleModelStatusAsync(
                new UpdateVehicleModelStatusDto { EvModelId = vm.EvModelId, Status = vm.Status }, ct);

            // If you want name/description updates, add this in VehicleService:
            // await _vehicleService.UpdateVehicleModelAsync(new UpdateVehicleModelDto { ... }, ct);

            TempData["Success"] = "Vehicle model updated.";
        }

        return RedirectToAction("Index", "Vehicle");
    }

    [Authorize(Roles = "DISTRIBUTOR")]
    [HttpGet]
    public async Task<IActionResult> EditTrim(int? trimId, CancellationToken ct)
    {
        if (trimId == null)
        {
            // Create: need model dropdown from VehicleService (build from all models present in vehicles)
            var vehicles = await _vehicleService.GetAllVehicles();
            var modelOptions = vehicles
                .GroupBy(v => new { v.ModelId, v.ModelName })
                .OrderBy(g => g.Key.ModelName)
                .Select(g => new { g.Key.ModelId, g.Key.ModelName })
                .Distinct()
                .ToList();

            ViewBag.ModelList = new SelectList(modelOptions, "ModelId", "ModelName");
            ViewBag.IsEdit = false;
            return View(new EditVehicleTrimViewModel());
        }

        // Edit: fetch trim via VehicleService
        var trim = await _vehicleService.GetVehicleById(trimId.Value); // <-- via VehicleService
        if (trim == null) return NotFound();

        // We might also want the model name for read-only display:
        var vehicleDto = await _vehicleService.GetVehicleById(trim.VehicleId); // has ModelName
        ViewBag.ModelName = vehicleDto?.ModelName ?? "Unknown";

        var vm = new EditVehicleTrimViewModel
        {
            EvTrimId = trim.VehicleId,
            EvModelId = trim.ModelId, // fixed on edit
            TrimName = trim.TrimName,
            ModelYear = trim.ModelYear,
            Description = trim.Specifications.TryGetValue("Description", out string? value) ? value : "N/A",
        };

        ViewBag.IsEdit = true;
        return View(vm);
    }

    [Authorize(Roles = "DISTRIBUTOR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTrim(EditVehicleTrimViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            if (vm.EvTrimId == 0)
            {
                // re-populate model list for create mode
                var vehicles = await _vehicleService.GetAllVehicles();
                var modelOptions = vehicles
                    .GroupBy(v => new { v.ModelId, v.ModelName })
                    .OrderBy(g => g.Key.ModelName)
                    .Select(g => new { g.Key.ModelId, g.Key.ModelName })
                    .Distinct()
                    .ToList();

                ViewBag.ModelList = new SelectList(modelOptions, "ModelId", "ModelName");
                ViewBag.IsEdit = false;
            }
            else
            {
                // edit mode: model fixed – show its name again
                var dto = await _vehicleService.GetVehicleById(vm.EvTrimId);
                ViewBag.ModelName = dto?.ModelName ?? "Unknown";
                ViewBag.IsEdit = true;
            }
            return View(vm);
        }

        if (vm.EvTrimId == 0)
        {
            // Create via VehicleService
            var dto = new CreateVehicleTrimDto
            {
                EvModelId = vm.EvModelId,
                TrimName = vm.TrimName,
                ModelYear = vm.ModelYear,
                Description = vm.Description,
                ListedPrice = vm.NewListedPrice
            };
            await _vehicleService.CreateVehicleTrimAsync(dto, ct);
            TempData["Success"] = "Vehicle trim created.";
        }
        else
        {
            // Update basic fields of trim via VehicleService.
            // If you have UpdateVehicleTrimAsync, call it here.
            // Otherwise, at least handle price updates using existing service:
            if (vm.NewListedPrice.HasValue && vm.NewListedPrice.Value > 0)
            {
                await _vehicleService.UpdateVehicleTrimPriceAsync(new UpdateVehicleTrimPriceDto
                {
                    EvTrimId = vm.EvTrimId,
                    NewListedPrice = vm.NewListedPrice.Value,
                    EffectiveDate = System.DateTime.UtcNow
                }, ct);
            }

            // For updating TrimName/Year/Description strictly through VehicleService,
            // add a method there (e.g., UpdateVehicleTrimAsync) and call it here.

            TempData["Success"] = "Vehicle trim updated.";
        }

        return RedirectToAction("Index", "Vehicle");
    }
}
