using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.GHN
{
    public class GHNSettings
    {
        public string Token { get; set; }
        public string BaseUrl { get; set; }
        public string TestBaseUrl { get; set; }
		public string ShopId { get; set; }
		public GHNEndpoints Endpoints { get; set; }
    }
}
