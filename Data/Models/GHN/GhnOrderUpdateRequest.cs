using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.GHN
{
    public class GhnOrderUpdateRequest
    {

        public string OrderCode { get; set; }
        public string ToName { get; set; } 
        public string ToPhone { get; set; } 
        public string ToAddress { get; set; } 
        public string Note { get; set; } 
    }
}

