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
       AppleDataDataContext data = new AppleDataDataContext();



        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection, KhachHang kh)
        {
            var ho = collection["ho"];
            var ten = collection["ten"];
            var tendangnhap = collection["tendangnhap"];
            var matkhau = collection["matkhau"];
            var MatKhauXacNhan = collection["MatKhauXacNhan"];
            var email = collection["email"];
            var diachi = collection["diachi"];
            var dienthoai = collection["dienthoai"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["ngaysinh"]);

            if (String.IsNullOrEmpty(MatKhauXacNhan))
            {
                ViewData["NhapMKXN"] = "Phải nhâp mật khẩu xác nhận";
            }
            else
            {
                if (!matkhau.Equals(MatKhauXacNhan))
                {
                    ViewData["MatKhauGiongNhau"] = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
                }
                else
                {
                    bool check = data.KhachHangs.Any(m => m.tendangnhap == tendangnhap);
                    if (check)
                    {
                        ViewData["TrungTenDangNhap"] = "Tên đăng nhập đã tồn tại";
                    }
                    else
                    {
                        kh.ho = ho;
                        kh.Ten = ten;
                        kh.tendangnhap = tendangnhap;
                        kh.matkhau = matkhau;
                        kh.email = email;
                        kh.diachi = diachi;
                        kh.dienthoai = dienthoai;
                        kh.ngaysinh = DateTime.Parse(ngaysinh);
                        data.KhachHangs.InsertOnSubmit(kh);
                        data.SubmitChanges();
                        return RedirectToAction("LogIn");
                    }
                }
            }
            return this.Register();

        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(LoginViewModel model)
        {
            var tendangnhap = model.tendangnhap;
            var matkhau = model.matkhau;
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.tendangnhap == tendangnhap && n.matkhau == matkhau);
            NhanVien nv = data.NhanViens.SingleOrDefault(n => n.tendangnhap == tendangnhap && n.matkhau == matkhau);


            if (kh != null || nv != null)
            {
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
                TempData["ThongBao"] = "Tên đăng nhập hoặc mật khẩu không đúng ";
                return RedirectToAction("LogIn", "Users");

            }

        }
        public ActionResult LogOff()
        {
            Session.Abandon();
            return RedirectToAction("LogIn", "Users");
        }
        public ActionResult ThongTinKhachHang()
        {
            // Lấy thông tin khách hàng từ biến Session
            var kh = (KhachHang)Session["TaiKhoan"];
            return View(kh);
        }

        public ActionResult QuanLyUser()
        {
            var all_user = from user in data.KhachHangs select user;
            return View(all_user);
        }
        public ActionResult DonHang()
        {
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            ViewBag.DonHang = data.GioHangs.Where(m => m.makh == kh.makh).ToList();
            return View();
        }
    }
}