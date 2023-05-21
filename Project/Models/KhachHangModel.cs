﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class KhachHangModel
    {
        public int Id { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public string Email { get; set; }
        public string Diachi { get; set; }
        public DateTime NgaySinh { get; set; }
        public string Sdt { get; set; }
    }
}