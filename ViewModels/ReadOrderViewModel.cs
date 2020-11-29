using OnlineShoppingStore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace OnlineShoppingStore.ViewModels
{
    public class ReadOrderViewModel
    {
        public Tbl_Members Member { get; set; }
        public Tbl_ShippingDetails ShippingDetails { get; set; }
        public Tbl_Orders Orders { get; set; }
        public List<Tbl_OrderProducts> OrderProducts { get; set; }
    }
}