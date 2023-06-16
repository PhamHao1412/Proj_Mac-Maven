using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class UsersController : Controller
    {
        public AppleDataDataContext data = new AppleDataDataContext();

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute to use CSRF token
        public ActionResult Register(KhachHang kh)
        {
            var MatKhauXacNhan = Request.Form["MatKhauXacNhan"];

            if (String.IsNullOrEmpty(MatKhauXacNhan))
            {
                ViewData["NhapMKXN"] = "Phải nhập mật khẩu xác nhận";
            }
            else
            {
                if (!kh.matkhau.Equals(MatKhauXacNhan))
                {
                    ViewData["MatKhauGiongNhau"] = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
                }
                else
                {
                    bool checkUsername = data.KhachHangs.Any(m => m.tendangnhap == kh.tendangnhap);
                    bool checkEmail = data.KhachHangs.Any(m => m.email == kh.email);
                    bool checkPhoneNumber = data.KhachHangs.Any(m => m.dienthoai == kh.dienthoai);

                    if (checkUsername)
                    {
                        ViewData["TrungTenDangNhap"] = "Tên đăng nhập đã tồn tại";
                    }
                    else if (checkEmail)
                    {
                        ViewData["TrungEmail"] = "Email đã tồn tại";
                    }
                    else if (checkPhoneNumber)
                    {
                        ViewData["TrungSDT"] = "Số điện thoại đã tồn tại";
                    }
                    else
                    {
                        kh.ngaysinh = kh.ngaysinh.GetValueOrDefault().Date;

                        // Use kh object directly in the query
                        data.KhachHangs.InsertOnSubmit(kh);
                        data.SubmitChanges();

                        return RedirectToAction("LogIn");
                    }
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            if (Session["IsLoginFormLocked"] != null && (bool)Session["IsLoginFormLocked"])
            {
                DateTime loginCooldownEndTime = (DateTime)Session["LoginCooldownEndTime"];

                if (loginCooldownEndTime > DateTime.Now)
                {
                    TimeSpan remainingTime = loginCooldownEndTime - DateTime.Now;
                    TempData["ThongBaoKhoa"] = $"Form đăng nhập đã bị khóa. Vui lòng thử lại sau {remainingTime.TotalSeconds} giây.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Session.Remove("IsLoginFormLocked");
                    Session.Remove("LoginCooldownEndTime");
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(LoginViewModel model)
        {
            var tendangnhap = model.tendangnhap;
            var matkhau = model.matkhau;

            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.tendangnhap == tendangnhap && n.matkhau == matkhau);
            NhanVien nv = data.NhanViens.SingleOrDefault(n => n.tendangnhap == tendangnhap && n.matkhau == matkhau);

            if (kh != null || nv != null)
            {
                Session.Remove("FailedLoginAttempts");
                Session.Remove("LoginCooldownEndTime");
                Session.Remove("IsLoginFormLocked");

                Session["TaiKhoan"] = kh;
                Session["NhanVien"] = nv;
                Session["ThongBao"] = "Chúc mừng đăng nhập thành công";

                if (nv != null && nv.MaCV == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (nv != null && nv.MaCV == 2)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                int failedLoginAttempts = 0;
                DateTime? loginCooldownEndTime = null;

                if (Session["FailedLoginAttempts"] != null)
                {
                    failedLoginAttempts = (int)Session["FailedLoginAttempts"];
                }

                if (Session["LoginCooldownEndTime"] != null)
                {
                    loginCooldownEndTime = (DateTime)Session["LoginCooldownEndTime"];
                }

                if (failedLoginAttempts >= 5)
                {
                    if (loginCooldownEndTime != null && loginCooldownEndTime > DateTime.Now)
                    {
                        var remainingTime = loginCooldownEndTime.Value - DateTime.Now;
                        TempData["ThongBaoKhoa"] = $"Bạn đã vượt quá số lần đăng nhập không thành công. Vui lòng thử lại sau {remainingTime.TotalSeconds} giây.";
                        return RedirectToAction("LogIn", "Users");
                    }
                    else
                    {
                        failedLoginAttempts = 1;
                        loginCooldownEndTime = DateTime.Now.AddSeconds(15);

                        Session["IsLoginFormLocked"] = true;
                        Session["FailedLoginAttempts"] = failedLoginAttempts;
                        Session["LoginCooldownEndTime"] = loginCooldownEndTime;

                        TempData["ThongBaoKhoa"] = "Bạn đã vượt quá số lần đăng nhập không thành công. Vui lòng thử lại sau 15 giây.";
                        return RedirectToAction("LogIn", "Users");
                    }
                }
                else
                {
                    failedLoginAttempts++;
                    Session["FailedLoginAttempts"] = failedLoginAttempts;

                    TempData["ThongBao"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                    return RedirectToAction("LogIn", "Users");
                }
            }
        }

        public ActionResult LogOff()
        {
            // Check if the user is logged in
            if (Session["TaiKhoan"] != null || Session["NhanVien"] != null)
            {
                // End the session
                Session.Abandon();
            }

            // Redirect the user to the login page
            return RedirectToAction("LogIn", "Users");
        }

        public ActionResult ThongTinKhachHang()
        {
            // Get customer information from the Session variable
            var kh = (KhachHang)Session["TaiKhoan"];
            return View(kh);
        }

        public ActionResult DonHang()
        {
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            ViewBag.DonHang = data.GioHangs.Where(m => m.makh == kh.makh).ToList();
            return View();
        }
    }
}
