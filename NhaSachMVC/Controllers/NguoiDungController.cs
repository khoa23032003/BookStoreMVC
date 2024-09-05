using NhaSachMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaSachMVC.Controllers
{
    public class NguoiDungController : Controller
    {
        QLBANSACHEntities database = new QLBANSACHEntities();
        // GET: NguoiDung
        public ActionResult Index()
        {
            return View();
        }
        //lấy thông tin về 
        [HttpGet]
        public ActionResult DangKy() { 
        return View();
        }
        //kiểm nha rồi Post thông tin 
        [HttpPost]
        public ActionResult DangKy(KHACHHANG kh)
        {
            if (string.IsNullOrEmpty(kh.HoTenKH))
                ModelState.AddModelError(string.Empty, "Họ tên không được để trống");
            if (string.IsNullOrEmpty(kh.TenDN))
                ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
            if (string.IsNullOrEmpty(kh.Matkhau))
                ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
            if (string.IsNullOrEmpty(kh.Email))
                ModelState.AddModelError(string.Empty, "Email không được để trống");
            if (string.IsNullOrEmpty(kh.DienthoaiKH))
                ModelState.AddModelError(string.Empty, "Điện thoại không được để trống");

            // Kiểm tra số lượng chữ số của số điện thoại (Ví dụ: 10 chữ số)
            if (kh.DienthoaiKH.Length != 10)
                ModelState.AddModelError(string.Empty, "Số điện thoại phải có đúng 10 chữ số");
            var khachhang = database.KHACHHANGs.FirstOrDefault(k => k.TenDN == kh.TenDN);
            if (khachhang == null)
                ModelState.AddModelError(string.Empty, "Đã có người đăng ký tên này");
            if (ModelState.IsValid) {
                database.KHACHHANGs.Add(kh);
                database.SaveChanges();
            }
            else {
                return View();    
            }
            return RedirectToAction("DangNhap");
        }


        //Đăng nhập 
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(KHACHHANG kh)
        {
            if (ModelState.IsValid) //kiểm tra dữ liệu đầu vào có hợp lệ hay không
            {
                if (string.IsNullOrEmpty(kh.TenDN))
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(kh.Matkhau))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if(ModelState.IsValid)
                {
                    var khach = database.KHACHHANGs.FirstOrDefault(k => k.TenDN == kh.TenDN && k.Matkhau == kh.Matkhau);
                    if (khach != null)
                    {
                        ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                        Session["TaiKhoan"] = khach;
                    }
                    else
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";            
                     }
            }
            return View();
        }
    }
}