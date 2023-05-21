using Project.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        AppleDataDataContext db = new AppleDataDataContext();
        public ActionResult Index()
        {
            var all_user = from s in db.KhachHangs select s;
            ViewBag.User = all_user.ToList();
            var result = (from nv in db.NhanViens
                          join cv in db.ChucVus on nv.MaCV equals cv.MaCV
                          select new StaffInfo { MaNV = nv.MaNV, Ten = nv.Ten, Ho = nv.Ho, TenCV = cv.TenCV }).ToList();

            ViewBag.Staff = result;
            return View();
        }
        public ActionResult Add_Products()
        {
            return View();
        }

        // Hàm tính ngày đầu tiên trong tuần theo số tuần và năm
        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)DayOfWeek.Monday - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);
            return firstMonday.AddDays(7 * (weekOfYear - 1));
        }


        public ActionResult ThongKeDoanhThu(int? year, int? weekNum)
        {
            int currentYear = DateTime.Now.Year;
            int currentWeekNum = GetIso8601WeekOfYear(DateTime.Now);

            int selectedYear = year ?? currentYear;
            int selectedWeekNum = weekNum ?? currentWeekNum;

            DateTime selectedDate = FirstDateOfWeekISO8601(selectedYear, selectedWeekNum);

            // Lấy danh sách doanh thu của các ngày trong tuần
            DateTime monday = selectedDate.AddDays(-(int)selectedDate.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime sunday = monday.AddDays(6);

            var viewModel = new ThongKeDoanhThuViewModel
            {
                Year = selectedYear,
                WeekNum = selectedWeekNum
            };

            var revenueList = db.DonHangs
                .Where(dh => dh.ngaygiao != null && dh.trangthai == "Hoàn tất" && dh.ngaygiao >= monday && dh.ngaygiao <= sunday)
                .Join(db.ChiTietDonHangs, dh => dh.madon, ctdh => ctdh.madon, (dh, ctdh) => new { dh.ngaygiao, ctdh.tongtien })
                .GroupBy(x => x.ngaygiao)
                .Select(g => new RevenueStatistics
                {
                    NgayGiao = (DateTime)g.Key,
                    TongTien = (decimal)g.Sum(ctdh => ctdh.tongtien)
                })
                .ToList();

            // Tính tổng doanh thu của tuần
            decimal tongDoanhThuTuan = revenueList.Sum(r => r.TongTien);

            // Truyền danh sách doanh thu và tổng doanh thu của tuần vào view
            ViewBag.RevenueStatistics = revenueList;
            ViewBag.TongDoanhThuTuan = tongDoanhThuTuan;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedWeekNum = selectedWeekNum;

            return View(viewModel);
        }

        // Hàm tính ngày đầu tiên trong tuần theo số tuần và năm
        private int GetIso8601WeekOfYear(DateTime now)
        {
            var culture = CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;

            // Get the week number for the current date based on the current culture's calendar
            int weekNum = calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            // Adjust the week number for ISO 8601, which defines the first week of the year as the week
            // containing the first Thursday of the year
            var jan1 = new DateTime(now.Year, 1, 1);
            int jan1DayOfWeek = (int)jan1.DayOfWeek;
            int daysToAdd = (int)DayOfWeek.Thursday - jan1DayOfWeek;
            if (jan1DayOfWeek > 4)
                daysToAdd += 7;

            var firstThursday = jan1.AddDays(daysToAdd);
            if (now < firstThursday)
            {
                // The date is before the start of week 1
                weekNum = calendar.GetWeekOfYear(firstThursday.AddDays(-7), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }

            return weekNum;
        }
        public ActionResult EditKhachHang(int makh)
        {

            var E_khachhang = db.KhachHangs.First(m => m.makh == makh);

            return View(E_khachhang);
        }
        [HttpPost]
        public ActionResult EditKhachHang(int makh, FormCollection collection)
        {
            var E_user = db.KhachHangs.First(m => m.makh == makh);
            var E_ho = collection["Ho"];
            var E_ten = collection["Ten"];
            var E_tendangnhap = collection["tendangnhap"];
            var E_matkhau = collection["matkhau"];
            var E_email = collection["email"];
            var E_diachi = collection["diachi"];
            var E_dienthoai = collection["dienthoai"];
            var E_ngaysinh = Convert.ToDateTime(collection["ngaysinh"]);
            E_user.makh = makh;
            if (string.IsNullOrEmpty(E_ten))
            {
                ViewData["Error"] = "Don't empty";
            }
            else
            {
                E_user.ho = E_ho.ToString();

                E_user.Ten = E_ten;
                E_user.tendangnhap = E_tendangnhap.ToString();
                E_user.matkhau = E_matkhau;
                E_user.email = E_email;
                E_user.diachi = E_diachi;
                E_user.dienthoai = E_dienthoai;
                E_user.ngaysinh = E_ngaysinh;
                UpdateModel(E_user);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.EditKhachHang(makh);
        }
        public ActionResult PhanQuyenNhanVien(int MaNV)
        {

            var E_nhanvien = db.NhanViens.FirstOrDefault(m => m.MaNV == MaNV);
            var chucVuList = db.ChucVus.ToList();
            var chucVuSelectList = new SelectList(chucVuList, "MaCV", "TenCV");
            ViewBag.ChucVuList = chucVuSelectList;


            return View(E_nhanvien);
        }
        [HttpPost]
        public ActionResult PhanQuyenNhanVien(int MaNV, FormCollection collection)
        {
            var E_nhanvien = db.NhanViens.First(m => m.MaNV == MaNV);
            var E_ho = collection["Ho"];
            var E_ten = collection["Ten"];
            var E_cv = collection["Chuc Vu"];
            E_nhanvien.MaNV = MaNV;

            if (string.IsNullOrEmpty(E_ten))
            {
                ViewData["Error"] = "Don't empty";
            }
            else
            {
                E_nhanvien.Ten = E_ten;
                E_nhanvien.Ho = E_ho;

                int maCV;
                if (int.TryParse(E_cv, out maCV))
                {
                    E_nhanvien.MaCV = maCV;
                }
                else
                {
                    ViewData["Error"] = "Invalid Chuc Vu";
                }
                UpdateModel(E_nhanvien);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }

            return this.PhanQuyenNhanVien(MaNV);
        }
        public ActionResult CreateNhanVien()

        {
            var chucVuList = db.ChucVus.ToList();
            ViewBag.ChucVuList = new SelectList(chucVuList, "MaCV", "TenCV");

            return View();
        }
        [HttpPost]
        public ActionResult CreateNhanVien(FormCollection collection, NhanVien nv)
        {
            var E_ho = collection["Ho"];
            var E_ten = collection["Ten"];
            var E_tendangnhap = collection["tendangnhap"];
            var E_matkhau = collection["matkhau"];
            var E_email = collection["email"];
            var E_diachi = collection["diachi"];
            var E_dienthoai = collection["dienthoai"];
            var E_ngaysinh = Convert.ToDateTime(collection["ngaysinh"]);
            var E_hinh = collection["hinh"];
            if (string.IsNullOrEmpty(E_ten))
            {
                ViewData["Error"] = "Don't empty";
            }
            else
            {

                nv.Ho = E_ho;
                nv.Ten = E_ten;
                nv.tendangnhap = E_tendangnhap;
                nv.matkhau = E_matkhau;
                nv.email = E_email;
                nv.diachi = E_diachi;
                nv.dienthoai = E_dienthoai;
                nv.ngaysinh = E_ngaysinh;
                nv.Hinh = E_hinh;
                db.NhanViens.InsertOnSubmit(nv);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.CreateNhanVien();
        }
        public ActionResult ThongKeSoLuongTon()
        {
            var viewModel = new ThongKeSoLuongTonViewModel();

            // Lấy danh sách sản phẩm và số lượng tồn
            var productList = db.Items.Select(sp => new ProductInventory
            {
                MaSP = sp.ma,
                TenSP = sp.ten,
                SoLuongTon = (int)sp.soluongton
            }).ToList();

            // Tính tổng số lượng tồn của tất cả sản phẩm
            int totalInventory = productList.Sum(p => p.SoLuongTon);

            // Lấy danh sách sản phẩm có số lượng dưới 15
            var lowInventoryProducts = productList.Where(p => p.SoLuongTon < 15).ToList();

            // Gửi thông báo cho các sản phẩm có số lượng dưới 15
            foreach (var product in lowInventoryProducts)
            {
                ViewBag.Message += $"Sản phẩm {product.TenSP} có số lượng tồn dưới 15. ";
            }

            // Truyền danh sách sản phẩm, tổng số lượng tồn và thông báo vào view
            ViewBag.ProductInventoryList = productList;
            ViewBag.TotalInventory = totalInventory;

            return View(viewModel);
        }
        public ActionResult QuanLyDonHang()
        {
            var query = from d in db.DonHangs
                        join c in db.ChiTietDonHangs on d.madon equals c.madon
                        join k in db.KhachHangs on d.makh equals k.makh
                        group c by new { d.madon, k.ho, k.Ten, d.trangthai } into g
                        orderby g.Key.madon descending
                        select new Store_Category
                        {
                            Madon = g.Key.madon,

                            TrangThai = g.Key.trangthai,
                            TongTien = (int)g.Sum(c => c.tongtien),
                            khachhang = new KhachHangModel
                            {
                                Ho = g.Key.ho,
                                Ten = g.Key.Ten,
                            }
                        };
            return View(query);
        }
        [HttpPost]
        public ActionResult UpdateStatus(string status, int orderId)
        {
            var order = db.DonHangs.FirstOrDefault(o => o.madon == orderId);
            if (order != null)
            {
                order.trangthai = status;
                if (status == "Hoàn tất")
                {
                    order.xacnhan = false;
                }
                db.SubmitChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }
        }
        public ActionResult Del_NhanVien(int MaNV)

        {
            var nhanVienList = db.NhanViens.ToList();
            var selectList = new SelectList(nhanVienList, "MaNV", "Ten");
            ViewBag.NhanVienList = selectList;

            var D_nv = db.NhanViens.First(m => m.MaNV == MaNV);
            return View(D_nv);

        }
        [HttpPost]
        public ActionResult Del_NhanVien(int MaNV, FormCollection collection)
        {
            var D_nv = db.NhanViens.Where(m => m.MaNV == MaNV).First();
                db.NhanViens.DeleteOnSubmit(D_nv);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

    }
}

    


