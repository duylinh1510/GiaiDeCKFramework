using System;
using System.Collections.Generic;

namespace FlightManagement.Models
{
    public class ChuyenBay
    {
        public string MACH { get; set; }
        public string CHUYEN { get; set; }
        public string DDI { get; set; }
        public string DDEN { get; set; }
        public DateTime NGAY { get; set; }
        public TimeSpan GBAY { get; set; }
        public TimeSpan GDEN { get; set; }
        public int THUONG { get; set; }
        public int VIP { get; set; }
        public string MAMB { get; set; }
    }
}