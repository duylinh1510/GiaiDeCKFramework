using Microsoft.AspNetCore.Mvc;
using FlightManagement.Models;

namespace FlightManagement.Controllers
{
    public class CT_CBController : Controller
    {
        private readonly StoreContext _storeContext;

        public CT_CBController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        // GET: CT_CB/HienThiFormThemHanhKhach - Hiển thị form thêm hành khách vào chuyến bay
        [HttpGet]
        public IActionResult HienThiFormThemHanhKhach(string mach)
        {
            var viewModel = _storeContext.XemThongTinChuyenBayVaDanhSachHanhKhach(mach);
            return View("ThemHanhKhachVaoCTCB", viewModel); // Chỉ định rõ tên View
        }

        // POST: CT_CB/ThemHanhKhachVaoCTCB - Xử lý thêm hành khách vào chuyến bay
        [HttpPost]
        public IActionResult ThemHanhKhachVaoCTCB(string mach, string mahk , string loaighe, string soghe)
        {
            _storeContext.ThemHanhKhachVaoCTCB(mach, mahk, loaighe, soghe);
            return RedirectToAction("XemThongTinChuyenBayVaDanhSachHanhKhach", "ChuyenBay", new {mach = mach}); 
        }

        // GET: CT_CB/SuaThongTinHanhKhachTrongCTCB - Hiển thị form sửa hành khách
        [HttpGet]
        public IActionResult HienThiFormSuaHanhKhach(string mach, string mahk) 
        {
            var ctcb = _storeContext.HienThiFormSuaHanhKhach(mach, mahk);
            return View("SuaThongTinHanhKhachTrongCTCB", ctcb);
        }

        // POST: CT_CB/SuaThongTinHanhKhachTrongCTCB - Xử lý sửa thông tin hành khách
        [HttpPost]
        public IActionResult SuaThongTinHanhKhachTrongCTCB(string mach, string mahk, string hoten, string loaighe, string soghe)
        {
            _storeContext.SuaThongTinHanhKhachTrongCTCB(mach, mahk, hoten, loaighe, soghe);
            return RedirectToAction("XemThongTinChuyenBayVaDanhSachHanhKhach", "ChuyenBay", new {mach = mach});
        }

        // POST: CT_CB/XoaHanhKhachTrongCTCB - Xóa hành khách
        [HttpPost]
        public IActionResult XoaHanhKhachTrongCTCB(string mach, string mahk)
        {
            _storeContext.XoaHanhKhachTrongCTCB(mach, mahk);
            return RedirectToAction("XemThongTinChuyenBayVaDanhSachHanhKhach", "ChuyenBay", new {mach = mach}); 
        }
    }
}