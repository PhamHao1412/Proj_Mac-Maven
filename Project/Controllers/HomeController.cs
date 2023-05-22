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
            // Loại bỏ khoảng trắng ở đầu và cuối chuỗi tìm kiếm
            searchTerm = searchTerm.Trim();

            // Tìm vị trí của khoảng trắng đầu tiên trong chuỗi tìm kiếm
            int firstSpaceIndex = searchTerm.IndexOf(' ');

            // Lấy từ khóa đầu tiên từ đầu chuỗi tới vị trí khoảng trắng đầu tiên (nếu có)
            string firstKeyword = firstSpaceIndex >= 0 ? searchTerm.Substring(0, firstSpaceIndex) : searchTerm;

            // Thực hiện tìm kiếm trong cơ sở dữ liệu dựa trên từ khóa đầu tiên
            var results = db.Items
                .Where(item => item.ten.Contains(firstKeyword))
                .Select(item => new
                {
                    id = item.ma,
                    name = item.ten,
                    hinh = item.hinh,
                    giaban = item.giaban,
                    giamgia = item.giamgia
                })
                .ToList()
                .Take(4);

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
            var comments = (from d in db.DanhGias
                            join k in db.KhachHangs on d.makh equals k.makh
                            where d.masp == id && d.Trangthai == true
                            select new DanhGiaModel
                            {
                                Id = d.id,
                                NoiDung = d.NoiDung,
                                NgayTao = (DateTime)d.NgayTao,
                                NgaySua = (DateTime)d.NgaySua,
                                khachhang = new KhachHangModel
                                {
                                    Ho = k.ho,
                                    Ten = k.Ten
                                },
                            }).ToList();
            ViewBag.Comments = comments;
            ViewBag.countReview = db.DanhGias.Where(i => i.masp == id).Count();
            return View(item);
        }


        [ChildActionOnly]
        public ActionResult MenuView()
        {
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            if (kh != null)
            {
                ViewBag.TotalPrice = Total(kh.makh);
                ViewBag.CountCart = db.GioHangs.Where(k => k.makh == kh.makh).Count();

            }
            var menu = db.Loais.ToList();
            return PartialView(menu);
        }



    }
}