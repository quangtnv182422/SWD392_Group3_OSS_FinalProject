﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.GHN.Interface
{
    public interface IGhnService
    {
        Task<string> GetProvincesAsync();
        Task<string> GetDistrictsAsync(int provinceId);
        Task<string> GetWardsAsync(int districtId);
        Task<string> GetAvailableServicesAsync(int shopId, int fromDistrictId, int toDistrictId);
        Task<string> CalculateShippingFeeAsync(int shopId, int fromDistrictId, int toDistrictId, int weight, int length, int width, int height, int serviceId, int serviceTypeId);
    }
}
