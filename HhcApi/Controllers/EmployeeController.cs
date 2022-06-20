using HhcApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HhcApi.Controllers
{
    public class EmployeeController : ApiController
    {
        HHCEntities db = new HHCEntities();

        [HttpPatch]
        public HttpResponseMessage CompleteAppointment(int id)
        {

            using (var ctx = new HHCEntities())
            {
                var updateOrg = ctx.Database.ExecuteSqlCommand("UPDATE Appointments SET Status = 'Complete' WHERE aid='" + id + "' ");
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

        [HttpDelete]
        public HttpResponseMessage DeleteSchedule(int eid, String date, String time)
        {
            try
            {
                var entity = db.Schedules.Where(e => e.eid == eid).Where(e => e.date == date).Where(e => e.timeslot == time).FirstOrDefault();
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Organization with id=" + eid.ToString() + "not found to delete");
                }
                else
                {
                    db.Schedules.Remove(entity);
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
        public HttpResponseMessage GetEmpAppointments(int eid)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Appointments.SqlQuery("Select * From Appointments where status='Pending' and eid='" + eid + "'").ToList<Appointment>();
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
                return Request.CreateResponse(HttpStatusCode.OK, data);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }
   

        [HttpPost]
        public HttpResponseMessage AddEmpSchedule(Schedule user)
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
       
        [HttpPatch]
        public HttpResponseMessage NoLeave(int eid)
        {

            using (var ctx = new HHCEntities())
            {
                var updateOrg = ctx.Database.ExecuteSqlCommand("update Schedule set NoLeave= NoLeave+1 where eid='" + eid + "' ");
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
        public HttpResponseMessage UpdatePendAppSubtract(int eid)
        {

            using (var ctx = new HHCEntities())
            {
                var updateOrg = ctx.Database.ExecuteSqlCommand("update Schedule set noOfpndApnt= noOfpndApnt-1 where eid='" + eid + "' ");
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
        public HttpResponseMessage GetScheduleObj(int eid)
        {
            try
            {
                var userFound = db.Schedules.FirstOrDefault(u => u.eid == eid);
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
