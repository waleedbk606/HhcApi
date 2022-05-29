using HhcApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HhcApi.Controllers
{
    public class HhcAdminController : ApiController
    {
        HHCEntities db = new HHCEntities();


        [HttpGet]
        public HttpResponseMessage GetLocations()   
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Locations.OrderBy(a => a.OrgName).ToList<Location>();
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

        [HttpPost]
        public HttpResponseMessage AddLocations(Location obj)
        {

            try
            {

                db.Locations.Add(obj);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
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

        [HttpGet]
        public HttpResponseMessage GetIndPendingEmp()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Employees.OrderBy(a => a.Fname).Where(a => a.Status == "Pending").ToList<Employee>();
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
        public HttpResponseMessage UpdateOrgStatus(int id)
        {

            using (var ctx = new HHCEntities())
            {
                var updateOrg = ctx.Database.ExecuteSqlCommand("UPDATE Organizations SET Status = 'Accepted' WHERE id='" + id + "' ");
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
        public HttpResponseMessage DeleteOrg(int id)
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var entity = db.Organizations.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                    
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Organization with id=" + id.ToString() + "not found to delete");
                    }
                    else
                    {
                        var locations = ctx.Database.ExecuteSqlCommand("Delete from Locations WHERE OrgName='" + entity.Name + "' ");
                        db.Organizations.Remove(entity);
                      
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetOrg()
        {
            try
            {
                using (var ctx = new HHCEntities())
                {
                    var studentList = ctx.Organizations.SqlQuery("Select * From Organizations where status ='Pending'").ToList<Organization>();
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
        public HttpResponseMessage IndEmphhc(String dep)
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

    }
}
