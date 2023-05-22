using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ThongKeSoLuongTonViewModel
    {
        public List<ProductInventory> ProductInventoryList { get; set; }
        public int TotalInventory { get; set; }
    }

    public class ProductInventory
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public int SoLuongTon { get; set; }
    }
}
