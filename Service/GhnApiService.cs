using System.Text;
using System.Text.Json;


namespace Service
{
    public class GhnApiService
    {
        private readonly HttpClient _httpClient;

        public GhnApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Token", "d7291c5c-f3f4-11ef-9f11-72d076f64f74");
        }

        public async Task<string> GetProvincesAsync()
        {
            var response = await _httpClient.GetAsync("https://online-gateway.ghn.vn/shiip/public-api/master-data/province");

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] Response from GHN API: {result}");

            return result;
        }


        public async Task<string> GetDistrictsAsync(int provinceId)
        {
            var jsonBody = JsonSerializer.Serialize(new { province_id = provinceId });
            var response = await _httpClient.PostAsync("https://online-gateway.ghn.vn/shiip/public-api/master-data/district",
                new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetWardsAsync(int districtId)
        {
            var jsonBody = JsonSerializer.Serialize(new { district_id = districtId });
            var response = await _httpClient.PostAsync("https://online-gateway.ghn.vn/shiip/public-api/master-data/ward",
                new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json"));
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

            var response = await _httpClient.PostAsync(
                "https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/available-services",
                new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] Available Services Response: {result}");

            return result;
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

            var response = await _httpClient.PostAsync(
                "https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee",
                new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] Response from GHN API: {result}");

            return result;
        }

    }
}