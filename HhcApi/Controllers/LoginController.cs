using HhcApi.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HhcApi.Controllers
{
    public class LoginController : ApiController
    {
        HHCEntities db = new HHCEntities();

       

        [HttpPost]
        public HttpResponseMessage AddAppointment(Appointment user)
        {

            try
            {
                db.Appointments.Add(user);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

       

       
       

       

        [HttpPost]
        public HttpResponseMessage AddDatedSchedule(Schedule user)
        {

            try
            {
                db.Schedules.Add(user);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddNullDatedSchedule(Schedule obj)
        {
            try
            {
                var data = db.Schedules.Find(obj.sid);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Employee Does Not exist");
                }
                db.Entry(data).CurrentValues.SetValues(obj);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, data);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpPatch]
        public HttpResponseMessage UpdatePendApp(int eid)
        {

            using (var ctx = new HHCEntities())
            {
                var updateOrg = ctx.Database.ExecuteSqlCommand("update Schedule set noOfpndApnt= noOfpndApnt+1 where eid='" + eid + "' ");
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
        public HttpResponseMessage GetSchedule(String orgname, String dep, String shift, String city)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Schedules.SqlQuery("Select * From Schedule where orgname='" + orgname + "' and dep='" + dep + "' and shift='" + shift + "' and city='" + city + "' and noOfpndApnt <3 order by ratings desc , noOfpndApnt desc ,eid desc").ToList<Schedule>();
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
        public HttpResponseMessage GetAllSchedule(String orgname, String dep, String shift, String city)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Schedules.SqlQuery("Select * From Schedule where orgname='" + orgname + "' and dep='" + dep + "' and shift='" + shift + "' and city='" + city + "' and noOfpndApnt <3 order by ratings desc , noOfpndApnt desc ,eid desc").ToList<Schedule>();
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
        public HttpResponseMessage CalculateRatings(int eid)
        {
            
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var AptList = ctx.Appointments.Where(a => a.eid == eid).ToList<Appointment>();
                    if (AptList != null)
                    {
                      
                        return Request.CreateResponse(HttpStatusCode.OK, AptList);
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
        public HttpResponseMessage acceptedorg()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Organizations.OrderBy(a => a.City).Where(o => o.Status == "Accepted").ToList<Organization>();
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
        public HttpResponseMessage Countnotification(int id)
        {
            try
            {
                var lowcgpa = db.Employees.Count(b => b.eid == id);

                return Request.CreateResponse(HttpStatusCode.OK, lowcgpa);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage verifylogin(Login obj)
        {
            try
            {
                var userFound = db.Logins.FirstOrDefault(u => u.Username == obj.Username && u.Password == obj.Password);
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

        [HttpPost]
        public HttpResponseMessage AddRegister(Signup user)
        {

            try
            {
                db.Signups.Add(user);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

       

        [HttpPost]
        public HttpResponseMessage AddLogin(Login reg)
        {

            try
            {

                db.Logins.Add(reg);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

      

        [HttpGet]
        public HttpResponseMessage ViewEmp()
        {
            try
            {

                var studentList = db.Employees.OrderBy(s => s.Fname).ToList<Employee>();
                if (studentList != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, studentList);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }



            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        //[HttpPatch]
        //public HttpResponseMessage UpdateEmp(int id)
        //{

        //    using (var ctx = new HHCEntities())
        //    {
        //        var updateOrg = ctx.Database.ExecuteSqlCommand("UPDATE Employee SET Status = 'Accepted' WHERE id='" + id + "' ");
        //        if (updateOrg != null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK, updateOrg);
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound);
        //        }
        //    }

        //}

       

        [HttpGet]
        public HttpResponseMessage GetEmpByUsername(String Username, String Password)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees.FirstOrDefault(u => u.Username == Username && u.Password == Password);
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
        public HttpResponseMessage IndEmp(String dep, String org)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees
                                        .SqlQuery("Select * from Employee where Department='" + dep + "' and OrgName='" + org + "'and Status='Accepted'").ToList<Employee>();
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


       

        [HttpDelete]
        public HttpResponseMessage DeleteEmp(int id)
        {
            try
            {
                var entity = db.Employees.FirstOrDefault(e => e.eid == id);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with id=" + id.ToString() + "not found to delete");
                }
                else
                {
                    db.Employees.Remove(entity);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

       


       



        //[HttpGet]
        //public HttpResponseMessage ViewSerOrg()
        //{
        //    try
        //    {
        //        using (var ctx = new HHCEntities())
        //        {
        //            var studentList = ctx.Services
        //                                .SqlQuery("select * from services where Organization=")
        //                                .ToList<Service>();
        //            if (studentList != null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, studentList);
        //            }
        //            else
        //            {
        //                return Request.CreateResponse(HttpStatusCode.NotFound);
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

        //    }
        //}

      

        [HttpPost]
        public HttpResponseMessage UpdateService(Service obj)
        {
            try
            {
                var data = db.Services.Find(obj.id);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Employee Does Not exist");
                }
                db.Entry(data).CurrentValues.SetValues(obj);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, data);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

    }

}

