using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HhcApi.Models
{
    public class LeaveRequestDTO
    {
        public string date { get; set; }
        public string time { get; set; }
        public int eid { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string orgname { get; set; }
        public string dep { get; set; }
        public string shift { get; set; }
        public Nullable<double> ratings { get; set; }
    }
}