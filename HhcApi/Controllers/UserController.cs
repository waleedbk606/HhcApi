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
        public HttpResponseMessage GetDropOrg()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Organizations.Where(a => a.Status == "Accepted").OrderBy(a => a.Name).ToList<Organization>();
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
        public HttpResponseMessage GetDepartments(String Org)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees.Where(m => m.OrgName == Org).GroupBy(m => m.Department).Select(x => x.FirstOrDefault()).ToList();
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
        public HttpResponseMessage ViewServices(String Org, String dep)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Services
                                        .SqlQuery("select * from services where Organization='" + Org + "' and Staff='" + dep + "'")
                                        .ToList<Service>();
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
