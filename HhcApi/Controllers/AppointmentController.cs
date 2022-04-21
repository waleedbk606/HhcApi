using HhcApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HhcApi.Controllers
{
    public class AppointmentController : ApiController
    {
        HHCEntities db = new HHCEntities();

        static double toRadians( double angleIn10thofaDegree)
        {
            // Angle in 10th
            // of a degree
            return (angleIn10thofaDegree *
                           Math.PI) / 180;
        }
        static double distance(double lat1,double lat2,double lon1,double lon2)
        {

            // The math module contains
            // a function named toRadians
            // which converts from degrees
            // to radians.
            lon1 = toRadians(lon1);
            lon2 = toRadians(lon2);
            lat1 = toRadians(lat1);
            lat2 = toRadians(lat2);

            // Haversine formula
            double dlon = lon2 - lon1;
            double dlat = lat2 - lat1;
            double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dlon / 2), 2);

            double c = 2 * Math.Asin(Math.Sqrt(a));

            // Radius of earth in
            // kilometers. Use 3956
            // for miles
            double r = 6371;

            // calculate the result
            return ((c * r));
        }

        [HttpGet]
        public HttpResponseMessage GetIndAvailableEmp(string orgname, string dep, string shift,string lat,string lng,string date,string time)
        {   
            try
            {
                using (var ctx = new HHCEntities())
                {
                    double Distance = 0.0;
                    string reaseon = "";
                    bool Avaliblity = false;
                   List<Employee> AvaliableEmployee = new List<Employee>();
                    List<Employee> UnAvaliableEmployee = new List<Employee>();
                    List<Employee> EmployeeList = ctx.Employees.OrderBy(x => x.eid).Where(x => (x.OrgName == orgname) && (x.Department == dep) && (x.Shift == shift)).ToList<Employee>();
                    if (EmployeeList.Count == 0)
                    {
                        reaseon = "No Employee in available in this Organization/shift/department";
                        return Request.CreateResponse(HttpStatusCode.OK, reaseon);
                    }
                    else
                    { 
                        for (int i = 0; i < EmployeeList.Count; i++)
                        {
                             Distance = distance(double.Parse(EmployeeList[i].Lat), double.Parse(lat), double.Parse(EmployeeList[i].Long), double.Parse(lng));
                           
                            if (Distance > double.Parse(EmployeeList[i].Radius))
                            {
                                Avaliblity = false;
                                EmployeeList[i].Availablity = Avaliblity;
                                EmployeeList[i].Distance = Distance.ToString();
                                UnAvaliableEmployee.Add(EmployeeList[i]);

                            }
                            else
                            {
                                int eid = EmployeeList[i].eid;
                                List<Schedule> EmployeeSchedule = ctx.Schedules.Where(x => x.eid == eid).ToList<Schedule>();
                                if (EmployeeSchedule.Count == 0)
                                {
                                    Avaliblity = true;
                                    EmployeeList[i].Availablity = Avaliblity;
                                    EmployeeList[i].Distance = Distance.ToString();
                                    AvaliableEmployee.Add(EmployeeList[i]);
                                }
                                else
                                {
                                    for (int j = 0; j < EmployeeSchedule.Count; j++)
                                    {
                                        if (EmployeeSchedule[j].date == date && EmployeeSchedule[j].timeslot == time)
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is busy in given date and time";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else if (EmployeeSchedule[j].date == date && EmployeeSchedule[j].timeslot == "Leave")
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is on full-day leave";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else if (EmployeeSchedule[j].date == date && EmployeeSchedule[j].timeslot == time + "L")
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is on leave at given time slot";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else
                                        {
                                            Avaliblity = true;
                                        }
                                    }
                                    if (Avaliblity == true)
                                    {
                                        EmployeeList[i].Availablity = Avaliblity;
                                        EmployeeList[i].Distance = Distance.ToString();
                                        AvaliableEmployee.Add(EmployeeList[i]);
                                    }
                                    else
                                    {
                                        EmployeeList[i].Availablity = Avaliblity;
                                        EmployeeList[i].Distance = Distance.ToString();
                                        UnAvaliableEmployee.Add(EmployeeList[i]);
                                    }

                                }
                            }
                        }
                        if (AvaliableEmployee.Count > 0)
                        {
                            AvaliableEmployee.OrderByDescending(i => i.Raitings);
                            return Request.CreateResponse(HttpStatusCode.OK, (AvaliableEmployee,UnAvaliableEmployee));
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, (reaseon, UnAvaliableEmployee));
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }


        [HttpGet]
        public HttpResponseMessage GetOrgAvailableEmp(string orgname, string dep, string shift, string lat, string lng, string date, string time)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    double Distance = 0.0;
                    string reaseon = "";
                    bool Avaliblity = false;
                    string Zone = "";
                    List<Employee> AvaliableEmployee = new List<Employee>();
                    List<Employee> UnAvaliableEmployee = new List<Employee>();
                    List<Location> OrgLoations = ctx.Locations.Where(x => x.OrgName == orgname).ToList<Location>();

                    for (int i = 0; i < OrgLoations.Count; i++)
                    {
                        Distance = distance(double.Parse(OrgLoations[i].Lat), double.Parse(lat), double.Parse(OrgLoations[i].Long), double.Parse(lng));
                        if (Distance < double.Parse(OrgLoations[i].Radius))
                        {
                            Zone = OrgLoations[i].Zones;
                            break;
                        }
                    }

                    List<Employee> EmployeeList = ctx.Employees.OrderBy(x => x.eid).Where(x => (x.OrgName == orgname) && (x.Zone == Zone) && (x.Department == dep) && (x.Shift == shift)).ToList<Employee>();
                    if (EmployeeList.Count == 0)
                    {
                        reaseon = "No Employee in available in this Organization/shift/department";
                        return Request.CreateResponse(HttpStatusCode.OK, reaseon);
                    }
                    else
                    {
                        for (int i = 0; i < EmployeeList.Count; i++)
                        {
               
                                int eid = EmployeeList[i].eid;
                                List<Schedule> EmployeeSchedule = ctx.Schedules.Where(x => x.eid == eid).ToList<Schedule>();
                                if (EmployeeSchedule.Count == 0)
                                {
                                    Avaliblity = true;
                                    EmployeeList[i].Availablity = Avaliblity;
                                    EmployeeList[i].Distance = Distance.ToString();
                                    AvaliableEmployee.Add(EmployeeList[i]);
                                }
                                else
                                {
                                    for (int j = 0; j < EmployeeSchedule.Count; j++)
                                    {
                                        if (EmployeeSchedule[j].date == date && EmployeeSchedule[j].timeslot == time)
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is busy in given date and time";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else if (EmployeeSchedule[j].date == date && EmployeeSchedule[j].timeslot == "Leave")
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is on full-day leave";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else if (EmployeeSchedule[j].date == date && EmployeeSchedule[j].timeslot == time + "L")
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is on leave at given time slot";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else
                                        {
                                            Avaliblity = true;
                                        }
                                    }
                                    if (Avaliblity == true)
                                    {
                                        EmployeeList[i].Availablity = Avaliblity;
                                        EmployeeList[i].Distance = Distance.ToString();
                                        AvaliableEmployee.Add(EmployeeList[i]);
                                    }
                                    else
                                    {
                                        EmployeeList[i].Availablity = Avaliblity;
                                        EmployeeList[i].Distance = Distance.ToString();
                                        UnAvaliableEmployee.Add(EmployeeList[i]);
                                    }

                                }
                           
                        }
                        if (AvaliableEmployee.Count > 0)
                        {
                            AvaliableEmployee.OrderByDescending(i => i.Raitings);
                            return Request.CreateResponse(HttpStatusCode.OK, (AvaliableEmployee, UnAvaliableEmployee));
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, (reaseon, UnAvaliableEmployee));
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }
    }
}
