//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HhcApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Patient
    {
        public int pid { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Service { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public int uid { get; set; }
    
        public virtual Signup Signup { get; set; }
    }
}
