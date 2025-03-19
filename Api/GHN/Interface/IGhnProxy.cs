using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models.GHN;

namespace Api.GHN.Interface
{
    public interface IGhnProxy
    {
        Task<string> GetProvincesAsync();
        Task<string> GetDistrictsAsync(int provinceId);
        Task<string> GetWardsAsync(int districtId);
        Task<string> GetAvailableServicesAsync(int shopId, int fromDistrictId, int toDistrictId);
        Task<string> CalculateShippingFeeAsync(int shopId, int fromDistrictId, int toDistrictId, int weight, int length, int width, int height, int serviceId, int serviceTypeId);
        Task<string> UpdateOrderOnGHNAsync(GhnOrderUpdateRequest request);
        Task<string> SendShippingOrderAsync(ShippingOrder order);

        Task<GhnOrderDetailResponse> GetOrderDetailsFromGhnAsync(string orderCode);

        Task<bool> CancelOrderOnGhnAsync(string orderCode);

    }
}
