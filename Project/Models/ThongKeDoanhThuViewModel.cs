using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ThongKeDoanhThuViewModel
    {
        public int Year { get; set; }
        public int WeekNum { get; set; }
      
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

}