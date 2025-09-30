using ASM_01.BusinessLayer.DTOs;
using ASM_01.BusinessLayer.Services;
using ASM_01.BusinessLayer.Services.Abstract;
using ASM_01.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASM_01.WebApp.Controllers;

[Authorize]
public class VehicleController(IVehicleService vehicleService) : Controller
{

    // GET: /Vehicle/Catalog
    public async Task<IActionResult> Catalog(string? searchTerm)
    {
        if (searchTerm != null)
        {
            var searchDtos = await vehicleService.SearchVehicles(searchTerm);
            var searchResults = searchDtos.Select(dto => new VehicleCatalogViewModel
            {
                VehicleId = dto.VehicleId,
                ModelId = dto.ModelId,
                ModelName = dto.ModelName,
                Description = dto.Description,
                TrimName = dto.TrimName,
                Status = dto.Status,
                Price = dto.Price
            }).ToList();
            return View(searchResults); // VehicleCatalog.cshtml
        }
        var dtos = await vehicleService.GetAllVehicles();
        var vehicleCatalogVMs = dtos.Select(dto => new VehicleCatalogViewModel
        {
            ModelId = dto.ModelId,
            VehicleId = dto.VehicleId,
            ModelName = dto.ModelName,
            TrimName = dto.TrimName,
            Description = dto.Description,
            Status = dto.Status,
            Price = dto.Price
        }).ToList();
        return View(vehicleCatalogVMs); // VehicleCatalog.cshtml
    }

    // GET: /Vehicle/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var dto = await vehicleService.GetVehicleById(id);
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
            Description = dto.Description,
            Price = dto.Price,
            Status = dto.Status,
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

        var dtos = await vehicleService.CompareVehicles(ids);
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

        var model = await vehicleService.GetModelAsync(modelId.Value); // <-- via VehicleService
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
            try
            {
                await vehicleService.CreateVehicleModelAsync(dto, ct);
                TempData["Success"] = "Vehicle model created.";
            }
            catch(InvalidOperationException ex)
            {
                ViewBag.IsEdit = false;
                ViewBag.Error = ex.Message;
                return View(vm);
            }
        }
        else
        {
            // Update — using VehicleService only.
            // If you have an UpdateVehicleModelAsync, call it here.
            // With the current service, we can at least update Status:
            try
            {
                await vehicleService.UpdateVehicleModelStatusAsync(
                new UpdateVehicleModelStatusDto { EvModelId = vm.EvModelId, Status = vm.Status, Description = vm.Description }, ct);

            }catch(InvalidOperationException ex)
            {
                ViewBag.IsEdit = true;
                ViewBag.Error = ex.Message;
                return View(vm);
            }

            TempData["Success"] = "Vehicle model updated.";
        }

        return RedirectToAction("Catalog", "Vehicle");
    }

    [Authorize(Roles = "DISTRIBUTOR")]
    [HttpGet]
    public async Task<IActionResult> EditTrim(int? trimId, CancellationToken ct)
    {
        if (trimId == null)
        {
            var vehicles = await vehicleService.GetAllModel();
            var modelOptions = vehicles
                .GroupBy(v => new { v.EvModelId, v.ModelName })
                .OrderBy(g => g.Key.ModelName)
                .Select(g => new { g.Key.EvModelId, g.Key.ModelName })
                .Distinct()
                .ToList();

            ViewBag.ModelList = new SelectList(modelOptions, "EvModelId", "ModelName");
            ViewBag.IsEdit = false;
            return View(new EditVehicleTrimViewModel());
        }

        // Edit: fetch trim via VehicleService
        var trim = await vehicleService.GetVehicleById(trimId.Value);
        if (trim == null) return NotFound();

        // We might also want the model name for read-only display:
        var vehicleDto = await vehicleService.GetVehicleById(trim.VehicleId); // has ModelName
        ViewBag.ModelName = vehicleDto?.ModelName ?? "Unknown";

        var vm = new EditVehicleTrimViewModel
        {
            EvTrimId = trim.VehicleId,
            EvModelId = trim.ModelId,
            TrimName = trim.TrimName,
            ModelYear = trim.ModelYear,
            Description = trim.Description
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
                var vehicles = await vehicleService.GetAllVehicles();
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
                var dto = await vehicleService.GetVehicleById(vm.EvTrimId);
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
            try
            {
                await vehicleService.CreateVehicleTrimAsync(dto, ct);
                TempData["Success"] = "Vehicle trim created.";
            }catch(Exception ex)
            {
                // Could be InvalidOperationException from service, or DbUpdateException from EF Core
                ViewBag.IsEdit = false;
                ViewBag.Error = ex.Message;
                // re-populate model list for create mode
                var vehicles = await vehicleService.GetAllVehicles();
                var modelOptions = vehicles
                    .GroupBy(v => new { v.ModelId, v.ModelName })
                    .OrderBy(g => g.Key.ModelName)
                    .Select(g => new { g.Key.ModelId, g.Key.ModelName })
                    .Distinct()
                    .ToList();
                ViewBag.ModelList = new SelectList(modelOptions, "ModelId", "ModelName");
                return View(vm);
            }
        }
        else
        {
            // Update basic fields of trim via VehicleService.
            // If you have UpdateVehicleTrimAsync, call it here.
            // Otherwise, at least handle price updates using existing service:
            if (vm.NewListedPrice.HasValue && vm.NewListedPrice.Value > 0)
            {
                try
                {
                    await vehicleService.UpdateVehicleTrimPriceAsync(new UpdateVehicleTrimPriceDto
                    {
                        EvTrimId = vm.EvTrimId,
                        NewListedPrice = vm.NewListedPrice.Value,
                        EffectiveDate = System.DateTime.UtcNow
                    }, ct);
                }
                catch (Exception ex)
                {
                    ViewBag.IsEdit = true;
                    ViewBag.Error = ex.Message;
                    // show model name again
                    var dto = await vehicleService.GetVehicleById(vm.EvTrimId);
                    ViewBag.ModelName = dto?.ModelName ?? "Unknown";
                    return View(vm);
                }
            }

            // For updating TrimName/Year/Description strictly through VehicleService,
            // add a method there (e.g., UpdateVehicleTrimAsync) and call it here.

            TempData["Success"] = "Vehicle trim updated.";
        }

        return RedirectToAction("Catalog", "Vehicle");
    }
}
