using HhcApi.Models;
using Newtonsoft.Json;
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

        public HttpResponseMessage GetCalculatedOrg(string service,string lat,string lng)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    double Distance = 0.0;
                    List<string> orgName= new List<string>();
                    orgName.Add("-Select Organization-");
                    var serviceProviders = ctx.Services.Where(x => (x.Name == service) && (x.Organization != "Independent")).ToList<Service>();
                    for (int i = 0; i < serviceProviders.Count; i++)
                    {
                        // Organization org = ctx.Organizations.Where(x => x.Name == serviceProviders[i].Organization).FirstOrDefault();
                        string OrgName = serviceProviders[i].Organization;
                        List<Location> OrgLoationLocations = ctx.Locations.Where(x => x.OrgName == OrgName).ToList<Location>();
                        for (int j = 0; j < OrgLoationLocations.Count; j++)
                        {
                            Distance = distance(double.Parse(OrgLoationLocations[j].Lat), double.Parse(lat), double.Parse(OrgLoationLocations[j].Long), double.Parse(lng));
                            if (Distance < double.Parse(OrgLoationLocations[j].Radius))
                            {
                                orgName.Add(OrgLoationLocations[j].OrgName);
                                break;
                            }
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, (orgName));
                }
            }
            catch (Exception)
            {

                throw;
            }
        
        }
        [HttpPost]
        public HttpResponseMessage GetIndAvailableEmp(AppointmentRequestDTO appointmentRequestDTO)
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
                    List<Employee> EmployeeList = ctx.Employees.OrderBy(x => x.eid).Where(x => (x.OrgName == appointmentRequestDTO.orgname) && (x.Shift == appointmentRequestDTO.shift)).ToList<Employee>();
                    if (EmployeeList.Count == 0)
                    {
                        reaseon = "No Employee in available in this Organization/shift/department";
                        return Request.CreateResponse(HttpStatusCode.OK, (AvaliableEmployee, UnAvaliableEmployee));
                    }
                    else
                    { 
                        for (int i = 0; i < EmployeeList.Count; i++)
                        {
                             Distance = distance(double.Parse(EmployeeList[i].Lat), double.Parse(appointmentRequestDTO.lat), double.Parse(EmployeeList[i].Long), double.Parse(appointmentRequestDTO.lng));
                           
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
                                        if (EmployeeSchedule[j].date == appointmentRequestDTO.date && EmployeeSchedule[j].timeslot == appointmentRequestDTO.time)
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is busy in given date and time";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else if (EmployeeSchedule[j].date == appointmentRequestDTO.date && EmployeeSchedule[j].timeslot == "Leave")
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is on full-day leave";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else if (EmployeeSchedule[j].date == appointmentRequestDTO.date && EmployeeSchedule[j].timeslot == appointmentRequestDTO.time + "L")
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
                            return Request.CreateResponse(HttpStatusCode.OK, (AvaliableEmployee, UnAvaliableEmployee));
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpPost]
        public HttpResponseMessage IndRepetedAppointment(RepeatedAppointmentDTO repeatedAppointmentDTO)
        {
            try
            {   
                using (var ctx = new HHCEntities())
                {
                    double Distance = 0.0;  
                    string reaseon = "";
                    bool Avaliblity = false;
                    Employee selectedEmp = new Employee();
                    Appointment addAppointment = new Appointment();
                    List<RepeatedResponseDTO> response = new List<RepeatedResponseDTO>();
                    List<string> DateList = JsonConvert.DeserializeObject<List<string>>(repeatedAppointmentDTO.date);
                    addAppointment.orgname = repeatedAppointmentDTO.orgname;
                    addAppointment.service = repeatedAppointmentDTO.service;
                    addAppointment.uid = int.Parse(repeatedAppointmentDTO.uid);
                    addAppointment.username = repeatedAppointmentDTO.username;
                    addAppointment.pfname = repeatedAppointmentDTO.pfname;
                    addAppointment.plname = repeatedAppointmentDTO.plname;
                    addAppointment.phnum = repeatedAppointmentDTO.phnum;
                    addAppointment.timeslot = repeatedAppointmentDTO.time;
                    addAppointment.timeduration = repeatedAppointmentDTO.timeduration;
                    addAppointment.status = repeatedAppointmentDTO.status;
                    addAppointment.ratings = int.Parse(repeatedAppointmentDTO.ratings);
                    Schedule addSchedule = new Schedule();
                    addSchedule.orgname = repeatedAppointmentDTO.orgname;
                    addSchedule.timeslot = repeatedAppointmentDTO.time;
                    addSchedule.shift = repeatedAppointmentDTO.shift;
                    addSchedule.ratings = double.Parse(repeatedAppointmentDTO.ratings);
                    var ServiceList = ctx.Services.Where(x => x.Name == repeatedAppointmentDTO.service).ToList<Service>();
                    var Dep = ServiceList.FirstOrDefault().Staff;
                    addAppointment.dep = Dep;
                    addSchedule.dep = Dep;
                    List<Employee> EmployeeList = ctx.Employees.OrderByDescending(x => x.Raitings).Where(x => (x.OrgName == repeatedAppointmentDTO.orgname) && (x.Department == Dep ) && (x.Shift == repeatedAppointmentDTO.shift)).ToList<Employee>();
                    if (EmployeeList.Count == 0)
                    {
                        reaseon = "No Employee in available in this Organization/shift/department";
                        return Request.CreateResponse(HttpStatusCode.OK, (reaseon));
                    }
                    else
                    {
                        for (int i = 0; i < EmployeeList.Count; i++)
                        {
                            Distance = distance(double.Parse(EmployeeList[i].Lat), double.Parse(repeatedAppointmentDTO.lat), double.Parse(EmployeeList[i].Long), double.Parse(repeatedAppointmentDTO.lng));

                            if (Distance > double.Parse(EmployeeList[i].Radius))
                            {
                                Avaliblity = false;
                                EmployeeList[i].Availablity = Avaliblity;
                                EmployeeList[i].Distance = Distance.ToString();
                               // UnAvaliableEmployee.Add(EmployeeList[i]);

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
                                   // AvaliableEmployee.Add(EmployeeList[i]);
                                    selectedEmp = EmployeeList[i];
                                    addAppointment.eid = selectedEmp.eid;
                                    addSchedule.eid = selectedEmp.eid;
                                    addSchedule.fname = selectedEmp.Fname;
                                    addSchedule.lname = selectedEmp.Lname;
                                    addAppointment.empname = selectedEmp.Fname +" "+ selectedEmp.Lname;
                                    for (int w = 0; w < DateList.Count; w++)
                                    {
                                        addAppointment.date = DateList[w];
                                        addSchedule.date = DateList[w];
                                        db.Appointments.Add(addAppointment);
                                        response.Add(new RepeatedResponseDTO() { date = addAppointment.date, time = addAppointment.timeslot, employee = addAppointment.empname, status = "Booked" });
                                        db.Schedules.Add(addSchedule);
                                        db.SaveChanges();
                                        DateList[w] = "Booked";
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < DateList.Count ; j++)
                                    {
                                            for (int q = 0; q < EmployeeSchedule.Count; q++)
                                            {
                                                if (EmployeeSchedule[q].date == DateList[j] && (EmployeeSchedule[q].timeslot == repeatedAppointmentDTO.time || EmployeeSchedule[q].timeslot == "Leave" || EmployeeSchedule[q].timeslot == repeatedAppointmentDTO.time + "L"))
                                                {
                                                    Avaliblity = false;
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
                                                // AvaliableEmployee.Add(EmployeeList[i]);
                                                selectedEmp = EmployeeList[i];
                                                addAppointment.eid = selectedEmp.eid;
                                                addSchedule.eid = selectedEmp.eid;
                                                addSchedule.fname = selectedEmp.Fname;
                                                addSchedule.lname = selectedEmp.Lname;
                                                addAppointment.empname = selectedEmp.Fname + " " + selectedEmp.Lname;
                                                addAppointment.date = DateList[j];
                                                addSchedule.date = DateList[j];
                                                db.Appointments.Add(addAppointment);
                                                response.Add(new RepeatedResponseDTO() { date = addAppointment.date, time = addAppointment.timeslot, employee = addAppointment.empname, status = "Booked" });
                                                db.Schedules.Add(addSchedule);
                                                db.SaveChanges();
                                                DateList[j] = "Booked";
                                            }
                                            else
                                            {
                                            response.Add(new RepeatedResponseDTO() { date = DateList[j], time = repeatedAppointmentDTO.time, employee = "No Employee Available", status = "Unbooked" });
                                            continue;
                                            }
                                        
                                    }
                                }
                            }
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, (response));
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpPost]
        public HttpResponseMessage OrgRepetedAppointment(RepeatedAppointmentDTO repeatedAppointmentDTO)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    double Distance = 0.0;
                    string reaseon = "";
                    bool Avaliblity = false;
                    Employee selectedEmp = new Employee();
                    List<string> DateList = JsonConvert.DeserializeObject<List<string>>(repeatedAppointmentDTO.date);
                    List<RepeatedResponseDTO> response = new List<RepeatedResponseDTO>();
                    Appointment addAppointment = new Appointment();
                    addAppointment.orgname = repeatedAppointmentDTO.orgname;
                    addAppointment.service = repeatedAppointmentDTO.service;
                    addAppointment.uid = int.Parse(repeatedAppointmentDTO.uid);
                    addAppointment.username = repeatedAppointmentDTO.username;
                    addAppointment.pfname = repeatedAppointmentDTO.pfname;
                    addAppointment.plname = repeatedAppointmentDTO.plname;
                    addAppointment.gender = repeatedAppointmentDTO.gender;
                    addAppointment.phnum = repeatedAppointmentDTO.phnum;
                    addAppointment.timeslot = repeatedAppointmentDTO.time;
                    addAppointment.timeduration = repeatedAppointmentDTO.timeduration;
                    addAppointment.status = repeatedAppointmentDTO.status;
                    addAppointment.ratings = int.Parse(repeatedAppointmentDTO.ratings);
                    Schedule addSchedule = new Schedule();
                    addSchedule.orgname = repeatedAppointmentDTO.orgname;
                    addSchedule.timeslot = repeatedAppointmentDTO.time;
                    addSchedule.shift = repeatedAppointmentDTO.shift;
                    addSchedule.ratings = double.Parse(repeatedAppointmentDTO.ratings);
                    var ServiceList = ctx.Services.Where(x => x.Name == repeatedAppointmentDTO.service).ToList<Service>();
                    var Dep = ServiceList.FirstOrDefault().Staff;
                    addAppointment.dep = Dep;
                    addSchedule.dep = Dep;
                    List<Employee> EmployeeList = ctx.Employees.OrderByDescending(x => x.Raitings).Where(x => (x.OrgName == repeatedAppointmentDTO.orgname) && (x.Department == Dep) && (x.Shift == repeatedAppointmentDTO.shift)).ToList<Employee>();
                    if (EmployeeList.Count == 0)
                    {
                        response.FirstOrDefault().status = "No Employee in available in this Organization/shift/department";
                        return Request.CreateResponse(HttpStatusCode.OK, (response));
                    }
                    else
                    {
                        for (int i = 0; i < EmployeeList.Count; i++)
                        {
                           // Distance = distance(double.Parse(EmployeeList[i].Lat), double.Parse(repeatedAppointmentDTO.lat), double.Parse(EmployeeList[i].Long), double.Parse(repeatedAppointmentDTO.lng));

                            //if (Distance > double.Parse(EmployeeList[i].Radius))
                            //{
                            //    Avaliblity = false;
                            //    EmployeeList[i].Availablity = Avaliblity;
                            //    EmployeeList[i].Distance = Distance.ToString();
                            //    // UnAvaliableEmployee.Add(EmployeeList[i]);

                            //}
                            //else
                            //{
                                int eid = EmployeeList[i].eid;
                                List<Schedule> EmployeeSchedule = ctx.Schedules.Where(x => x.eid == eid).ToList<Schedule>();
                                if (EmployeeSchedule.Count == 0)
                                {
                                    Avaliblity = true;
                                    EmployeeList[i].Availablity = Avaliblity;
                                    EmployeeList[i].Distance = Distance.ToString();
                                    // AvaliableEmployee.Add(EmployeeList[i]);
                                    selectedEmp = EmployeeList[i];
                                    addAppointment.eid = selectedEmp.eid;
                                    addSchedule.eid = selectedEmp.eid;
                                    addSchedule.fname = selectedEmp.Fname;
                                    addSchedule.lname = selectedEmp.Lname;
                                    addAppointment.empname = selectedEmp.Fname + " " + selectedEmp.Lname;
                                    for (int w = 0; w < DateList.Count; w++)
                                    {
                                        addAppointment.date = DateList[w];
                                        addSchedule.date = DateList[w];
                                        response.Add( new RepeatedResponseDTO() {date = addAppointment.date, time= addAppointment.timeslot, employee= addAppointment.empname, status="Booked"});
                                        db.Appointments.Add(addAppointment); 
                                        db.Schedules.Add(addSchedule);
                                        db.SaveChanges();
                                        DateList[w] = DateList[w]+ "-Booked";
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < DateList.Count; j++)
                                    {
                                        for (int q = 0; q < EmployeeSchedule.Count; q++)
                                        {
                                            if (EmployeeSchedule[q].date == DateList[j] && (EmployeeSchedule[q].timeslot == repeatedAppointmentDTO.time || EmployeeSchedule[q].timeslot == "Leave" || EmployeeSchedule[q].timeslot == repeatedAppointmentDTO.time + "L"))
                                            {
                                                Avaliblity = false;
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
                                            // AvaliableEmployee.Add(EmployeeList[i]);
                                            selectedEmp = EmployeeList[i];
                                            addAppointment.eid = selectedEmp.eid;
                                            addSchedule.eid = selectedEmp.eid;
                                            addSchedule.fname = selectedEmp.Fname;
                                            addSchedule.lname = selectedEmp.Lname;
                                            addAppointment.empname = selectedEmp.Fname + " " + selectedEmp.Lname;
                                            addAppointment.date = DateList[j];
                                            addSchedule.date = DateList[j];
                                            db.Appointments.Add(addAppointment);
                                            response.Add(new RepeatedResponseDTO() { date = addAppointment.date, time = addAppointment.timeslot, employee = addAppointment.empname, status = "Booked" });
                                            db.Schedules.Add(addSchedule);
                                            db.SaveChanges();
                                            DateList[j] = DateList[j] +"-Booked";
                                        }
                                        else
                                        {
                                        response.Add(new RepeatedResponseDTO() { date = DateList[j], time = repeatedAppointmentDTO.time, employee ="No Employee Available", status = "Unbooked" });
                                        continue;
                                        }

                                    }
                                }
                           // }
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, (response));
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpPost]
        public HttpResponseMessage GetOrgAvailableEmp(AppointmentRequestDTO appointmentRequestDTO)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                   // double Distance = 0.0;
                    string reaseon = "";
                    bool Avaliblity = false;
                   // string Zone = "";
                    List<Employee> AvaliableEmployee = new List<Employee>();
                    List<Employee> UnAvaliableEmployee = new List<Employee>();
                   // List<Location> OrgLoations = ctx.Locations.Where(x => x.OrgName == appointmentRequestDTO.orgname).ToList<Location>();
                    var dep = ctx.Services.Where(x => (x.Name == appointmentRequestDTO.service)&&(x.Organization == appointmentRequestDTO.orgname)).ToList<Service>();
                    string department = dep.FirstOrDefault().Staff;
                    //for (int i = 0; i < OrgLoations.Count; i++)
                    //{
                    //    Distance = distance(double.Parse(OrgLoations[i].Lat), double.Parse(appointmentRequestDTO.lat), double.Parse(OrgLoations[i].Long), double.Parse(appointmentRequestDTO.lng));
                    //    if (Distance < double.Parse(OrgLoations[i].Radius))
                    //    {
                    //        Zone = OrgLoations[i].Zones;
                    //        break;
                    //    }
                    //}

                    List<Employee> EmployeeList = ctx.Employees.OrderBy(x => x.eid).Where(x => (x.OrgName == appointmentRequestDTO.orgname)/*&& (x.Zone == Zone)*/ && (x.Department == department) && (x.Shift == appointmentRequestDTO.shift)).ToList<Employee>();
                    if (EmployeeList.Count == 0)
                    {
                        reaseon = "No Employee in available in this Organization/shift/department";
                        return Request.CreateResponse(HttpStatusCode.OK, (AvaliableEmployee, UnAvaliableEmployee));
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
                                   // EmployeeList[i].Distance = Distance.ToString();
                                    AvaliableEmployee.Add(EmployeeList[i]);
                                }
                                else
                                {
                                    for (int j = 0; j < EmployeeSchedule.Count; j++)
                                    {
                                        if (EmployeeSchedule[j].date == appointmentRequestDTO.date && EmployeeSchedule[j].timeslot == appointmentRequestDTO.time)
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is busy in given date and time";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else if (EmployeeSchedule[j].date == appointmentRequestDTO.date && EmployeeSchedule[j].timeslot == "Leave")
                                        {
                                            Avaliblity = false;
                                            reaseon = "Employee is on full-day leave";
                                            //UnAvaliableEmployee.Add(EmployeeList[i]);
                                            break;
                                        }
                                        else if (EmployeeSchedule[j].date == appointmentRequestDTO.date && EmployeeSchedule[j].timeslot == appointmentRequestDTO.time + "L")
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
                                        //EmployeeList[i].Distance = Distance.ToString();
                                        AvaliableEmployee.Add(EmployeeList[i]);
                                    }
                                    else
                                    {
                                        EmployeeList[i].Availablity = Avaliblity;
                                        //EmployeeList[i].Distance = Distance.ToString();
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
                            return Request.CreateResponse(HttpStatusCode.OK, (AvaliableEmployee, UnAvaliableEmployee));
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
