using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class SanPham
    {
        public int Ma { get; set; }
        public int MaLoai { get; set; }
        public string Ten { get; set; }
        public string Hinh { get; set; }
        public decimal GiaBan { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int SoLuongTon { get; set; }
        public decimal GiamGia { get; set; }
        public int SoLuongLike { get; set; }
    }
}