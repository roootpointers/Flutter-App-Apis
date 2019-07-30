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
    public class NotesController : ApiController
    {
        private AdminContext db = new AdminContext();

        // GET: api/Notes
        public IQueryable<Notes> GetNotes()
        {
            return db.Notes;
        }

        // GET: api/Notes/5
        [ResponseType(typeof(Notes))]
        public async Task<IHttpActionResult> GetNotes(int id)
        {
            Notes notes = await db.Notes.FindAsync(id);
            if (notes == null)
            {
                return NotFound();
            }

            return Ok(notes);
        }

        // Get Public Notes In Agenda: Admin, Member, Secretary
        public ResponseList GetPublicNotesByAgenda(int agendaId, int? page = 1, int? per_page = 10)
        {
            ResponseList li = new ResponseList();
            try
            {
                if (page.Value <= 0)
                    page = 1;
                int pg = (page.Value - 1);
                var skip = pg * per_page.Value;

                li.Status = true;
                li.Message = "List of Notes in this Agenda";
                li.Data = db.Notes.Where(x => x.AgendasID == agendaId && x.Status.Equals("Public")).OrderBy(x => x.AgendasID).Skip(skip).Take(per_page.Value).Select(x => new { x.ID, x.Title, x.Description, x.AgendasID, MemberId = x.Peoples.ID, MemberImage = x.Peoples.Image, MemberName = x.Peoples.FullName, UserType = x.Peoples.Type, NoteStatus = x.Status }).ToList();

                return li;
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // Get Private Notes In Agenda: Admin, Member, Secretary
        public ResponseList GetPrivateNotesByAgenda(int agendaId, int? page = 1, int? per_page = 10)
        {
            ResponseList li = new ResponseList();
            try
            {
                if (page.Value <= 0)
                    page = 1;
                int pg = (page.Value - 1);
                var skip = pg * per_page.Value;

                li.Status = true;
                li.Message = "List of Notes in this Agenda";
                li.Data = db.Notes.Where(x => x.AgendasID == agendaId && x.Status.Equals("Private")).OrderBy(x => x.AgendasID).Skip(skip).Take(per_page.Value).Select(x => new { x.ID, x.Title, x.Description, x.AgendasID, MemberId = x.Peoples.ID, MemberImage = x.Peoples.Image, MemberName = x.Peoples.FullName, UserType = x.Peoples.Type, NoteStatus = x.Status }).ToList();

                return li;
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // Get Notes In Agenda: Admin, Member, Secretary
        public ResponseList GetNotesInAgenda(int agendaId, int? page = 1, int? per_page = 10)
        {
            ResponseList li = new ResponseList();
            try
            {
                if (page.Value <= 0)
                    page = 1;
                int pg = (page.Value - 1);
                var skip = pg * per_page.Value;

                li.Status = true;
                li.Message = "List of Notes in this Agenda";
                li.Data = db.Notes.Where(x => x.AgendasID == agendaId).OrderBy(x => x.AgendasID).Skip(skip).Take(per_page.Value).Select(x => new { x.ID, x.Title, x.Description, x.AgendasID, MemberId = x.Peoples.ID, MemberImage = x.Peoples.Image, MemberName = x.Peoples.FullName, UserType = x.Peoples.Type, NoteStatus = x.Status }).ToList();

                return li;
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // GET: api/Notes/GetNotesByAgenda/1
        public ResponseList GetPrivateNotes(int agendaId, int memberId, int? page = 1, int? per_page = 10)
        {
            ResponseList li = new ResponseList();
            try
            {
                if (page.Value <= 0)
                    page = 1;
                int pg = (page.Value - 1);
                var skip = pg * per_page.Value;

                li.Status = true;
                li.Message = "List of Notes in this Agenda";
                li.Data = db.Notes.Where(x => x.AgendasID == agendaId && x.Status.Equals("Private") && x.User == memberId).OrderBy(x => x.AgendasID).Skip(skip).Take(per_page.Value).Select(x => new { x.ID, x.Title, x.Description, x.AgendasID, MemberId = x.Peoples.ID, MemberImage = x.Peoples.Image, MemberName = x.Peoples.FullName, UserType = x.Peoples.Type, NoteStatus = x.Status }).ToList();

                return li;
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // PUT: api/Notes/5
        [ResponseType(typeof(void))]
        public async Task<ResponseObject> PutNotes(int id, Notes notes)
        {
            ResponseObject obj = new ResponseObject();
            if (!ModelState.IsValid)
            {
                obj.Status = false;
                obj.Message = "Model state is not valid.";
                return obj;
            }

            if (id != notes.ID)
            {
                obj.Status = false;
                obj.Message = "404 Record Not Found.";
                return obj;
            }
            db.Entry(notes).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotesExists(id))
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
            obj.Data = notes;
            return obj;
        }

        // POST: api/Notes
        [ResponseType(typeof(Notes))]
        public async Task<ResponseObject> PostNotes(Notes notes)
        {
            ResponseObject temp = new ResponseObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    temp.Status = false;
                    temp.Message = "Model State is not valid.";
                    return temp;
                }

                Peoples people = await db.Peoples.FirstOrDefaultAsync(x => x.ID == notes.User && x.Type.Equals("Secretary"));
                if (people != null)
                    notes.Status = "Public";
                else
                    notes.Status = "Private";

                db.Notes.Add(notes);
                await db.SaveChangesAsync();

                temp.Status = true;
                temp.Message = "201 Created.";
                temp.Data = notes;
                return temp;
            }
            catch (Exception e)
            {
                temp.Status = false;
                temp.Message = e.InnerException.ToString();
                return temp;
            }
        }

        // POST: api/Notes
        [ResponseType(typeof(Notes))]
        public async Task<ResponseObject> PostApproveNotes(NotesRequests status)
        {
            ResponseObject temp = new ResponseObject();
            try
            {
                if (!ModelState.IsValid)
                {
                    temp.Status = false;
                    temp.Message = "Model State is not valid.";
                    return temp;
                }
                 
                Notes notes = await db.Notes.FirstOrDefaultAsync(x => x.ID == status.NotesID);
                if (notes != null)
                {
                    NotesRequests check = await db.NotesRequests.FirstOrDefaultAsync(x => x.NotesID == status.NotesID && x.SecretaryID == status.SecretaryID);
                    if (check == null)
                    {
                        notes.Status = status.Status;
                        db.Entry(notes).State = EntityState.Modified; 

                        db.NotesRequests.Add(status);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        notes.Status = status.Status;
                        db.Entry(notes).State = EntityState.Modified;

                        check.Status = status.Status;
                        db.Entry(check).State = EntityState.Modified;

                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    temp.Status = false;
                    temp.Message = "Note id is not valid.";
                    return temp;
                }
                temp.Status = true;
                temp.Message = "Successfully Updated.";
                temp.Data = notes;
                return temp;
            }
            catch (Exception e)
            {
                temp.Status = false;
                temp.Message = e.InnerException.ToString();
                return temp;
            }
        }

        // DELETE: api/Notes/5
        [ResponseType(typeof(Notes))]
        public async Task<IHttpActionResult> DeleteNotes(int id)
        {
            Notes notes = await db.Notes.FindAsync(id);
            if (notes == null)
            {
                return NotFound();
            }

            db.Notes.Remove(notes);
            await db.SaveChangesAsync();

            return Ok(notes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NotesExists(int id)
        {
            return db.Notes.Count(e => e.ID == id) > 0;
        }
    }
}