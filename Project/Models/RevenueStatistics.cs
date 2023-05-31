using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class RevenueStatistics
    {
        public DateTime NgayGiao { get; set; }
        public decimal TongTien { get; set; }
        public List<BestSellingProduct> BestSellingProducts { get; set; }

    }

}