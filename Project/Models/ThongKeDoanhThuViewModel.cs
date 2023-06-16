using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ThongKeDoanhThuViewModel
    {
        public int ProductID { get; set; }
        public decimal TotalQuantity { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int WeekNum { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDateOfWeek { get; set; }
        public DateTime EndDateOfWeek { get; set; }
        public DateTime StartDateOfMonth { get; set; }
        public DateTime EndDateOfMonth { get; set; }
        public ThongKeDoanhThuViewModel()
        {
            // Khởi tạo giá trị mặc định cho WeekNum
            WeekNum = GetIso8601WeekOfYear(DateTime.Now);
        }

        private int GetIso8601WeekOfYear(DateTime now)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(now);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                now = now.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

    }
    public class ThongKeDoanhThuViewData
    {
        public List<RevenueStatistics> RevenueStatistics { get; set; }
        public List<List<BestSellingProduct>> BestSellingProducts { get; set; }
    }
  

    public class BestSellingProduct
    {
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public int TongSoLuong { get; set; }
        public decimal TongTienSP { get; set; }
    }

}
