using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.GHN
{
    public class AvailableServicesRequest
    {
        public int shopId { get; set; }
        public int fromDistrictId { get; set; }
        public int toDistrictId { get; set; }
    }
}
