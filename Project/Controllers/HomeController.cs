using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace Project.Controllers
{
    public class HomeController : Controller
    {
        AppleDataDataContext db = new AppleDataDataContext();
  
        private int CountProductIDs(int? makh)
        {
            int count = 0;
            var gioHangList = db.GioHangs;
            var idList = gioHangList.Where(g => g.makh == makh).Select(g => g.masp).ToList();
            count = idList.Distinct().Count();
            return count;
        }
        private decimal Total(int makh)
        {
            decimal total = 0;
            var giohang = db.GioHangs.Where(g => g.makh == makh);
            if (giohang != null)
            {
                total = (giohang?.Sum(g => g.tongtien) ?? 0);
            }
            return total;
        }
        public ActionResult Index()
        {
            ViewBag.iPhone = db.Items.Where(s => s.maloai == 1).Take(4).ToList();
            ViewBag.mac = db.Items.Where(s => s.maloai == 2).Take(4).ToList();

            return View(db.Items.ToList());
        }
        public ActionResult Store()
        {
            ViewBag.iPhone = db.Items.Where(s => s.maloai == 1).Take(4).ToList();
            ViewBag.mac = db.Items.Where(s => s.maloai == 2).Take(4).ToList();
            ViewBag.iPad = db.Items.Where(s => s.maloai == 3).Take(4).ToList();
            ViewBag.Watch = db.Items.Where(s => s.maloai == 4).Take(4).ToList();
            ViewBag.Airpods = db.Items.Where(s => s.maloai == 5).Take(4).ToList();
            return View();
        }
        [HttpPost]
        public JsonResult Search(string searchTerm)
        {
            // Thực hiện tìm kiếm trong cơ sở dữ liệu dựa trên searchTerm
            var results = db.Items.Where(item => item.ten.Contains(searchTerm)).Select(item => new
            {
                id = item.ma,
                name = item.ten,
                hinh = item.hinh,
                giaban = item.giaban,
                giamgia = item.giamgia
            }).ToList().Take(4);

            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Product_Details(int id)
        {
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            var item = db.Items.FirstOrDefault(d => d.ma == id);
            ViewBag.HinhAnh = db.HinhAnhs.Where(s => s.ma == id).ToList();
            ViewBag.CountCart = CountProductIDs(kh?.makh); 

            return View(item);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [ChildActionOnly]
        public ActionResult MenuView()
        {
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            if (kh != null) 
            {
                ViewBag.CountCart = CountProductIDs(kh.makh);
                ViewBag.TotalPrice = Total(kh.makh);

            }
            var menu = db.Loais.ToList();
            return PartialView(menu);
        }



    }
}