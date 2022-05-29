using HhcApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HhcApi.Controllers
{
    public class UserController : ApiController
    {
        HHCEntities db = new HHCEntities();


        [HttpGet]
        public HttpResponseMessage GetAllServices(string OrgOrInd)  
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    List<string> service = new List<string>();
                    if (OrgOrInd == "Organization")
                    {
                        var studentList = ctx.Services.Where(a => a.Organization != "Independent").OrderBy(a => a.Staff).Distinct().ToList<Service>();  
                        service.Add("-Select Service-");
                        if (studentList != null)
                        {
                            for (int i = 0; i < studentList.Count; i++)
                            {
                                string Orgname = studentList[i].Name.ToString();
                                service.Add(Orgname);
                            }
                           // return Request.CreateResponse(HttpStatusCode.OK, service);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                    }
                    else if(OrgOrInd == "Independent")
                    {
                        var studentList = ctx.Services.Where(a => a.Organization == "Independent").OrderBy(a => a.Staff).Distinct().ToList<Service>();
                    
                        service.Add("-Select Service-");
                        if (studentList != null)
                        {
                            for (int i = 0; i < studentList.Count; i++)
                            {
                                string Orgname = studentList[i].Name.ToString();
                                service.Add(Orgname);
                            }
                           
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, service);
                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }


        [HttpGet]
        public HttpResponseMessage GetDropCity()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees.Where(a => (a.Status == "Accepted" )).OrderBy(a => a.City).Distinct().ToList<Employee>();
                    List<string> City = new List<string>();
                    City.Add("---Select City---");
                    if (studentList != null)
                    {
                        for (int i = 0; i < studentList.Count; i++)
                        {
                            string cityname = studentList[i].City.ToString();
                            City.Add(cityname);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, City.Distinct());
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

       
        [HttpGet]
        public HttpResponseMessage GetDropOrg(string city)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Organizations.Where(a =>( a.Status == "Accepted" && a.City == city)).OrderBy(a => a.Name).Distinct().ToList<Organization>();
                    List<string> Org = new List<string>();
                    Org.Add("-Select Organization-");
                    if (studentList != null)
                    {
                        for (int i = 0; i < studentList.Count; i++)
                        {
                            string Orgname = studentList[i].Name.ToString();
                            Org.Add(Orgname);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, Org);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpGet]
        public HttpResponseMessage GetDepartments(string Org,string city)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees.Where(m => (m.OrgName == Org && m.City == city)).GroupBy(m => m.Department).Select(x => x.FirstOrDefault()).ToList();
                    List<string> Dep = new List<string>();
                    Dep.Add("-Select Staff-");
                    if (studentList != null)
                    {
                        for (int i = 0; i < studentList.Count; i++)
                        {
                            string departments = studentList[i].Department.ToString();
                            Dep.Add(departments);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, Dep);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpGet]
        public HttpResponseMessage ViewServices(String Org, String dep)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Services.SqlQuery("select * from services where Organization='" + Org + "' and Staff='" + dep + "'").ToList<Service>();
                    List<string> service = new List<string>();
                    service.Add("-Select Service-");
                    if (studentList != null)
                    {
                        for (int i = 0; i < studentList.Count; i++)
                        {
                            string departments = studentList[i].Name.ToString();
                            service.Add(departments);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, service);
                    }
                    else
                    {

                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpGet]
        public HttpResponseMessage GetAppointments(int uid)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Appointments.SqlQuery("Select * From Appointments where status='Pending' and uid='" + uid + "'").ToList<Appointment>();
                    if (studentList != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, studentList);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }



        [HttpGet]
        public HttpResponseMessage GetCompletedAppointments(int uid)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Appointments.SqlQuery("Select * From Appointments where status='Complete' and uid='" + uid + "'").ToList<Appointment>();
                    if (studentList != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, studentList);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                }
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }


        [HttpPatch]
        public HttpResponseMessage UpdateRatings(int aid, string rating)
        {

            using (var ctx = new HHCEntities())
            {
                var updateOrg = ctx.Database.ExecuteSqlCommand("update Appointments set ratings= '" + rating + "' where aid='" + aid + "' ");
                if (updateOrg != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, updateOrg);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }

        }

        [HttpGet]
        public HttpResponseMessage GetUserDetails(String Username, String Password)
        {
            try
            {
                var userFound = db.Signups.FirstOrDefault(u => u.Username == Username && u.Password == Password);
                if (userFound == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid user Name or Password");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, userFound);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }
    }
}
