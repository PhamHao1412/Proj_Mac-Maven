using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class LichSuMuaHangModel
    {
        public int SoLuong { get; set; }
        public string TenSP { get; set; }
        public int Ma { get; set; }
        public int MaDon { get; set; }
        public int TongTien { get; set; }
        public int DonGia { get; set; }
        public KhachHangModel khachhang { get; set; }
        public int MaKh { get; set; }
        public DateTime NgayDat { get; set; }
    }
}