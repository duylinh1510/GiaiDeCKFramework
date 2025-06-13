using System.Collections.Generic;
namespace FlightManagement.Models
{
    public class ChuyenBayViewModel
    {
        public ChuyenBay ChuyenBay { get; set; }
        public List<CT_CB> HanhKhachs { get; set; }
        public int SoHanhKhachThuong { get; set; }
        public int SoHanhKhachVip { get; set; }
    }
}