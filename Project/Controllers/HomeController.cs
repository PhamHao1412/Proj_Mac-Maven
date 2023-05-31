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
        public IEnumerable<object> GetItemsByLoai(string loai)
        {
            var query = (from l in db.Loais
                        join i in db.Items on l.maloai equals i.maloai
                        where l.tenloai == loai
                        select new Store_Category
                        {
                           MaSP = i.ma,
                           TenSP= i.ten,
                           Hinh =i.hinh,
                           GiaBan=(int) i.giaban,
                           GiamGia=(int) i.giamgia,
                           MaLoai =l.maloai
                        }).Take(4);

            return query;
        }
        public ActionResult Index()
        {
            // Set the cookie
           
            // Retrieve and set other view data
            ViewBag.iPhone = db.Items.Where(s => s.maloai == 1).Take(4).ToList();
            ViewBag.mac = db.Items.Where(s => s.maloai == 2).Take(4).ToList();

            return View(db.Items.ToList());
        }

        public ActionResult Store()
        {
            var query = (from l in db.Loais
                         join i in db.Items on l.maloai equals i.maloai
                         where i.maloai == 1
                         select l.tenloai).Distinct();

            ViewBag.iPhone = GetItemsByLoai("iPhone");
            ViewBag.mac = GetItemsByLoai("mac");
            ViewBag.iPad = GetItemsByLoai("iPad");
            ViewBag.Watch = GetItemsByLoai("Watch");
            ViewBag.Airpods = GetItemsByLoai("Airpods");

            return View();
        }
        public ActionResult Search(string searchTerm)
        {
            // Loại bỏ khoảng trắng ở đầu và cuối chuỗi tìm kiếm
            searchTerm = searchTerm.Trim();

            // Tách các từ khóa từ chuỗi tìm kiếm
            string[] keywords = searchTerm.Split(' ');

            // Lấy từ khóa đầu tiên
            string firstKeyword = keywords[0];

            // Lấy từ khóa thứ hai (nếu có)
            string secondKeyword = keywords.Length > 1 ? keywords[1] : "";

            // Thực hiện tìm kiếm trong cơ sở dữ liệu dựa trên từ khóa đầu tiên và thứ hai
            var queryResults = db.Items
                .Where(item => item.ten.Contains(firstKeyword) && item.ten.Contains(secondKeyword))
                .Select(item => new Store_Category
                {
                    MaSP = item.ma,
                    TenSP = item.ten,
                    Hinh = item.hinh,
                    GiaBan = (int)item.giaban,
                    GiamGia = (int)item.giamgia
                });
            var jsonResult = queryResults
                .Take(4)
                .ToList();
            var viewResult = queryResults.ToList();
            ViewBag.SearchTerm = searchTerm; // Gửi searchTerm để hiển thị trong view

            if (Request.IsAjaxRequest())
            {
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View(viewResult);
            }
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
                                XepHang = (int)d.XepHang,
                                khachhang = new KhachHangModel
                                {
                                    Id = (int)d.makh,
                                    Ho = k.ho,
                                    Ten = k.Ten
                                },
                            }).ToList();
            ViewBag.Comments = comments;
            //Đếm số lượt đánh giá tại mã sp
            ViewBag.countReview = db.DanhGias.Where(i => i.masp == id).Count();
            //Lấy sô lượt đánh giá gần đây
            ViewBag.recentReview = (from dg in db.DanhGias
                                    where dg.NgayTao == (from dg2 in db.DanhGias
                                                         where dg2.masp == id
                                                         select dg2.NgayTao).Max()
                                    select dg.XepHang).FirstOrDefault();


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
        public ActionResult MenuViewAdmin()
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