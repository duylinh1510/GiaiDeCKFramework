using System.Collections.Generic;
namespace FlightManagement.Models
{
    public class ChuyenBayViewModel
    {
        //Lấy thông tin chuyến bay
        public ChuyenBay ChuyenBay { get; set; }
        //Lấy danh sách hành khách trong chuyến bay
        public List<CT_CB> HanhKhachs { get; set; } = new List<CT_CB>();
        public int SoHanhKhachThuong { get; set; }
        public int SoHanhKhachVip { get; set; }
    }
}