using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class DanhGiaModel
    {
        public int Id { get; set; }
        public string NoiDung { get; set; }
        public string TinTuc { get; set; }
        public string TaiKhoan { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgaySua { get; set; }
        public int XepHang { get; set; }

        public bool TrangThai { get; set; }
        public KhachHangModel khachhang { get; set; }
    }
}