using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Models.GHN
{
    public class GhnOrderUpdateRequest
    {

        [JsonPropertyName("order_code")]
        public string OrderCode { get; set; }

        [JsonPropertyName("to_name")]
        public string ToName { get; set; }

        [JsonPropertyName("to_phone")]
        public string ToPhone { get; set; }

        [JsonPropertyName("to_address")]
        public string ToAddress { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("items")]
        public List<GhnOrderItem> Items { get; set; } = new List<GhnOrderItem>();
    }
}

