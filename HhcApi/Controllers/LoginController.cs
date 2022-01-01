using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HhcApi.Models;

namespace HhcApi.Controllers
{
    public class LoginController : ApiController
    {
        HHCEntities db = new HHCEntities();

        [HttpGet]
        public HttpResponseMessage GetOrg()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Organizations
                                        .SqlQuery("Select * from Organizations where Status ='Pending'")
                                        .ToList<Organization>();
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
        public HttpResponseMessage GetUser(String  Username ,String Password)
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

        [HttpGet]
        public HttpResponseMessage GetIndPendingEmp()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees
                                        .SqlQuery("Select * from Employee where Status ='Pending'")
                                        .ToList<Employee>();
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
        public HttpResponseMessage GetDropOrg()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Organizations
                                        .SqlQuery("Select * from Organizations")
                                        .ToList<Organization>();
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
        public HttpResponseMessage acceptedorg( )
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Organizations
                                        .SqlQuery("Select * from Organizations where Status ='Accepted'")
                                        .ToList<Organization>();
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

        [HttpDelete]
        public HttpResponseMessage DeleteOrg(int id)
        {
            try
            {
                var entity = db.Organizations.FirstOrDefault(e => e.id == id);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Organization with id=" + id.ToString() + "not found to delete");
                }
                else
                {
                    db.Organizations.Remove(entity);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeletePendingEmpl(int id)
        {
            try
            {
                var entity = db.Employees.FirstOrDefault(e => e.eid == id);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Organization with id=" + id.ToString() + "not found to delete");
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

        [HttpPost]
        public HttpResponseMessage AddOrg(Organization obj)
        {

            try
            {

                db.Organizations.Add(obj);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPatch]
        public HttpResponseMessage UpdateOrgStatus(int id) {

            using (var ctx = new HHCEntities())
            {
                var updateOrg  = ctx.Database.ExecuteSqlCommand("UPDATE Organizations SET Status = 'Accepted' WHERE id='" + id + "' ");
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

        [HttpPatch]
        public HttpResponseMessage UpdateEpmlStatus(int id)
        {

            using (var ctx = new HHCEntities())
            {
                var updateOrg = ctx.Database.ExecuteSqlCommand("UPDATE Employee SET Status = 'Accepted' WHERE eid='" + id + "' ");
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

        [HttpPost]
        public HttpResponseMessage PatientDetails(Patient user)
        {

            try
            {
                db.Patients.Add(user);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
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
        public HttpResponseMessage ViewEmp()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees
                                        .SqlQuery("Select * from Employee")
                                        .ToList<Employee>();
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
        public HttpResponseMessage GetEmp(int id)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees
                                        .SqlQuery("Select * from Employee where eid='" + id + "'").ToList<Employee>();
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
        public HttpResponseMessage DepDropOrg(String dep)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees
                                        .SqlQuery("Select * from Employee where Department='" + dep + "'").ToList<Employee>();
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
        public HttpResponseMessage IndEmp(String dep)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees
                                        .SqlQuery("Select * from Employee where Department='" + dep + "' and OrgName='Independent'and Status='Accepted'").ToList<Employee>();
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


        [HttpPost]
        public HttpResponseMessage ModifyEmployee(Employee obj)
        {
            try
            {
                var data = db.Employees.Find(obj.eid);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Employee Does Not exist");
                }
                db.Entry(data).CurrentValues.SetValues(obj);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK,data);

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

        [HttpPost]
        public HttpResponseMessage AddService(Service obj)
        {

            try
            {
                db.Services.Add(obj);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK,obj);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage ViewServices( String Org,String dep)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Services
                                        .SqlQuery("select * from services where Organization='" + Org + "' and staff = '" + dep+"' ")
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
        public HttpResponseMessage ViewSerOrg()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Services
                                        .SqlQuery("select * from services")
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

        [HttpDelete]
        public HttpResponseMessage DeleteService(int id)
        {
            try
            {
                var entity = db.Services.FirstOrDefault(e => e.id == id);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with id=" + id.ToString() + "not found to delete");
                }
                else
                {
                    db.Services.Remove(entity);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


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

