using Microsoft.AspNetCore.Mvc;
using FlightManagement.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace FlightManagement.Controllers
{
    public class CT_CBController : Controller
    {
        private readonly string _connectionString;
        public CT_CBController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Hiển thị form thêm hành khách, truyền model chuyến bay
        [HttpGet]
        public IActionResult Create(string mach)
        {
            ChuyenBay chuyenBay = null;
            List<CT_CB> hanhKhachs = new List<CT_CB>();
            int soHanhKhachThuong = 0;
            int soHanhKhachVip = 0;
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // Lấy thông tin chuyến bay
                string sqlChuyenBay = "SELECT * FROM CHUYENBAY WHERE MACH=@MACH";
                using (SqlCommand cmd = new SqlCommand(sqlChuyenBay, connection))
                {
                    cmd.Parameters.AddWithValue("@MACH", mach);
                    using (SqlDataReader reader = cmd.ExecuteReader())
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
                
                // Lấy danh sách hành khách hiện tại để đếm
                string sqlHanhKhach = @"SELECT CT.MACH, CT.MAHK, CT.SOGHE, CT.LOAIGHE, HK.HOTEN, HK.DIENTHOAI
                FROM CT_CB CT
                JOIN HANHKHACH HK ON CT.MAHK=HK.MAHK
                WHERE CT.MACH = @MACH";
                using (SqlCommand command = new SqlCommand(sqlHanhKhach, connection))
                {
                    command.Parameters.AddWithValue("@MACH", mach);
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
            
            return View(viewModel);
        }

        // POST: Xử lý thêm hành khách vào chuyến bay
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string mach, string mahk, string hoten, string loaighe, string soghe)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // Kiểm tra và thêm hành khách nếu chưa có
                    string checkHK = "SELECT COUNT(*) FROM HANHKHACH WHERE MAHK=@MAHK";
                    using (SqlCommand cmd = new SqlCommand(checkHK, connection))
                    {
                        cmd.Parameters.AddWithValue("@MAHK", mahk);
                        int count = (int)cmd.ExecuteScalar();
                        if (count == 0)
                        {
                            string insertHK = "INSERT INTO HANHKHACH(MAHK, HOTEN) VALUES(@MAHK, @HOTEN)";
                            using (SqlCommand cmd2 = new SqlCommand(insertHK, connection))
                            {
                                cmd2.Parameters.AddWithValue("@MAHK", mahk);
                                cmd2.Parameters.AddWithValue("@HOTEN", hoten);
                                cmd2.ExecuteNonQuery();
                            }
                        }
                    }
                    // Thêm vào CT_CB
                    string insertCTCB = "INSERT INTO CT_CB(MACH, MAHK, SOGHE, LOAIGHE) VALUES(@MACH, @MAHK, @SOGHE, @LOAIGHE)";
                    using (SqlCommand cmd = new SqlCommand(insertCTCB, connection))
                    {
                        cmd.Parameters.AddWithValue("@MACH", mach);
                        cmd.Parameters.AddWithValue("@MAHK", mahk);
                        cmd.Parameters.AddWithValue("@SOGHE", soghe);
                        cmd.Parameters.AddWithValue("@LOAIGHE", loaighe == "VIP");
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Details", "ChuyenBay", new { maChuyenBay = mach });
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return RedirectToAction("Create", new { mach = mach });
            }
        }

        // GET: Hiển thị form sửa hành khách
        [HttpGet]
        public IActionResult Edit(string mach, string mahk) 
        {
            CT_CB ctc = null;
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT CT.MACH, CT.MAHK, HK.HOTEN, CT.SOGHE, CT.LOAIGHE FROM CT_CB CT JOIN HANHKHACH HK ON CT.MAHK = HK.MAHK WHERE CT.MACH = @MACH AND CT.MAHK = @MAHK";
                using(SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@MACH", mach);
                    cmd.Parameters.AddWithValue("@MAHK", mahk);
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            ctc = new CT_CB
                            {
                                MACH = reader["MACH"].ToString(),
                                MAHK = reader["MAHK"].ToString(),
                                HOTEN = reader["HOTEN"].ToString(),
                                SOGHE = reader["SOGHE"].ToString(),
                                LOAIGHE = Convert.ToBoolean(reader["LOAIGHE"])
                            };
                        }
                    }    
                }
            }
            return View(ctc);
        }

        // POST: Xử lý sửa thông tin hành khách
        [HttpPost]
        public IActionResult Edit(string mach, string mahk, string hoten, string loaighe, string soghe)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Cập nhật thông tin trong bảng CT_CB
                    string updateCTCB = "UPDATE CT_CB SET SOGHE=@SOGHE, LOAIGHE=@LOAIGHE WHERE MACH=@MACH AND MAHK=@MAHK";
                    using(SqlCommand cmd = new SqlCommand(updateCTCB, connection))
                    {
                        cmd.Parameters.AddWithValue("@MACH", mach);
                        cmd.Parameters.AddWithValue("@MAHK", mahk);
                        cmd.Parameters.AddWithValue("@SOGHE", soghe);
                        cmd.Parameters.AddWithValue("@LOAIGHE", loaighe == "VIP");
                        cmd.ExecuteNonQuery();
                    }
                    
                    // Cập nhật tên hành khách trong bảng HANHKHACH
                    string updateHanhKhach = "UPDATE HANHKHACH SET HOTEN=@HOTEN WHERE MAHK=@MAHK";
                    using(SqlCommand cmd = new SqlCommand(updateHanhKhach, connection))
                    {
                        cmd.Parameters.AddWithValue("@MAHK", mahk);
                        cmd.Parameters.AddWithValue("@HOTEN", hoten);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Details", "ChuyenBay", new {maChuyenBay = mach});
            }
            catch(SqlException ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return RedirectToAction("Edit", new {mach = mach, mahk = mahk});
            }
        }

        // POST: Xóa hành khách
        [HttpPost]
        public IActionResult Delete(string mach, string mahk)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string deleteCTCB = "DELETE FROM CT_CB WHERE MACH=@MACH AND MAHK=@MAHK";
                    using(SqlCommand cmd = new SqlCommand(deleteCTCB, connection))
                    {
                        cmd.Parameters.AddWithValue("@MACH", mach);
                        cmd.Parameters.AddWithValue("@MAHK", mahk);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Details", "ChuyenBay", new {maChuyenBay = mach});
            }
            catch(SqlException ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return RedirectToAction("Details", "ChuyenBay", new {maChuyenBay = mach});
            }
        }
    }
}