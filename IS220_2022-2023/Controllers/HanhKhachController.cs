using FlightManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace FlightManagement.Controllers
{
    public class HanhKhachController : Controller
    {
        private readonly string _connectionString;
        public HanhKhachController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //GET: HanhKhach/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(HanhKhach hanhKhach)
        {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO HANHKHACH(MAHK,HOTEN,DIACHI,DIENTHOAI) VALUES(@MAHK,@HOTEN,@DIACHI,@DIENTHOAI)";
                        using (SqlCommand cmd = new SqlCommand(sql, connection))
                        {
                            cmd.Parameters.AddWithValue("@MAHK", hanhKhach.MAHK);
                            cmd.Parameters.AddWithValue("@HOTEN", hanhKhach.HOTEN);
                            cmd.Parameters.AddWithValue("@DIACHI", hanhKhach.DIACHI);
                            cmd.Parameters.AddWithValue("@DIENTHOAI", hanhKhach.DIENTHOAI);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return RedirectToAction("Index", "Home");
                    return View(hanhKhach);
        }
    }
}   
