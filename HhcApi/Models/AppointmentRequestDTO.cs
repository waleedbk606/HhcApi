using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HhcApi.Models
{
    public class AppointmentRequestDTO
    {
        public string orgname { get; set; }
        public string service { get; set; }
        public string shift { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    }
}