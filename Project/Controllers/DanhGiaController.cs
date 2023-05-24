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
        public ActionResult Create(string cMessage, int MaTinTuc)
        {

            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            if (kh == null)
            {
                TempData["Error"] = "Bạn phải đăng nhập";
                return Redirect("/Home/Product_Details/" + MaTinTuc);
            };
            string data = Session["TaiKhoan"].ToString();
            string[] Account = new string[3];
            Account = (data != null) ? data.Split(',') : Account;
            DanhGia cmt = new DanhGia();
            cmt.NoiDung = cMessage;
            cmt.masp = MaTinTuc;
            cmt.makh = kh.makh;
            cmt.NgayTao = DateTime.Now;
            cmt.NgaySua = DateTime.Now;
            cmt.Trangthai = true;
            db.DanhGias.InsertOnSubmit(cmt);
            db.SubmitChanges();
            return Redirect("/Home/Product_Details/" + MaTinTuc);
        }
    }
}