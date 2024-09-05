using NhaSachMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using PagedList;
using System.IO;
namespace NhaSachMVC.Controllers

{
    public class AdminController : Controller
    {
        QLBANSACHEntities database = new QLBANSACHEntities();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login");
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(ADMIN admin)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(admin.UserAdmin)
                    )
                {
                    ModelState.AddModelError(string.Empty, "User name không được để trống");
                }
                if (String.IsNullOrEmpty(admin.PassAdmin))
                {
                    ModelState.AddModelError(string.Empty, "Password không được để trống");
                }
                //kiểm tra xem tồn tại hay chưa 
                var adminDB = database.ADMINs.FirstOrDefault(ad => ad.UserAdmin == admin.UserAdmin && ad.PassAdmin == admin.PassAdmin);
                if (adminDB != null)
                {

                    ModelState.AddModelError(String.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");


                }
                else
                {
                    Session["Admin"] = adminDB;

                    ViewBag.ThongBao = "Đăng nhập admin thành công";
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }

        public ActionResult Sach(int? page)
        {
            //tạo biến số mỗi trang 
            int Pagesize = 7;
            int PageNum = (page ?? 1);

            var dsSach = database.SACHes.ToList();
            return View(dsSach.OrderBy(sach => sach.Masach).ToPagedList(PageNum, Pagesize));
        }

        //thêm sách
        [HttpGet]
        public ActionResult ThemSach()
        {
            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB");

            return View();
        }
        [HttpPost]
        public ActionResult ThemSach(SACH sach, HttpPostedFileBase Hinhminhhoa)
        {
            //Lấy tên file của hình được up lên 
            var fileName = Path.GetFileName(Hinhminhhoa.FileName);
            //tạo đường dẫn tới file
            var path = Path.Combine(Server.MapPath("~/Images"), fileName);

            //Kiểm tra hình đã tồn tại trong hệ thống chưa
            if (System.IO.File.Exists(path))
            {
                ViewBag.ThongBao = "Hình đã tồn tại ";
            }
            else
            {
                Hinhminhhoa.SaveAs(path); // lưu vào hệ thống
            }
            //Lưu tên sách vào trường HinhMinhHoa
            sach.Hinhminhhoa = fileName;
            database.SACHes.Add(sach);
            database.SaveChanges();
            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB");

            return View();
        }

        public ActionResult ChiTietSach(int id)
        {
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);

        }
        // Hiển thị form chỉnh sửa sách
        [HttpGet]
        public ActionResult ChinhSuaSach(int id)
        {
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }

        // Xử lý dữ liệu khi người dùng submit form chỉnh sửa sách
        [HttpPost]
        public ActionResult ChinhSuaSach(SACH sach, HttpPostedFileBase HinhMinhHoa)
        {
            if (ModelState.IsValid)
            {
                var sachSua = database.SACHes.FirstOrDefault(s => s.Masach == sach.Masach);
                if (sachSua == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }

                if (HinhMinhHoa != null)
                {
                    // Lấy tên file của hình được up lên
                    var fileName = Path.GetFileName(HinhMinhHoa.FileName);
                    // Tạo đường dẫn tới file
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);

                    // Kiểm tra hình đã tồn tại trong hệ thống chưa
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình đã tồn tại";
                        return View(sach);
                    }
                    else
                    {
                        HinhMinhHoa.SaveAs(path); // Lưu vào hệ thống
                        sachSua.Hinhminhhoa = fileName;
                    }
                }

                // Cập nhật thông tin sách
                sachSua.Tensach = sach.Tensach;
                sachSua.MaCD = sach.MaCD;
                sachSua.MaNXB = sach.MaNXB;
                sachSua.Dongia = sach.Dongia;
                sachSua.Mota = sach.Mota;
                sachSua.Donvitinh = sach.Donvitinh;

                database.SaveChanges();

                return RedirectToAction("Sach");
            }

            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB", sach.MaNXB);

            return View(sach);
        }
        [HttpGet]
        public ActionResult xoaSach(int id)
        {
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            return View(sach);
        }
        [HttpPost]
        public ActionResult xoaSach(int id, HttpPostedFileBase HinhMinhHoa)
        {
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);

            // Nếu không tìm thấy sách, trả về lỗi 404
            if (sach == null)
            {
                return HttpNotFound();
            }

            // Xóa sách khỏi cơ sở dữ liệu
            database.SACHes.Remove(sach);
            database.SaveChanges();

            // Chuyển hướng về trang danh sách sách
            return RedirectToAction("Sach");
        }
    }

    }