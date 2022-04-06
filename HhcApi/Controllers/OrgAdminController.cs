using HhcApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HhcApi.Controllers
{
    public class OrgAdminController : ApiController
    {

        HHCEntities db = new HHCEntities();

        [HttpPost]
        public HttpResponseMessage AddService(Service obj)
        {

            try
            {
                db.Services.Add(obj);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddEmployee(Employee user)
        {

            try
            {
                db.Employees.Add(user);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage ViewServices(String Org)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Services
                                        .SqlQuery("select * from services where Organization='" + Org + "'")
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
        public HttpResponseMessage GetOrgDetails(String Username, String Password)
        {
            try
            {
                var userFound = db.Organizations.FirstOrDefault(u => u.Username == Username && u.Password == Password);
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

        [HttpGet]
        public HttpResponseMessage DepDropOrg(String dep, String org)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees
                                        .SqlQuery("Select * from Employee where Department='" + dep + "' and OrgName='" + org + "' and status='Accepted'").ToList<Employee>();
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


    }
}
