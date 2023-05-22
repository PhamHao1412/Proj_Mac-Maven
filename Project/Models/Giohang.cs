using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Project.Models
{
    public class Giohang
    {
        AppleDataDataContext data = new AppleDataDataContext();

        public int ma { get; set; }
        [Display(Name = "Tên sách")]
        public string ten { get; set; }
        [Display(Name = "Ảnh bìa")]
        public string hinh { get; set; }
        [Display(Name = "Giá bán")]
        public Double giaban { get; set; }
        [Display(Name = "Số lượng")]
        public int isoLuong { get; set; }
        [Display(Name = "Thành tiền")]
        public Double dthanhtien
        {
            get { return isoLuong * giaban; }
        }
        public Giohang(int id)
        {
            ma = id;
            Item item = data.Items.Single(n => n.ma == ma);
            ten = item.ten;
            hinh = item.hinh;
            giaban = double.Parse(item.giaban.ToString());
            isoLuong = 1;
        }

    }
}