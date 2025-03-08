using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingSystem_Main.Models
{
    public class OrderConfirmationViewModel
    {
        public List<CartItem> SelectedCartItems { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }

        public int SelectedProvince { get; set; }
        public int SelectedDistrict { get; set; }
        public int SelectedWard { get; set; }
        public string PaymentMethod {  get; set; }
        public string DeliveryNotes {  get; set; }
        public double SubTotal { get; set; }
        public double ShippingFee { get; set; }
        public double TotalCost => SubTotal + ShippingFee;
    }

}
