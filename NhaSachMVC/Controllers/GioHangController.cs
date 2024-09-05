using NhaSachMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaSachMVC.Controllers
{
    public class GioHangController : Controller
    {
        List<MatHangMua> LayGioHang()
        {
            List<MatHangMua> gioHang = Session["GioHang"] as List<MatHangMua>;
            //nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào sesion
            if (gioHang == null)
            {
                gioHang = new List<MatHangMua>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }
        public ActionResult ThemSanPhamVaoGio(int MaSP)
        {
            //lấy giỏ hàng hiện tại 
            List<MatHangMua> giohang = LayGioHang();
            MatHangMua sanPham = giohang.FirstOrDefault(s => s.MaSach == MaSP);
            if (sanPham == null) //san phẩm chưa có trong giỏ 
            {
                sanPham = new MatHangMua(MaSP);
                giohang.Add(sanPham);
            }
            else
            {
                sanPham.SoLuong++;
            }
            return RedirectToAction("Details", "BookStore", new { id = MaSP });
        }

        //tính tổng số lượng mặt hàng mua  

        private int TinhTongSL()
        {
            int tongSL = 0;
            List<MatHangMua> giohang = LayGioHang();
            if (giohang != null)
                tongSL = giohang.Sum(sp => sp.SoLuong);
            return tongSL;
        }
        //tính tổng tiền 
        private double TinhTongTien()
        {
            double TongTien = 0;
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang != null)
                TongTien = gioHang.Sum(sp => sp.ThanhTien());
            return TongTien;
        }

        public ActionResult HienThiGioHang()
        {
            List<MatHangMua> gioHang = LayGioHang();
            //Nếu giỏ hàng trống thì trả về trang ban đầu 
            if (gioHang == null || gioHang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(gioHang); //trả về view hiển thị thông tin giỏ hàng
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return PartialView();
        }
        //Xoá giỏ hàng
        public ActionResult XoaMatHang(int Masp)
        {
            List<MatHangMua> gioHang = LayGioHang();

            //lấy sản phẩm trong giỏ hàng 
            var sanpham = gioHang.FirstOrDefault(s => s.MaSach == Masp);
            if (sanpham == null)
            {
                gioHang.RemoveAll(s => s.MaSach == Masp);
                return RedirectToAction("HienThiGioHang");
            }
            if (gioHang.Count == 0)
                return RedirectToAction("Index", "BookStore");
            return RedirectToAction("HienThiGioHang");
        }
        // cập nhật số lượng 
        public ActionResult CapNhatMatHang(int MaSP, int SoLuong)
        {
            List<MatHangMua> gioHang = LayGioHang();
            //Lấy sản phẩm trong giỏ hàng 
            var sanpham = gioHang.FirstOrDefault(s => s.MaSach == MaSP);
            if (sanpham != null)
            {
                //cập nhật lại số lượng tương ứng
                sanpham.SoLuong = SoLuong;
            }
            return RedirectToAction("HienThiGioHang");
        }

        //dặt hàng 
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null) //chưa đăng nhập
                return RedirectToAction("DangNhap", "NguoiDung");
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang == null || gioHang.Count == 0)
                return RedirectToAction("Index", "BookSore");
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(gioHang);
        }
        QLBANSACHEntities database = new QLBANSACHEntities();
        //xác nhận đặt hàng và lưu vào cơ sở dữ liệu 
        public ActionResult DongYDatHang()
        {
            KHACHHANG khach = Session["TaiKhoan"] as KHACHHANG;
            List<MatHangMua> gioHang = LayGioHang();
            DONDATHANG DonHang = new DONDATHANG();
            DonHang.MaKH = khach.MaKH;
            DonHang.NgayDH = DateTime.Now;
            DonHang.Trigia = (decimal)TinhTongTien();
            DonHang.Dagiao = false;
            DonHang.Tennguoinhan = khach.HoTenKH;
            DonHang.Diachinhan = khach.DiachiKH;
            DonHang.Dienthoainhan = khach.DienthoaiKH;
            DonHang.HTThanhtoan = 
                false;
            DonHang.HTGiaohang = false;
            database.SaveChanges();
            //lần lượt thêm từng chi tiết cho đơn hàng trên 
            foreach(var sanpham in gioHang)
            {
                CTDATHANG chitiet = new CTDATHANG();
                chitiet.SoDH = DonHang.SoDH;
                chitiet.Masach = sanpham.MaSach;
                chitiet.Soluong = sanpham.SoLuong;
                chitiet.Dongia = (decimal)sanpham.DonGia;
                database.CTDATHANGs.Add(chitiet);
            }    
            database.SaveChanges();
            //xoá giỏ hàng 
            Session["GioHang"] = null;
            return RedirectToAction("HoanThanhDonHang");


        }
    }
}