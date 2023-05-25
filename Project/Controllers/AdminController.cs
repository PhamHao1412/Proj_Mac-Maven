using Project.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
            ThongKeSoLuongTon();//gọi hàm để hiển thị sl sản phẩm có slton duoi 10
            Show_DonHangMoi();
            Show_DonHangDangXuLy();
            Show_DonHangHoanTat();
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
            ThongKeSoLuongTon();
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
        public ActionResult PhanQuyenNhanVien(int? MaNV)
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
            return RedirectToAction("Del_NhanVien");
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

            // Truyền số loại sản phẩm có số lượng dưới 15 vào view
            ViewBag.LowInventoryProductCount = lowInventoryProducts.Count;

            return View(viewModel);
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
       
       
        public ActionResult Create_DanhMuc()
        {
            var all_category = db.Loais.OrderBy(p=>p.maloai).ToList();
            ViewBag.Category = all_category.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Create_DanhMuc(FormCollection collection, Loai danhMuc)
        {
            var C_maloai = Convert.ToInt32(collection["maloai"]);
            var C_tenloai = collection["tenloai"];

            if ( string.IsNullOrEmpty(C_tenloai))
            {
                ViewData["Error"] = "Please enter both category code and name.";
            }
            else
            {
                danhMuc.maloai = C_maloai;
                danhMuc.tenloai = C_tenloai;

                db.Loais.InsertOnSubmit(danhMuc);
                db.SubmitChanges();

                return RedirectToAction("Create_DanhMuc");
            }

            return View();
        }
        public ActionResult EditDanhMuc(int maloai)
        {

            var E_loai = db.Loais.First(m => m.maloai == maloai);

            return View(E_loai);
        }
        [HttpPost]
        public ActionResult EditDanhMuc(int maloai, FormCollection collection)
        {
            var E_loai = db.Loais.First(m => m.maloai == maloai);
            var E_tenloai = collection["tenloai"];

            E_loai.maloai = maloai;
            if (string.IsNullOrEmpty(E_tenloai))
            {
                ViewData["Error"] = "Don't empty";
            }
            else
            {
                E_loai.tenloai = E_tenloai.ToString();

              
                UpdateModel(E_loai);
                db.SubmitChanges();
                return RedirectToAction("Create_DanhMuc");
            }
            return this.EditKhachHang(maloai);
        }

        public ActionResult DeleteDanhMuc(int maloai)
        {
            Loai loai = db.Loais.FirstOrDefault(g => g.maloai == maloai);
            if (loai != null)
            {
                db.Loais.DeleteOnSubmit(loai);
                db.SubmitChanges();
            }

            return RedirectToAction("Create_DanhMuc", "Admin");
        }
        public ActionResult Edit_SanPham(int ma)
        {
          
            var E_sanpham = db.Items.First(m => m.ma == ma);
            return View(E_sanpham);
        }
        [HttpPost]
        public ActionResult Edit_SanPham(int ma, FormCollection collection)
        {
            var E_sanpham = db.Items.First(m => m.ma == ma);
            var ten = collection["ten"];
            var hinh = collection["hinh"];
            var giaban = Convert.ToDecimal(collection["giaban"]);
            var ngaycapnhat = Convert.ToDateTime(collection["ngaycapnhat"]);
            var soluongton = Convert.ToInt32(collection["soluongton"]);
            var giamgia = Convert.ToDecimal(collection["giamgia"]);

            if (string.IsNullOrEmpty(ten))
            {
                ViewData["Error"] = "Don't empty";
            }
            else
            {
                
                E_sanpham.ten = ten;
                E_sanpham.hinh = hinh;
                E_sanpham.giaban = giaban;
                E_sanpham.ngaycapnhat = ngaycapnhat;
                E_sanpham.soluongton = soluongton;
                if (giamgia != 0)
                {
                    E_sanpham.giamgia = E_sanpham.giaban - (E_sanpham.giaban * giamgia / 100);
                   
                }
                 
            //khong dung updateModel vi se bi loi
                db.SubmitChanges();
                return RedirectToAction("Create_SanPham");
            }
            return this.Edit_SanPham(ma);
        }
        public ActionResult Create_SanPham()
        {
            var all_product = db.Items.OrderBy(p => p.maloai).ToList();
            ViewBag.Product = all_product.ToList();
            var danhMucList = db.Loais.ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "maloai", "tenloai");
            return View();
        }

        [HttpPost]

        public ActionResult Create_SanPham(FormCollection collection, Item items)
        {
            var ma = Convert.ToInt32(collection["ma"]);
            var maloai = Convert.ToInt32(collection["maloai"]);
            var ten = collection["ten"];
            var hinh = collection["hinh"];
            var giaban = Convert.ToDecimal(collection["giaban"]);
            var ngaycapnhat = DateTime.Now;
            var soluongton = Convert.ToInt32(collection["soluongton"]);
            var giamgia = Convert.ToDecimal(collection["giamgia"]);
            if (string.IsNullOrEmpty(ten))
            {
                ViewData["Error"] = "Don't empty";
            }
            else
            {
                items.ma = ma;
                items.maloai = maloai;
                items.ten = ten;
                items.hinh = hinh;
                items.giaban = giaban;
                items.ngaycapnhat = ngaycapnhat;
                items.soluongton = soluongton;
                if (giamgia != 0)
                {
                    items.giamgia = giaban - (giaban * giamgia / 100);
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                db.Items.InsertOnSubmit(items);
                db.SubmitChanges();
                return RedirectToAction("Create_SanPham");
            }
            return this.Create_SanPham();

        }
        public ActionResult Show_DonHangMoi()
        {

            var query = from d in db.DonHangs
                        join c in db.ChiTietDonHangs on d.madon equals c.madon
                        join k in db.KhachHangs on d.makh equals k.makh
                        where d.trangthai == "Đơn hàng mới" // Thêm điều kiện trạng thái
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
            ViewBag.Query = query.ToList();
            ViewBag.InventoryCount = db.DonHangs.Where(d => d.trangthai == "Đơn hàng mới").Count(); // Đếm số lượng đơn hàng có trạng thái "Chờ xác nhận"
            return View("Show_DonHangMoi", query.ToList());
        }

        public ActionResult Show_DonHangDangXuLy()
        {
            var query = from d in db.DonHangs
                        join c in db.ChiTietDonHangs on d.madon equals c.madon
                        join k in db.KhachHangs on d.makh equals k.makh
                        where d.trangthai == "Đang xử lý" // Thêm điều kiện trạng thái
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
            ViewBag.Query = query;
            ViewBag.InventoryCount2 = query.Count(d => d.TrangThai == "Đang xử lý"); // Đếm số lượng đơn hàng có trạng thái

            return View("Show_DonHangDangXuLy", query.ToList());
        }
        public ActionResult Show_DonHangHoanTat()
        {
            var query = from d in db.DonHangs
                        join c in db.ChiTietDonHangs on d.madon equals c.madon
                        join k in db.KhachHangs on d.makh equals k.makh
                        where d.trangthai == "Hoàn tất" // Thêm điều kiện trạng thái
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
            ViewBag.Query = query;
            ViewBag.InventoryCount3 = query.Count(d => d.TrangThai == "Hoàn tất"); // Đếm số lượng đơn hàng có trạng thái 
            return View("Show_DonHangHoanTat", query.ToList());
        }
        public ActionResult ChiTietDonHang(int maDonHang)
        {
            var query = from d in db.DonHangs
                        join c in db.ChiTietDonHangs on d.madon equals c.madon
                        join k in db.KhachHangs on d.makh equals k.makh
                        join i in db.Items on c.ma equals i.ma
                        where d.madon == maDonHang
                        group new { c, i } by new { d.madon, k.ho, k.Ten, d.ngaydat, k.diachi, k.ngaysinh, k.email, k.dienthoai, i.ten } into g
                        select new LichSuMuaHangModel
                        {
                            MaDon = g.Key.madon,
                            NgayDat = (DateTime)g.Key.ngaydat,
                            TongTien = (int)g.Sum(x => x.c.soluong * x.c.gia),
                            TenSP = g.Key.ten,
                            SoLuong = (int)g.Sum(x => x.c.soluong),
                            khachhang = new KhachHangModel
                            {
                                Ho = g.Key.ho,
                                Ten = g.Key.Ten,
                                NgaySinh = (DateTime)g.Key.ngaysinh,
                                Email = g.Key.email,
                                Sdt = g.Key.dienthoai,
                                Diachi = g.Key.diachi
                            }
                        };
           
            //nếu không nhóm theo madon thì view sẽ bị lặp thông tin
            // ta sử dụng kiểu IEnumerable<System.Linq.IGrouping<int, Project.Models.LichSuMuaHangModel>> làm kiểu mô hình (model type) cho view
            // vì dữ liệu trả về từ truy vấn được nhóm theo một trường (MaDon) và có thể có nhiều đơn hàng có cùng MaDon.
            var result = query.ToList().GroupBy(m => m.MaDon); 
            ViewBag.result = result;
            return View(ViewBag.result); // Pass ViewBag.result to the view
        }
        

    }

}








    
