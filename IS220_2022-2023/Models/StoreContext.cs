using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FlightManagement.Models
{
    public class StoreContext
    {
        private readonly string _connectionString;

        public StoreContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        //Câu 1
        // Phương thức thêm hành khách mới (cho HanhKhachController)
        public void ThemThongTinHanhKhach(HanhKhach hanhKhach)
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
        }
        //Câu 2 Khi nhập mã chuyến bay vào trong ô textfield và nhấn nút “Liệt kê” thì sẽ hiển thị trang Web mới liệt kê thông tin chuyến bay và danh sách hành khách
        // Phương thức cho CT_CB Controller - Lấy thông tin chuyến bay và hành khách để hiển thị form thêm
        public ChuyenBayViewModel XemThongTinChuyenBayVaDanhSachHanhKhach(string mach)
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

            return new ChuyenBayViewModel
            {
                ChuyenBay = chuyenBay,
                HanhKhachs = hanhKhachs,
                SoHanhKhachThuong = soHanhKhachThuong,
                SoHanhKhachVip = soHanhKhachVip
            };
        }

        // Phương thức thêm hành khách vào chuyến bay
        public void ThemHanhKhachVaoCTCB(string mach, string mahk, string loaighe, string soghe)
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                // Thêm vào bảng CT_CB
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
        }
        

        // Phương thức lấy thông tin hành khách để chỉnh sửa
        public CT_CB HienThiFormSuaHanhKhach(string mach, string mahk)
        {
            CT_CB ctcb = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT CT.MACH, CT.MAHK, HK.HOTEN, CT.SOGHE, CT.LOAIGHE FROM CT_CB CT JOIN HANHKHACH HK ON CT.MAHK = HK.MAHK WHERE CT.MACH = @MACH AND CT.MAHK = @MAHK";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@MACH", mach);
                    cmd.Parameters.AddWithValue("@MAHK", mahk);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ctcb = new CT_CB
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
            return ctcb;
        }

        // Phương thức cập nhật thông tin hành khách
        public void SuaThongTinHanhKhachTrongCTCB(string mach, string mahk, string hoten, string loaighe, string soghe)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // Cập nhật thông tin trong bảng CT_CB
                string updateCTCB = "UPDATE CT_CB SET SOGHE=@SOGHE, LOAIGHE=@LOAIGHE WHERE MACH=@MACH AND MAHK=@MAHK";
                using (SqlCommand cmd = new SqlCommand(updateCTCB, connection))
                {
                    cmd.Parameters.AddWithValue("@MACH", mach);
                    cmd.Parameters.AddWithValue("@MAHK", mahk);
                    cmd.Parameters.AddWithValue("@SOGHE", soghe);
                    cmd.Parameters.AddWithValue("@LOAIGHE", loaighe == "VIP");
                    cmd.ExecuteNonQuery();
                }
                
                // Cập nhật tên hành khách trong bảng HANHKHACH
                string updateHanhKhach = "UPDATE HANHKHACH SET HOTEN=@HOTEN WHERE MAHK=@MAHK";
                using (SqlCommand cmd = new SqlCommand(updateHanhKhach, connection))
                {
                    cmd.Parameters.AddWithValue("@MAHK", mahk);
                    cmd.Parameters.AddWithValue("@HOTEN", hoten);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Phương thức xóa hành khách khỏi chuyến bay
        public void XoaHanhKhachTrongCTCB(string mach, string mahk)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string deleteCTCB = "DELETE FROM CT_CB WHERE MACH=@MACH AND MAHK=@MAHK";
                using (SqlCommand cmd = new SqlCommand(deleteCTCB, connection))
                {
                    cmd.Parameters.AddWithValue("@MACH", mach);
                    cmd.Parameters.AddWithValue("@MAHK", mahk);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
} 