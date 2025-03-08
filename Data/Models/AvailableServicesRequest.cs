using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingSystem_Main.Models
{
    public class AvailableServicesRequest
    {
        public int shopId { get; set; }
        public int fromDistrictId { get; set; }
        public int toDistrictId { get; set; }
    }
}
