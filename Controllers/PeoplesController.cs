using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Project.Models.Database;

namespace Project.Controllers
{
    public class PeoplesController : ApiController
    {
        private AdminContext db = new AdminContext();

        // GET: api/Peoples
        public ResponseList GetPeoples(int? page = 1, int? per_page = 10, string search = "")
        {
            ResponseList li = new ResponseList();
            try
            {
                if (page.Value <= 0)
                    page = 1;
                int pg = (page.Value - 1);
                var skip = pg * per_page.Value;

                li.Status = true;
                li.Message = "List of People";
                if (String.IsNullOrWhiteSpace(search))
                    li.Data = db.Peoples.OrderBy(x => x.ID).Skip(skip).Take(per_page.Value).Select(x => new { x.ID, x.Image, x.FullName, x.Email, x.Type }).ToList();
                else
                    li.Data = db.Peoples.Where(x => x.FullName.Contains(search)).OrderBy(x => x.ID).Skip(skip).Take(per_page.Value).Select(x => new { x.ID, x.Image, x.FullName, x.Email, x.Type }).ToList();
                return li;
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // GET: api/Peoples
        public ResponseList GetMembers(int? page = 1, int? per_page = 10, string search = "")
        {
            ResponseList li = new ResponseList();
            try
            {
                if (page.Value <= 0)
                    page = 1;
                int pg = (page.Value - 1);
                var skip = pg * per_page.Value;

                li.Status = true;
                li.Message = "List of People";
                if (String.IsNullOrWhiteSpace(search))
                    li.Data = db.Peoples.Where(x => x.Type != "Admin").OrderBy(x => x.ID).Skip(skip).Take(per_page.Value).Select(x => new { x.ID, x.Image, x.FullName, x.Email, x.Type }).ToList();
                else
                    li.Data = db.Peoples.Where(x => x.FullName.Equals(search) && x.Type != "Admin").OrderBy(x => x.ID).Skip(skip).Take(per_page.Value).Select(x => new { x.ID, x.Image, x.FullName, x.Email, x.Type }).ToList();
                return li;
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // GET: api/Peoples/5
        [ResponseType(typeof(Peoples))]
        public async Task<ResponseObject> GetPeoplesById(int id)
        {
            ResponseObject temp = new ResponseObject();
            Object peoples = await db.Peoples.Select(x => new { x.ID, x.Image, x.FullName, x.Email, x.Phone, x.Address, x.Type }).FirstOrDefaultAsync(x => x.ID == id);
            if (peoples == null)
            {
                temp.Status = false;
                temp.Message = "404 Not Found";
                return temp;
            }
            else
            {
                temp.Status = true;
                temp.Message = "Record you asked.";
                temp.Data = peoples;
                return temp;
            }
        }

        // PUT: api/Peoples/5
        [ResponseType(typeof(void))]
        public async Task<ResponseObject> PutPeoples(int id, Peoples peoples)
        {
            ResponseObject obj = new ResponseObject();
            if (!ModelState.IsValid)
            {
                obj.Status = false;
                obj.Message = "Model state is not valid.";
                return obj;
            }

            if (id != peoples.ID)
            {
                obj.Status = false;
                obj.Message = "404 Record Not Found.";
                return obj;
            }

            bool check = db.Peoples.Count(x => x.Email.Equals(peoples.Email) && x.ID != id) > 0;
            if (check)
            {
                obj.Status = false;
                obj.Message = "Email Already Exists.";
                return obj;
            }
            else
            {
                if (String.IsNullOrWhiteSpace(peoples.Password))
                {  
                    Peoples temp = await db.Peoples.FirstOrDefaultAsync(x => x.ID == id);
                    string pass = temp.Password;
                    if (temp != null)
                        peoples.Password = pass;

                    temp.Email = peoples.Email;
                    temp.FullName = peoples.FullName;
                    temp.Phone = peoples.Phone;
                    temp.Address = peoples.Address;
                    temp.Image = peoples.Image;
                    temp.Type = peoples.Type;
                    temp.Password = pass; 

                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PeoplesExists(id))
                        {
                            obj.Status = false;
                            obj.Message = "404 Record Not Found.";
                            return obj;
                        }
                        else
                        {
                            obj.Status = false;
                            obj.Message = "There was an error.Try again later.";
                            return obj;
                        }
                    }

                    obj.Status = true;
                    obj.Message = "Record Successfully Updated";
                    obj.Data = temp;
                    return obj;
                }
                else
                { 
                    db.Entry(peoples).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PeoplesExists(id))
                        {
                            obj.Status = false;
                            obj.Message = "404 Record Not Found.";
                            return obj;
                        }
                        else
                        {
                            obj.Status = false;
                            obj.Message = "There was an error.Try again later.";
                            return obj;
                        }
                    }

                    obj.Status = true;
                    obj.Message = "Record Successfully Updated";
                    obj.Data = peoples;
                    return obj;
                }
            }
        }

        // POST: api/Peoples
        [ResponseType(typeof(Peoples))]
        public async Task<ResponseObject> PostPeoples(Peoples peoples)
        {
            ResponseObject obj = new ResponseObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    obj.Status = false;
                    obj.Message = "Model State is not valid.";
                    return obj;
                }

                Peoples check = await db.Peoples.FirstOrDefaultAsync(x => x.Email.Equals(peoples.Email));

                if (check != null)
                {
                    obj.Status = false;
                    obj.Message = "Email Already Exists.";
                    return obj;
                }
                else
                {
                    db.Peoples.Add(peoples);
                    await db.SaveChangesAsync();

                    obj.Status = true;
                    obj.Message = "201 Created.";
                    obj.Data = peoples;
                    return obj;
                }
            }
            catch (Exception e)
            {
                obj.Status = false;
                obj.Message = e.InnerException.ToString();
                return obj;
            }
        }

        // DELETE: api/Peoples/5
        [ResponseType(typeof(Peoples))]
        public async Task<Response> DeletePeoples(int id)
        {
            Response obj = new Response();
            Peoples people = await db.Peoples.FirstOrDefaultAsync(x => x.ID == id && x.Type != "Admin");
            if (people == null)
            {
                obj.Status = false;
                obj.Message = "404 Not Found";
                return obj;
            }

            var users = db.AgendaUsers.Where(x => x.User == id).ToList();
            db.AgendaUsers.RemoveRange(users);
            await db.SaveChangesAsync();

            var notes = db.Notes.Where(x => x.User == id).ToList();

            foreach (var details in notes)
            {
                NotesRequests temp = await db.NotesRequests.FirstOrDefaultAsync(x => x.NotesID == details.ID);
                db.NotesRequests.Remove(temp);
                await db.SaveChangesAsync();
            }
            db.Notes.RemoveRange(notes);
            await db.SaveChangesAsync();

            db.Peoples.Remove(people);
            await db.SaveChangesAsync();

            obj.Status = true;
            obj.Message = "Delete Successfull";
            return obj;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PeoplesExists(int id)
        {
            return db.Peoples.Count(e => e.ID == id) > 0;
        }  
    }
}