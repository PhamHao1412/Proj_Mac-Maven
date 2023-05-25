using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class DanhGiaController : Controller
    {
        // GET: DanhGia
        AppleDataDataContext db = new AppleDataDataContext();
        public ActionResult Create(string cMessage, int MaSP, int xephang)
        {

            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            if (kh == null)
            {
                TempData["Error"] = "Bạn phải đăng nhập";
                return Redirect("/Home/Product_Details/" + MaSP);
            };
            string data = Session["TaiKhoan"].ToString();
            string[] Account = new string[3];
            Account = (data != null) ? data.Split(',') : Account;
            DanhGia cmt = new DanhGia();
            cmt.NoiDung = cMessage;
            cmt.masp = MaSP;
            cmt.makh = kh.makh;
            cmt.NgayTao = DateTime.Now;
            cmt.NgaySua = DateTime.Now;
            cmt.Trangthai = true;
            cmt.XepHang = xephang;
            db.DanhGias.InsertOnSubmit(cmt);
            db.SubmitChanges();
            return Redirect("/Home/Product_Details/" + MaSP);
        }
    }
}