using Api.GHN.Interface;
using Data.Models;
using Data.Models.GHN;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace Api.GHN.Implementation
{
    public class GhnApiProxy : IGhnProxy
    {
        private readonly HttpClient _httpClient;
        private readonly GHNSettings _ghnSettings;

        public GhnApiProxy(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _ghnSettings = configuration.GetSection("GHNSettings").Get<GHNSettings>();
            _httpClient.DefaultRequestHeaders.Add("Token", _ghnSettings.Token);
        }

        public async Task<string> GetProvincesAsync()
        {
            var response = await _httpClient.GetAsync($"{_ghnSettings.BaseUrl}{_ghnSettings.Endpoints.Provinces}");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetDistrictsAsync(int provinceId)
        {
            var jsonBody = JsonSerializer.Serialize(new { province_id = provinceId });
            var response = await _httpClient.PostAsync($"{_ghnSettings.BaseUrl}{_ghnSettings.Endpoints.Districts}",
                new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetWardsAsync(int districtId)
        {
            var jsonBody = JsonSerializer.Serialize(new { district_id = districtId });
            var response = await _httpClient.PostAsync($"{_ghnSettings.BaseUrl}{_ghnSettings.Endpoints.Wards}",
                new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAvailableServicesAsync(int shopId, int fromDistrictId, int toDistrictId)
        {
            var jsonBody = JsonSerializer.Serialize(new
            {
                shop_id = shopId,
                from_district = fromDistrictId,
                to_district = toDistrictId
            });
            var response = await _httpClient.PostAsync($"{_ghnSettings.BaseUrl}{_ghnSettings.Endpoints.AvailableServices}",
                new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CalculateShippingFeeAsync(int shopId, int fromDistrictId, int toDistrictId, int weight, int length, int width, int height, int serviceId, int serviceTypeId)
        {
            var jsonBody = JsonSerializer.Serialize(new
            {
                shop_id = shopId,
                from_district_id = fromDistrictId,
                to_district_id = toDistrictId,
                weight,
                length,
                width,
                height,
                service_id = serviceId,
                service_type_id = serviceTypeId
            });
            var response = await _httpClient.PostAsync($"{_ghnSettings.BaseUrl}{_ghnSettings.Endpoints.ShippingFee}",
                new StringContent(jsonBody, Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UpdateOrderOnGHNAsync(GhnOrderUpdateRequest request)
        {
            var jsonBody = JsonSerializer.Serialize(request);
            var response = await _httpClient.PostAsync($"{_ghnSettings.BaseUrl}/v2/shipping-order/update",
                new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            return await response.Content.ReadAsStringAsync();
        }
		public async Task<string> SendShippingOrderAsync(ShippingOrder order)
		{
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
                        var json = JsonSerializer.Serialize(order, options);

			var response = await _httpClient.PostAsync($"{_ghnSettings.BaseUrl}{_ghnSettings.Endpoints.CreateOrder}",
				new StringContent(json, Encoding.UTF8, "application/json"));
			return await response.Content.ReadAsStringAsync();


			
		}

	}
}