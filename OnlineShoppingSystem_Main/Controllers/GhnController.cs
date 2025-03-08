﻿using Microsoft.AspNetCore.Mvc;
using OnlineShoppingSystem_Main.Models;
using Service;



namespace OnlineShoppingSystem_Main.Controllers
{
    [Route("api/ghn")]
    [ApiController]
    public class GhnController : ControllerBase
    {
        private readonly GhnApiService _ghnApiService;

        public GhnController(GhnApiService ghnApiService)
        {
            _ghnApiService = ghnApiService;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await _ghnApiService.GetProvincesAsync();
            return Content(result, "application/json");
        }

        [HttpGet("districts/{provinceId}")]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            var result = await _ghnApiService.GetDistrictsAsync(provinceId);
            return Content(result, "application/json");
        }

        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWards(int districtId)
        {
            var result = await _ghnApiService.GetWardsAsync(districtId);
            return Content(result, "application/json");
        }

        [HttpPost("calculate-shipping")]
        public async Task<IActionResult> CalculateShipping([FromBody] ShippingRequest request)
        {
            try
            {
                var result = await _ghnApiService.CalculateShippingFeeAsync(
                    request.shopId,
                    request.fromDistrictId,
                    request.toDistrictId,
                    request.weight,
                    request.length,
                    request.width,
                    request.height,
                    request.service_id,
                    request.service_type_id);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("available-services")]
        public async Task<IActionResult> GetAvailableServices([FromBody] AvailableServicesRequest request)
        {
            try
            {
                var result = await _ghnApiService.GetAvailableServicesAsync(
                    request.shopId,
                    request.fromDistrictId,
                    request.toDistrictId);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}