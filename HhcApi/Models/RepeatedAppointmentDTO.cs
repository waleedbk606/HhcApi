using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HhcApi.Models
{
    public class RepeatedAppointmentDTO
    {
        public string orgname { get; set; }
        public string service { get; set; }
        public string shift { get; set; }
        public string uid { get; set; }
        public string username { get; set; }
        public string pfname { get; set; }
        public string plname { get; set; }
        public string gender { get; set; }
        public string phnum { get; set; }
        public string timeduration { get; set; }
        public string status { get; set; }
        public string ratings { get; set; } 
        public string lat { get; set; }
        public string lng { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    }
}