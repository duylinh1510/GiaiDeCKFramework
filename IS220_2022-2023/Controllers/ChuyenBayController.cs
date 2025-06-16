using FlightManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightManagement.Controllers
{
    public class ChuyenBayController : Controller
    {
        private readonly StoreContext _storeContext;

        public ChuyenBayController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        //GET: ChuyenBay/Cau2_XemThongTinChuyenBay - Hiển thị form tìm kiếm chuyến bay
        public IActionResult Cau2_XemThongTinChuyenBay()
        {
            return View();
        }

        //POST: ChuyenBay/Cau2_XemThongTinChuyenBay - Xử lý tìm kiếm từ form
        [HttpPost]
        public IActionResult Cau2_XemThongTinChuyenBay(string mach)
        {
            var viewModel = _storeContext.XemThongTinChuyenBayVaDanhSachHanhKhach(mach);
            return View("ThongTinChuyenBay", viewModel);
        }

        //GET: ChuyenBay/XemThongTinChuyenBayVaDanhSachHanhKhach - Hiển thị thông tin (cho redirect)
        [HttpGet]
        public IActionResult XemThongTinChuyenBayVaDanhSachHanhKhach(string mach)
        {
            var viewModel = _storeContext.XemThongTinChuyenBayVaDanhSachHanhKhach(mach);
            return View("ThongTinChuyenBay", viewModel);
        }
    }
}