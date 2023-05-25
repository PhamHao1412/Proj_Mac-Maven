using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class DanhMucController : Controller
    {
        // GET: DanhMuc
        AppleDataDataContext db = new AppleDataDataContext();
        public ActionResult Index(int page = 1, int id = 0)
        {
            var data = db.Loais.FirstOrDefault(d => d.maloai == id);
            if (data == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Title = data.tenloai;
            //var rs = (from tt in db.Items
            //          join dm in db.Loais on tt.maloai equals dm.maloai
            //          orderby tt.ma descending
            //          where tt.maloai == id
            //          select new Store_Category
            //          {
            //              DanhMuc = dm.tenloai,
            //              MaLoai = dm.maloai,
            //              MaSP = tt.ma,
            //              TenSP = tt.ten,
            //              Hinh = tt.hinh,
            //              GiaBan = tt.giaban != null ? (int)tt.giaban : 0,
            //              GiamGia = tt.giamgia != null ? (int)tt.giamgia : 0,
            //              SoLuongLike = tt.SoLuongLike != null ? (int)tt.SoLuongLike : 0
            //          }).ToPagedList(page, 12);
            var rs = (from tt in db.Items
                      join dm in db.Loais on tt.maloai equals dm.maloai
                      orderby tt.ma descending
                      where tt.maloai == id
                      select new Store_Category
                      {
                          DanhMuc = dm.tenloai,
                          MaLoai = dm.maloai,
                          MaSP = tt.ma,
                          TenSP = tt.ten,
                          Hinh = tt.hinh,
                          GiaBan = tt.giaban ?? 0,           // Use the null-coalescing operator (??) to handle null values
                          GiamGia = tt.giamgia ?? 0,
                          SoLuongLike = tt.SoLuongLike ?? 0  // Adjust the nullable fields to avoid arithmetic overflow
                      }).ToPagedList(page, 12);
            return View(rs);
        }
    }
}