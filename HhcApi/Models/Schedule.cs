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
    
    public partial class Schedule
    {
        public int sid { get; set; }
        public int eid { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string orgname { get; set; }
        public string dep { get; set; }
        public string shift { get; set; }
        public Nullable<int> noOfpndApnt { get; set; }
        public Nullable<int> ratings { get; set; }
        public string date { get; set; }
        public string timeslot { get; set; }
    }
}
