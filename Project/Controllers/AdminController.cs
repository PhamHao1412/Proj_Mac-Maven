using Microsoft.Ajax.Utilities;
using Project.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
            //check loai tai khoan dang nhap
            if (Session["NhanVien"] != null)
            {
                


                //truy van hien thi cac tai khoan nhan vien
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
            else
            {
                return RedirectToAction("Index", "Home");

            }
           
        }
        
        // Hàm tính ngày đầu tiên trong tuần theo số tuần và năm
        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)DayOfWeek.Monday - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);
            return firstMonday.AddDays(7 * (weekOfYear - 1));
        }


        public ActionResult ThongKeDoanhThu(int? year, int? weekNum, string startDate, string endDate)
        {
            try
            {
                // Kiểm tra xem phiên làm việc của người dùng có tồn tại không
                if (Session["NhanVien"] != null)
                {
                    // Lấy năm và tuần hiện tại
                    int currentYear = DateTime.Now.Year;
                    int currentWeekNum = GetIso8601WeekOfYear(DateTime.Now);

                    // Lấy năm và tuần được chọn, nếu không có thì sử dụng năm và tuần hiện tại
                    int selectedYear = year ?? currentYear;
                    int selectedWeekNum = weekNum ?? currentWeekNum;

                    // Tính toán ngày đầu tuần dựa trên năm và tuần được chọn
                    DateTime selectedDate = FirstDateOfWeekISO8601(selectedYear, selectedWeekNum);
                    DateTime startDateOfWeek = selectedDate.AddDays(-(int)selectedDate.DayOfWeek + (int)DayOfWeek.Monday);
                    DateTime endDateOfWeek = startDateOfWeek.AddDays(6);

                    // Chuyển đổi chuỗi ngày bắt đầu và ngày kết thúc thành kiểu DateTime, nếu không có thì sử dụng ngày đầu và cuối tuần
                    DateTime start = string.IsNullOrEmpty(startDate) ? startDateOfWeek.Date : DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime end = string.IsNullOrEmpty(endDate) ? endDateOfWeek.Date : DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    // Tạo ViewModel để lưu trữ thông tin về năm, tuần, ngày bắt đầu và ngày kết thúc
                    var viewModel = new ThongKeDoanhThuViewModel
                    {
                        Year = selectedYear,
                        WeekNum = selectedWeekNum,
                        StartDate = start,
                        EndDate = end,
                        StartDateOfWeek = startDateOfWeek,
                        EndDateOfWeek = endDateOfWeek
                    };


                    //var revenueList = db.DonHangs
                    //.Where(dh => dh.ngaygiao != null && dh.trangthai == "Hoàn tất" && dh.ngaygiao >= start && dh.ngaygiao <= end)
                    //.Join(db.ChiTietDonHangs, dh => dh.madon, ctdh => ctdh.madon, (dh, ctdh) => new { dh.ngaygiao, ctdh.tongtien })
                    //.GroupBy(x => x.ngaygiao)
                    //.Select(g => new RevenueStatistics
                    //{
                    //    NgayGiao = (DateTime)g.Key,
                    //    TongTien = (decimal)g.Sum(ctdh => ctdh.tongtien),
                    //    BestSellingProducts = db.Items
                    //    // ...
                    //})
                    //.ToList();

                    // Trong truy vấn này, bạn đã tạo một danh sách revenueList chứa các đối tượng RevenueStatistics.
                    //Mỗi đối tượng RevenueStatistics đại diện cho một ngày bán hàng và bao gồm thông tin về tổng tiền và danh sách các sản phẩm bán chạy.

                    //Tuy nhiên, khi bạn thêm các thông tin sản phẩm bán chạy vào đối tượng RevenueStatistics,
                    //bạn đã sử dụng cùng một danh sách bestSellingProductNames và bestSellingQuantities cho tất cả các ngày trong vòng lặp.
                    //    Điều này dẫn đến việc ghi đè dữ liệu và hiển thị sai kết quả.

                    //Để khắc phục vấn đề này, chúng ta cần tạo danh sách bestSellingProductNames và bestSellingQuantities riêng biệt cho mỗi ngày trong vòng lặp.
                    //    Điều này đảm bảo rằng mỗi ngày sẽ có danh sách sản phẩm bán chạy riêng của nó.


                    //Vì vậy, trong phần controller, thay vì sử dụng danh sách bestSellingProductNames và bestSellingQuantities, 
                    // chúng ta đã thay thế chúng bằng cách gán trực tiếp các danh sách tương ứng cho từng đối tượng RevenueStatistics như sau:

                    //               var revenueList = db.DonHangs
                    // ...
                    //.Select(g => new RevenueStatistics
                    //{
                    //    NgayGiao = (DateTime)g.Key,
                    //    TongTien = (decimal)g.Sum(ctdh => ctdh.tongtien),
                    //    BestSellingProductNames = productList,
                    //    BestSellingQuantities = quantityList
                    //})
                    //.ToList();

                    //Bằng cách này, mỗi đối tượng RevenueStatistics trong danh sách revenueList sẽ chứa danh sách sản phẩm bán chạy riêng biệt cho từng ngày.

                    //                    Sau khi đã cập nhật truy vấn dữ liệu, chúng ta cần cập nhật cả phần view để hiển thị đúng kết quả.
                    //                        Trong phần view, chúng ta đã sử dụng các danh sách lồng nhau ViewBag.BestSellingProductNames và ViewBag.BestSellingQuantities để truy cập thông tin sản phẩm bán chạy của từng ngày.

                    //Bên trong vòng lặp for, chúng ta đã sử dụng vòng lặp foreach để duyệt qua các phần tử tương ứng trong danh sách lồng nhau và hiển thị tên sản phẩm và số lượng bán ra.
                    //                        Điều này đảm bảo rằng chúng ta sẽ hiển thị đúng thông tin sản phẩm bán chạy cho từng ngày.




                    // Truy vấn các đơn hàng hoàn tất trong khoảng thời gian được chọn và tính tổng tiền
                    var revenueList = db.DonHangs
                        // Lọc các đơn hàng đã giao hàng và hoàn tất trong khoảng thời gian từ start đến end
                        .Where(dh => dh.ngaygiao != null && dh.trangthai == "Hoàn tất" && dh.ngaygiao >= start && dh.ngaygiao <= end)
                        // Kết hợp bảng DonHangs và ChiTietDonHangs dựa trên khóa ngoại madon
                        .Join(db.ChiTietDonHangs, dh => dh.madon, ctdh => ctdh.madon, (dh, ctdh) => new { dh.ngaygiao, ctdh.tongtien })
                        // Nhóm các đơn hàng theo ngày giao
                        .GroupBy(x => x.ngaygiao)
                        // Tạo danh sách RevenueStatistics từ các đơn hàng đã nhóm
                        .Select(g => new RevenueStatistics
                        {
                            NgayGiao = (DateTime)g.Key, // Ngày giao
                            TongTien = (decimal)g.Sum(ctdh => ctdh.tongtien), // Tổng tiền của các đơn hàng
                            BestSellingProducts = db.Items
                                // Kết hợp bảng Items và ChiTietDonHangs dựa trên khóa ngoại ma
                                .Join(db.ChiTietDonHangs, i => i.ma, c => c.ma, (i, c) => new { Item = i, ChiTietDonHang = c })
                                // Kết hợp bảng ChiTietDonHangs và DonHangs dựa trên khóa ngoại madon
                                .Join(db.DonHangs, c => c.ChiTietDonHang.madon, d => d.madon, (c, d) => new { c.Item, c.ChiTietDonHang, DonHang = d })
                                // Lọc các sản phẩm trong đơn hàng đã hoàn tất và có ngày giao trùng khớp với khóa nhóm g.Key
                                .Where(x => x.DonHang.trangthai == "Hoàn tất" && x.DonHang.ngaygiao != null && x.DonHang.ngaygiao == g.Key)
                                // Nhóm các sản phẩm theo mã và tên
                                .GroupBy(x => new { x.Item.ma, x.Item.ten })
                                // Sắp xếp giảm dần theo tổng số lượng sản phẩm đã bán
                                .OrderByDescending(grp => grp.Sum(x => x.ChiTietDonHang.soluong))
                                // Chọn 5 sản phẩm bán chạy nhất
                                .Take(5)
                                // Tạo danh sách BestSellingProduct từ các sản phẩm đã nhóm
                                .Select(grp => new BestSellingProduct
                                {
                                    MaSP = grp.Key.ma.ToString(), // Mã sản phẩm
                                    TenSP = grp.Key.ten, // Tên sản phẩm
                                    TongSoLuong = (int)grp.Sum(x => x.ChiTietDonHang.soluong), // Tổng số lượng đã bán
                                    TongTienSP = (decimal)grp.Sum(x => x.ChiTietDonHang.soluong * x.ChiTietDonHang.gia) // Tổng tiền của sản phẩm
                                })
                                .ToList() // Chuyển đổi thành danh sách
                        })
                        .ToList(); // Chuyển đổi thành danh sách


                    // Khai báo danh sách tên sản phẩm bán chạy và số lượng bán chạy
                    var bestSellingProductNames = new List<List<string>>();
                    var bestSellingQuantities = new List<List<int>>();

                    // Lặp qua danh sách doanh thu để lấy thông tin sản phẩm bán chạy cho mỗi ngày
                    foreach (var revenue in revenueList)
                    {
                        var productList = new List<string>();
                        var quantityList = new List<int>();

                        // Lặp qua danh sách sản phẩm bán chạy và thêm thông tin vào danh sách tương ứng
                        foreach (var product in revenue.BestSellingProducts)
                        {
                            var productInfo = db.Items.FirstOrDefault(item => item.ma == int.Parse(product.MaSP));

                            if (productInfo != null)
                            {
                                productList.Add(productInfo.ten);
                                quantityList.Add(product.TongSoLuong);
                            }
                        }

                        // Thêm danh sách tên sản phẩm và số lượng vào danh sách lớn
                        bestSellingProductNames.Add(productList);
                        bestSellingQuantities.Add(quantityList);
                    }


                    // Tính tổng doanh thu trong tuần
                    decimal tongDoanhThuTuan = revenueList.Sum(r => r.TongTien);

                    // Gán các thông tin cần thiết vào ViewBag để truyền cho View
                    ViewBag.ViewModel = viewModel;
                    ViewBag.RevenueStatistics = revenueList;
                    ViewBag.TongDoanhThuTuan = tongDoanhThuTuan;
                    ViewBag.SelectedYear = selectedYear;
                    ViewBag.SelectedWeekNum = selectedWeekNum;
                    ViewBag.BestSellingProductNames = bestSellingProductNames;
                    ViewBag.BestSellingQuantities = bestSellingQuantities;

                    // Gọi phương thức ThongKeSoLuongTon để cập nhật số lượng tồn kho
                    ThongKeSoLuongTon();

                    // Trả về View và truyền vào ViewModel
                    return View(viewModel);
                }
                else
                {
                    // Nếu phiên làm việc không tồn tại, chuyển hướng đến trang chủ
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                // Bạn có thể sử dụng một framework ghi log như Serilog hoặc ghi log vào file hoặc cơ sở dữ liệu.
                // Ở đây, ta chỉ đơn giản ghi ra console.
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }
        }



        // Hàm tính ngày đầu tiên trong tuần theo số tuần và năm
        private int GetIso8601WeekOfYear(DateTime now)
        {
            var culture = CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;

            // Lấy số tuần cho ngày hiện tại dựa trên lịch hiện tại của văn hóa
            int weekNum = calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            // Điều chỉnh số tuần cho ISO 8601, mà xác định tuần đầu tiên của năm là tuần chứa Thứ Năm đầu tiên của năm
            var jan1 = new DateTime(now.Year, 1, 1);//Dòng đầu tiên tạo một đối tượng DateTime với ngày 1 tháng 1 của năm hiện tại. Đây là ngày đầu tiên của năm.

            int jan1DayOfWeek = (int)jan1.DayOfWeek;//Dòng thứ hai lấy giá trị ngày trong tuần của ngày 1 tháng 1 (jan1.DayOfWeek) và ép kiểu sang kiểu nguyên ((int)jan1.DayOfWeek).
                                                    //Giá trị này biểu thị ngày trong tuần dưới dạng số, trong đó 0 tương đương với Chủ nhật và 6 tương đương với Thứ Bảy.

            int daysToAdd = (int)DayOfWeek.Thursday - jan1DayOfWeek;//Dòng thứ ba tính toán số ngày cần thêm vào để đạt được Thứ Năm đầu tiên của năm.
                                                                    //Nó lấy giá trị của ngày trong tuần của Thứ Năm ((int)DayOfWeek.Thursday) và trừ đi ngày trong tuần của ngày 1 tháng 1 (jan1DayOfWeek). Kết quả là số ngày cần thêm vào.


            if (jan1DayOfWeek > 4)//Dòng thứ tư kiểm tra xem ngày trong tuần của ngày 1 tháng 1 có lớn hơn 4 hay không (đại diện cho Thứ Tư).
                                  //Nếu có, tức là ngày 1 tháng 1 không nằm trong tuần đầu tiên của năm.
                                  //Trong trường hợp này, chúng ta cần thêm 7 ngày để đảm bảo ngày Thứ Năm đầu tiên của năm nằm trong tuần đầu tiên.


                daysToAdd += 7;

            var firstThursday = jan1.AddDays(daysToAdd);
            if (now < firstThursday)
            {
                // Ngày hiện tại nằm trước tuần đầu tiên của năm
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
            try
            {
                if (Session["NhanVien"] != null)
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
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                // Bạn có thể sử dụng một framework ghi log như Serilog hoặc ghi log vào file hoặc cơ sở dữ liệu.
                // Ở đây, ta chỉ đơn giản ghi ra console.
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }
        }

        public ActionResult DeleteKhachHang(int makh)
        {
            if (Session["NhanVien"] != null)
            {
                try
                {
                    KhachHang khachhang = db.KhachHangs.FirstOrDefault(g => g.makh == makh);
                    if (khachhang != null)
                    {
                        db.KhachHangs.DeleteOnSubmit(khachhang);
                        db.SubmitChanges();
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi để kiểm tra
                    // Bạn có thể sử dụng một framework ghi log như Serilog hoặc ghi log vào file hoặc cơ sở dữ liệu.
                    // Ở đây, ta chỉ đơn giản ghi ra console.
                    Console.WriteLine(ex.ToString());

                    // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult PhanQuyenNhanVien(int? MaNV)
        {
            if (Session["NhanVien"] != null)
            {
                var E_nhanvien = db.NhanViens.FirstOrDefault(m => m.MaNV == MaNV);
                var chucVuList = db.ChucVus.ToList();
                var chucVuSelectList = new SelectList(chucVuList, "MaCV", "TenCV");
                ViewBag.ChucVuList = chucVuSelectList;

                return View(E_nhanvien);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult PhanQuyenNhanVien(int MaNV, FormCollection collection)
        {
            if (Session["NhanVien"] != null)
            {
                try
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
                catch (Exception ex)
                {
                    // Ghi log lỗi để kiểm tra
                    // Bạn có thể sử dụng một framework ghi log như Serilog hoặc ghi log vào file hoặc cơ sở dữ liệu.
                    // Ở đây, ta chỉ đơn giản ghi ra console.
                    Console.WriteLine(ex.ToString());

                    // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CreateNhanVien()
        {
            if (Session["NhanVien"] != null)
            {
                var chucVuList = db.ChucVus.ToList();
                ViewBag.ChucVuList = new SelectList(chucVuList, "MaCV", "TenCV");

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult CreateNhanVien(FormCollection collection, NhanVien nv)
        {
            if (Session["NhanVien"] != null)
            {
                try
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

                    var chucVuList = db.ChucVus.ToList();
                    ViewBag.ChucVuList = new SelectList(chucVuList, "MaCV", "TenCV");

                    return View();
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi để kiểm tra
                    // Bạn có thể sử dụng một framework ghi log như Serilog hoặc ghi log vào file hoặc cơ sở dữ liệu.
                    // Ở đây, ta chỉ đơn giản ghi ra console.
                    Console.WriteLine(ex.ToString());

                    // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Del_NhanVien(int MaNV)
        {
            if (Session["NhanVien"] != null)
            {
                var nhanVienList = db.NhanViens.ToList();
                var selectList = new SelectList(nhanVienList, "MaNV", "Ten");
                ViewBag.NhanVienList = selectList;

                var D_nv = db.NhanViens.First(m => m.MaNV == MaNV);
                return View(D_nv);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Del_NhanVien(int MaNV, FormCollection collection)
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    var D_nv = db.NhanViens.FirstOrDefault(m => m.MaNV == MaNV);
                    if (D_nv != null)
                    {
                        db.NhanViens.DeleteOnSubmit(D_nv);
                        db.SubmitChanges();
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                // Bạn có thể sử dụng một framework ghi log như Serilog hoặc ghi log vào file hoặc cơ sở dữ liệu.
                // Ở đây, ta chỉ đơn giản ghi ra console.
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }
        }

        public ActionResult ThongKeSoLuongTon()
        {
            try
            {
                if (Session["NhanVien"] != null)
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
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                // Bạn có thể sử dụng một framework ghi log như Serilog hoặc ghi log vào file hoặc cơ sở dữ liệu.
                // Ở đây, ta chỉ đơn giản ghi ra console.
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                ViewData["Error"] = "Đã xảy ra lỗi khi thực hiện thống kê số lượng tồn.";
                return View();
            }
        }


        [HttpPost]
        public ActionResult UpdateStatus(string status, int orderId)
        {
            try
            {
                if (Session["NhanVien"] != null)
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
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một thông báo lỗi hoặc chuyển hướng đến trang lỗi tùy thuộc vào yêu cầu của ứng dụng
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật trạng thái đơn hàng." });
            }
        }



        public ActionResult Create_DanhMuc()
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    var all_category = db.Loais.OrderBy(p => p.maloai).ToList();
                    ViewBag.Category = all_category.ToList();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Create_DanhMuc(FormCollection collection, Loai danhMuc)
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    var C_maloai = Convert.ToInt32(collection["maloai"]);
                    var C_tenloai = collection["tenloai"];

                    if (string.IsNullOrEmpty(C_tenloai))
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
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }

            return View();
        }

        public ActionResult EditDanhMuc(int maloai)
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    var E_loai = db.Loais.First(m => m.maloai == maloai);
                    return View(E_loai);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult EditDanhMuc(int maloai, FormCollection collection)
        {
            try
            {
                if (Session["NhanVien"] != null)
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
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }

            return this.EditKhachHang(maloai);
        }

        public ActionResult DeleteDanhMuc(int maloai)
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    Loai loai = db.Loais.FirstOrDefault(g => g.maloai == maloai);
                    if (loai != null)
                    {
                        db.Loais.DeleteOnSubmit(loai);
                        db.SubmitChanges();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }

            return RedirectToAction("Create_DanhMuc", "Admin");
        }


        public ActionResult Edit_SanPham(int ma)
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    var E_sanpham = db.Items.First(m => m.ma == ma);
                    return View(E_sanpham);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Edit_SanPham(int ma, FormCollection collection)
        {
            try
            {
                if (Session["NhanVien"] != null)
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

                        db.SubmitChanges();
                        return RedirectToAction("Create_SanPham");
                    }

                    return RedirectToAction("Edit_SanPham", new { ma = ma });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }
        }

        public ActionResult Create_SanPham()
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    var all_product = db.Items.OrderBy(p => p.maloai).ToList();
                    ViewBag.Product = all_product.ToList();
                    var danhMucList = db.Loais.ToList();
                    ViewBag.DanhMucList = new SelectList(danhMucList, "maloai", "tenloai");
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Create_SanPham(FormCollection collection, Item items)
        {
            try
            {
                if (Session["NhanVien"] != null)
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
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }

            return this.Create_SanPham();
        }

        public ActionResult DeleteSanPham(int ma)
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    Item item = db.Items.FirstOrDefault(g => g.ma == ma);
                    if (item != null)
                    {
                        db.Items.DeleteOnSubmit(item);
                        db.SubmitChanges();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Create_SanPham");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để kiểm tra
                Console.WriteLine(ex.ToString());

                // Trả về một trang lỗi hoặc thông báo lỗi tùy thuộc vào yêu cầu của ứng dụng
                return View("Error");
            }
        }

        public ActionResult Show_DonHangMoi()
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    var query = from d in db.DonHangs
                                join c in db.ChiTietDonHangs on d.madon equals c.madon
                                join k in db.KhachHangs on d.makh equals k.makh
                                where d.trangthai == "Đơn hàng mới"
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
                    ViewBag.InventoryCount = db.DonHangs.Count(d => d.trangthai == "Đơn hàng mới");
                    return View("Show_DonHangMoi", query.ToList());
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View("Error");
            }
        }

        public ActionResult Show_DonHangDangXuLy()
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    var query = from d in db.DonHangs
                                join c in db.ChiTietDonHangs on d.madon equals c.madon
                                join k in db.KhachHangs on d.makh equals k.makh
                                where d.trangthai == "Đang xử lý"
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
                    ViewBag.InventoryCount2 = query.Count(d => d.TrangThai == "Đang xử lý");
                    return View("Show_DonHangDangXuLy", query.ToList());
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View("Error");
            }
        }

        public ActionResult Show_DonHangHoanTat()
        {
            try
            {
                if (Session["NhanVien"] != null)
                {
                    var query = from d in db.DonHangs
                                join c in db.ChiTietDonHangs on d.madon equals c.madon
                                join k in db.KhachHangs on d.makh equals k.makh
                                where d.trangthai == "Hoàn tất"
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
                    ViewBag.InventoryCount3 = query.Count(d => d.TrangThai == "Hoàn tất");
                    return View("Show_DonHangHoanTat", query.ToList());
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View("Error");
            }
        }

        //public ActionResult ChiTietDonHang(int maDonHang)
        //{
        //    var query = from d in db.DonHangs
        //                join c in db.ChiTietDonHangs on d.madon equals c.madon
        //                join k in db.KhachHangs on d.makh equals k.makh
        //                join i in db.Items on c.ma equals i.ma
        //                where d.madon == maDonHang
        //                group new { c, i } by new { d.madon, k.ho, k.Ten, d.ngaydat, k.diachi, k.ngaysinh, k.email, k.dienthoai, i.ten } into g
        //                select new LichSuMuaHangModel
        //                {
        //                    MaDon = g.Key.madon,
        //                    NgayDat = (DateTime)g.Key.ngaydat,
        //                    TongTien = (int)g.Sum(x => x.c.soluong * x.c.gia),
        //                    TenSP = g.Key.ten,
        //                    SoLuong = (int)g.Sum(x => x.c.soluong),
        //                    khachhang = new KhachHangModel
        //                    {
        //                        Ho = g.Key.ho,
        //                        Ten = g.Key.Ten,
        //                        NgaySinh = (DateTime)g.Key.ngaysinh,
        //                        Email = g.Key.email,
        //                        Sdt = g.Key.dienthoai,
        //                        Diachi = g.Key.diachi
        //                    }
        //                };
           
        //    //nếu không nhóm theo madon thì view sẽ bị lặp thông tin
        //    // ta sử dụng kiểu IEnumerable<System.Linq.IGrouping<int, Project.Models.LichSuMuaHangModel>> làm kiểu mô hình (model type) cho view
        //    // vì dữ liệu trả về từ truy vấn được nhóm theo một trường (MaDon) và có thể có nhiều đơn hàng có cùng MaDon.
        //    var result = query.ToList().GroupBy(m => m.MaDon); 
        //    ViewBag.result = result;
        //    return View(ViewBag.result); // Pass ViewBag.result to the view
        //}
        
        public ActionResult ChiTietDonHang(int maDonHang)
{
    try
    {
        if (Session["NhanVien"] != null)
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

            var result = query.GroupBy(m => m.MaDon).ToList();
            ViewBag.result = result;
            return View("ChiTietDonHang", result);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        return View("Error");
    }
}

    }

}








    
