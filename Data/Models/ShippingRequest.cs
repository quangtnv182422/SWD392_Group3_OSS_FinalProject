﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingSystem_Main.Models
{
    public class ShippingRequest
    {
        public int fromDistrictId { get; set; }
        public int toDistrictId { get; set; }
        public int weight { get; set; }
        public int length { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int service_id { get; set; }
        public int service_type_id { get; set; }
        public int shopId { get; set; }
    }
}
