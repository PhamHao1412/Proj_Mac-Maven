using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class StaffInfo
    {
        public int MaNV { get; set; }
        public string ChucVu { get; set; }
        public string TenCV { get; internal set; }
        public string Ten { get; internal set; }
        public string Ho { get; internal set; }
    }
}