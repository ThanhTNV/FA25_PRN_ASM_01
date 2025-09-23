using ASM_01.BusinessLayer.DTOs;
using ASM_01.BusinessLayer.Services;
using ASM_01.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_01.WebApp.Controllers
{
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
                    ModelName = dto.ModelName,
                    TrimName = dto.TrimName,
                    Price = dto.Price
                }).ToList();
                return View(searchResults); // VehicleCatalog.cshtml
            }
            var dtos = await _vehicleService.GetAllVehicles();
            var vehicleCatalogVMs = dtos.Select(dto => new VehicleCatalogViewModel
            {
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
    }

}
