using FlightManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightManagement.Controllers
{
    public class HanhKhachController : Controller
    {
        private readonly StoreContext _storeContext;

        public HanhKhachController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        //🎯 CÁCH 1: Tên khác nhau cho GET và POST (không dùng ActionName)
        
        //GET: HanhKhach/Cau1_ThemThongTinHanhKhach - Hiển thị form thêm hành khách
        [HttpGet]
        public IActionResult Cau1_ThemThongTinHanhKhach()
        {
            return View();
        }

        //POST: HanhKhach/XuLyThemHanhKhach - Xử lý thêm hành khách
        [HttpPost]
        public IActionResult ThemThongTinHanhKhach(HanhKhach hanhKhach)
        {
            _storeContext.ThemThongTinHanhKhach(hanhKhach);
            return RedirectToAction("Index", "Home");
        }

        //🎯 CÁCH 2: Cùng tên cho GET và POST (backup methods)
        
        //GET: HanhKhach/XemThongTinHanhKhach
        // [HttpGet]
        // public IActionResult XemThongTinHanhKhach()
        // {
        //     return View();
        // }

        // //POST: HanhKhach/XemThongTinHanhKhach (cùng tên với GET)
        // [HttpPost]
        // public IActionResult XemThongTinHanhKhach(HanhKhach hanhKhach)
        // {
        //     _storeContext.ThemThongTinHanhKhach(hanhKhach);
        //     return RedirectToAction("Index", "Home");
        // }
    }
}   
