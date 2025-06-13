using FlightManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FlightManagement.Controllers
{
    public class ChuyenBayController : Controller
    {
        private readonly string _connectionString;
        public ChuyenBayController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IActionResult Search()
        {
            return View();
        }

        
        public IActionResult Details(string maChuyenBay)
        {
            try
            {
                ChuyenBay chuyenBay = null;
                List<CT_CB> hanhKhachs = new List<CT_CB>();
                int soHanhKhachThuong = 0;
                int soHanhKhachVip = 0;
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sqlChuyenBay = "SELECT * FROM CHUYENBAY WHERE MACH=@MACH";
                    using (SqlCommand command = new SqlCommand(sqlChuyenBay, connection))
                    {
                        command.Parameters.AddWithValue("@MACH", maChuyenBay);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                chuyenBay = new ChuyenBay
                                {
                                    MACH = reader["MACH"].ToString(),
                                    CHUYEN = reader["CHUYEN"].ToString(),
                                    DDI = reader["DDI"].ToString(),
                                    DDEN = reader["DDEN"].ToString(),
                                    NGAY = Convert.ToDateTime(reader["NGAY"]),
                                    GBAY = TimeSpan.Parse(reader["GBAY"].ToString()),
                                    GDEN = TimeSpan.Parse(reader["GDEN"].ToString()),
                                    THUONG = Convert.ToInt32(reader["THUONG"]),
                                    VIP = Convert.ToInt32(reader["VIP"]),
                                    MAMB = reader["MAMB"].ToString()
                                };
                            }
                        }
                    }
                    if (chuyenBay == null)
                    {
                        ViewData["Error"] = "Không tìm thấy chuyến bay";
                        return View("Search");
                    }
                    string sqlHanhKhach = @"SELECT CT.MACH, CT.MAHK, CT.SOGHE, CT.LOAIGHE, HK.HOTEN, HK.DIENTHOAI
                    FROM CT_CB CT
                    JOIN HANHKHACH HK ON CT.MAHK=HK.MAHK
                    WHERE CT.MACH = @MACH";
                    using (SqlCommand command = new SqlCommand(sqlHanhKhach, connection))
                    {
                        command.Parameters.AddWithValue("@MACH", maChuyenBay);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool isVip = Convert.ToBoolean(reader["LOAIGHE"]);
                                hanhKhachs.Add(new CT_CB
                                {
                                    MAHK = reader["MAHK"].ToString(),
                                    HOTEN = reader["HOTEN"].ToString(),
                                    DIENTHOAI = reader["DIENTHOAI"].ToString(),
                                    SOGHE = reader["SOGHE"].ToString(),
                                    LOAIGHE = isVip
                                });
                                if (isVip)
                                    soHanhKhachVip++;
                                else
                                    soHanhKhachThuong++;
                            }
                        }
                    }
                }
                var viewModel = new ChuyenBayViewModel
                {
                    ChuyenBay = chuyenBay,
                    HanhKhachs = hanhKhachs,
                    SoHanhKhachThuong = soHanhKhachThuong,
                    SoHanhKhachVip = soHanhKhachVip
                };
                return View("Details", viewModel);
            }
            catch (SqlException ex)
            {
                ViewData["Error"] = $"Lỗi khi truy vấn: {ex.Message}";
                return View("Search");
            }
        }
    }
}