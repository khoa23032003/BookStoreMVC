using NhaSachMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;
namespace NhaSachMVC.Controllers
{
    public class BookStoreController : Controller
    {
        QLBANSACHEntities database = new QLBANSACHEntities();  
        // GET: BookStore

        public ActionResult Index(int? page)
        {
            //tạo biến số sách mỗi trang 
            int paseSize = 5;
            //tạo biến số trang 
            int PageNum = (page ?? 1);
            var dsSachMoi = LaySachMoi(5);
            return View(dsSachMoi);
        }

        private List<SACH> LaySachMoi(int soLuong)
        {
            //sắp xếp theo ngày cập nhật giảm dần
            //chuyển qua dạng danh sách kết quả đạt được 
            return database.SACHes.OrderByDescending(sach => sach.Ngaycapnhat).Take(soLuong).ToList();
        }
        //lấy danh sách trả về dưới dạng Partial View
        public ActionResult LayChuDe()
        {
            var dsChuDe = database.CHUDEs.ToList();
            return PartialView(dsChuDe);
        }
        //lấy danh sách trả về nhà xuất bản 
        public ActionResult LayNhaXuatBan()
        {
            var dsNhaXuatBan = database.NHAXUATBANs.ToList();
            return PartialView( dsNhaXuatBan);
        }
        //lấy sản phẩm theo chủ đề 
        public ActionResult SPTheoChuDe(int id)
        {
            var dsTheoChuDe = database.SACHes.Where(sach=> sach.MaCD == id).ToList();
            //trả về view sau khi sử dụng 
            return View("Index",dsTheoChuDe);

        }
        //lấy sản phẩm theo nhà xuất bản 
        public ActionResult SPtheoNXB(int id)
        {
            var dsTheoNXB = database.SACHes.Where(sach => sach.MaNXB == id);
            return View("Index", dsTheoNXB);
        }

        //lay chi tiết sản phẩm 
        public ActionResult Details(int id)
        {
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            return View(sach);
        }
    }
}