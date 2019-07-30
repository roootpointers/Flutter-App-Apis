using System;
using System.Collections.Generic;
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
    public class AgendasController : ApiController
    {
        private AdminContext db = new AdminContext();

        // Get All Agendas: Admin  
        public ResponseList GetAgendas(int? page = 1, int? per_page = 10, string search = "")
        {
            ResponseList li = new ResponseList();
            try
            {
                List<Agendas> data = new List<Agendas>();
                if (page.Value <= 0)
                    page = 1;
                int pg = (page.Value - 1);
                var skip = pg * per_page.Value;

                li.Status = true;
                li.Message = "List of Agendas";
                if (String.IsNullOrWhiteSpace(search))
                    data = db.Agendas.OrderBy(x => x.DateTime).Skip(skip).Take(per_page.Value).ToList();
                else
                    data = db.Agendas.Where(x => x.Title.Contains(search)).OrderBy(x => x.DateTime).Skip(skip).Take(per_page.Value).ToList();

                List<AgendasClass> list1 = new List<AgendasClass>();
                List<AgendasClass> list2 = new List<AgendasClass>();
                List<AgendasClass> list3 = new List<AgendasClass>();
                foreach (var details in data)
                {
                    AgendasClass temp = new AgendasClass
                    {
                        ID = details.ID,
                        Title = details.Title,
                        DateTime = details.DateTime.ToString("dddd dd MMMM, yyyy") + " at " + details.DateTime.ToShortTimeString(),
                        Description = details.Description,
                        Users = db.AgendaUsers.Where(x => x.AgendasID == details.ID).Count()
                    };
                    if (details.DateTime.Day == DateTime.Today.Day && details.DateTime.Year == DateTime.Today.Year && details.DateTime.Month == DateTime.Today.Month)
                    {
                        temp.Status = "Today";
                        temp.Color = "0xff38ad0d";
                        list1.Add(temp);
                    }
                    else if (details.DateTime > DateTime.Today)
                    {
                        temp.Status = "Upcoming";
                        temp.Color = "0xfff47d08";
                        list2.Add(temp);
                    }
                    else
                    {
                        temp.Status = "Hosted";
                        temp.Color = "0xffb7b7b7";
                        list3.Add(temp);
                    }
                }
                li.Data = list1.Concat(list2).Concat(list3).ToList();
                return li;
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // Get Single Agenda By Id: Admin
        [ResponseType(typeof(Agendas))]
        public async Task<SingleAgenda> GetAgendaById(int id)
        {
            SingleAgenda li = new SingleAgenda();
            try
            {
                Agendas agenda = await db.Agendas.FindAsync(id);
                if (agenda == null)
                {
                    li.Status = false;
                    li.Message = "404 Not Found";
                    return li;
                }
                else
                {
                    li.Status = true;
                    li.Message = "Record you asked.";
                    AgendasClass temp = new AgendasClass
                    {
                        ID = agenda.ID,
                        Title = agenda.Title,
                        DateTime = agenda.DateTime.ToString("dddd dd MMMM, yyyy") + " at " + agenda.DateTime.ToShortTimeString(),
                        Description = agenda.Description,
                        Users = db.AgendaUsers.Where(x => x.AgendasID == agenda.ID).Count()
                    };
                    if (agenda.DateTime == DateTime.Now)
                    {
                        temp.Status = "Today";
                        temp.Color = "0xff38ad0d";
                    }
                    else if (agenda.DateTime < DateTime.Now)
                    {
                        temp.Status = "Hosted";
                        temp.Color = "0xffb7b7b7";
                    }
                    else
                    {
                        temp.Status = "Upcoming";
                        temp.Color = "0xfff47d08";
                    }

                    li.Agenda = temp;
                    li.Note = await db.Notes.Select(x => new { x.ID, x.Title, x.Description, x.AgendasID, MemberId = x.Peoples.ID, MemberImage = x.Peoples.Image, MemberName = x.Peoples.FullName, UserType = x.Peoples.Type, NoteStatus = x.Status }).FirstOrDefaultAsync(x => x.AgendasID == agenda.ID && x.NoteStatus.Equals("Public"));
                    li.Secretary = db.Peoples.Where(x => x.Type.Equals("Secretary")).Select(x => new { x.ID, x.Image, x.FullName, x.Email }).ToList();
                    li.Members = db.AgendaUsers.Where(x => x.AgendasID == agenda.ID && x.Status == true).Take(4).Select(x => new { MemberId = x.Peoples.ID, MemberImage = x.Peoples.Image, MemberName = x.Peoples.FullName, MemberEmail = x.Peoples.Email }).ToList();
                    return li;
                }
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // Get Members In Agenda: Admin
        public ResponseList GetMembersByAgenda(int agendaId, int? page = 1, int? per_page = 10)
        {
            ResponseList li = new ResponseList();
            try
            {
                if (page.Value <= 0)
                    page = 1;
                int pg = (page.Value - 1);
                var skip = pg * per_page.Value;

                li.Status = true;
                li.Message = "List of Members in this Agenda";
                li.Data = db.AgendaUsers.Where(x => x.AgendasID == agendaId && x.Status == true).OrderBy(x => x.AgendasID).Skip(skip).Take(per_page.Value).Select(x => new { AgendaId = x.Peoples.ID, MemberImage = x.Peoples.Image, MemberName = x.Peoples.FullName, MemberEmail = x.Peoples.Email }).ToList();

                return li;
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // Get All Agendas by Member: Member
        public ResponseList GetAgendasByMember(int memberId, int? page = 1, int? per_page = 10, string search = "")
        {
            ResponseList li = new ResponseList();
            try
            {
                List<Agendas> data = new List<Agendas>();
                if (page.Value <= 0)
                    page = 1;
                int pg = (page.Value - 1);
                var skip = pg * per_page.Value;

                li.Status = true;
                li.Message = "List of Agendas";
                if (!String.IsNullOrWhiteSpace(search))
                    data = db.Agendas.OrderBy(x => x.DateTime).Skip(skip).Take(per_page.Value).ToList();
                else
                    data = db.Agendas.Where(x => x.Title.Contains(search)).OrderBy(x => x.DateTime).Skip(skip).Take(per_page.Value).ToList();

                List<AgendasClassForMember> list1 = new List<AgendasClassForMember>();
                List<AgendasClassForMember> list2 = new List<AgendasClassForMember>();
                List<AgendasClassForMember> list3 = new List<AgendasClassForMember>();
                foreach (var details in data)
                {
                    AgendasClassForMember temp = new AgendasClassForMember
                    {
                        ID = details.ID,
                        Title = details.Title,
                        DateTime = details.DateTime.ToString("dddd dd MMMM, yyyy") + " at " + details.DateTime.ToShortTimeString(),
                        Description = details.Description,
                        Users = db.AgendaUsers.Where(x => x.AgendasID == details.ID && x.Status == true).Count()
                    };

                    AgendaUsers Member = db.AgendaUsers.FirstOrDefault(x => x.AgendasID == details.ID && x.User == memberId);
                    if (Member != null)
                    {
                        if (!Member.Status)
                        {
                            temp.MemberStatus = "Not Going";
                            temp.MemberStatusColor = "0xffd32424";
                        }
                        else
                        {
                            temp.MemberStatus = "Going";
                            temp.MemberStatusColor = "0xff15bc52";
                        }
                    }
                    else
                    {
                        temp.MemberStatus = "Not Going";
                        temp.MemberStatusColor = "0xffd32424";
                    }

                    if (details.DateTime.Day == DateTime.Today.Day && details.DateTime.Year == DateTime.Today.Year && details.DateTime.Month == DateTime.Today.Month)
                    {
                        temp.AgendaStatus = "Today";
                        temp.AgendaStatusColor = "0xff38ad0d";
                        list1.Add(temp);
                    }
                    else if (details.DateTime > DateTime.Today)
                    {
                        temp.AgendaStatus = "Upcoming";
                        temp.AgendaStatusColor = "0xfff47d08";
                        list2.Add(temp);
                    }
                    else
                    {
                        temp.AgendaStatus = "Hosted";
                        temp.AgendaStatusColor = "0xffb7b7b7";
                        list3.Add(temp);
                    }
                }
                li.Data = list1.Concat(list2).Concat(list3).ToList();
                return li;
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // Get Single Agenda by Member: Member
        [ResponseType(typeof(Agendas))]
        public async Task<SingleAgenda> GetAgendaForMemberById(int agendaId, int memberId)
        {
            SingleAgenda li = new SingleAgenda();
            try
            {
                Agendas agenda = await db.Agendas.FindAsync(agendaId);
                if (agenda == null)
                {
                    li.Status = false;
                    li.Message = "404 Not Found";
                    return li;
                }
                else
                {
                    li.Status = true;
                    li.Message = "Record you asked.";
                    AgendasClassForMember temp = new AgendasClassForMember
                    {
                        ID = agenda.ID,
                        Title = agenda.Title,
                        DateTime = agenda.DateTime.ToString("dddd dd MMMM, yyyy") + " at " + agenda.DateTime.ToShortTimeString(),
                        Description = agenda.Description,
                        Users = db.AgendaUsers.Where(x => x.AgendasID == agenda.ID && x.Status == true).Count()
                    };
                    if (agenda.DateTime == DateTime.Now)
                    {
                        temp.AgendaStatus = "Today";
                        temp.AgendaStatusColor = "0xff38ad0d";
                    }
                    else if (agenda.DateTime < DateTime.Now)
                    {
                        temp.AgendaStatus = "Hosted";
                        temp.AgendaStatusColor = "0xffb7b7b7";
                    }
                    else
                    {
                        temp.AgendaStatus = "Upcoming";
                        temp.AgendaStatusColor = "0xfff47d08";
                    }
                    AgendaUsers Member = db.AgendaUsers.FirstOrDefault(x => x.AgendasID == agenda.ID && x.User == memberId);
                    if (Member == null)
                    { 
                        temp.MemberStatus = "Not Going";
                        temp.MemberStatusColor = "0xffd32424";
                    }
                    else if (Member.Status)
                    {
                        temp.MemberStatus = "Going";
                        temp.MemberStatusColor = "0xff15bc52";
                    }
                    else
                    { 
                        temp.MemberStatus = "Not Going";
                        temp.MemberStatusColor = "0xffd32424";
                    }

                    li.Agenda = temp;
                    li.Note = await db.Notes.Select(x => new { x.ID, x.Title, x.Description, x.AgendasID, MemberId = x.Peoples.ID, MemberImage = x.Peoples.Image, MemberName = x.Peoples.FullName, UserType = x.Peoples.Type, NoteStatus = x.Status }).FirstOrDefaultAsync(x => x.AgendasID == agenda.ID && x.NoteStatus.Equals("Public"));
                    li.Secretary = db.Peoples.Where(x => x.Type.Equals("Secretary")).Select(x => new { x.ID, x.Image, x.FullName, x.Email }).ToList();
                    li.Members = db.AgendaUsers.Where(x => x.AgendasID == agenda.ID && x.Status == true && x.User != memberId).Take(4).Select(x => new { MemberId = x.Peoples.ID, MemberImage = x.Peoples.Image, MemberName = x.Peoples.FullName, MemberEmail = x.Peoples.Email }).ToList();
                    return li;
                }
            }
            catch (Exception e)
            {
                li.Status = false;
                li.Message = e.InnerException.ToString();
                return li;
            }
        }

        // Update Agenda: Admin
        [ResponseType(typeof(void))]
        public async Task<ResponseObject> PutAgendas(int id, Agendas agendas)
        {
            ResponseObject obj = new ResponseObject();
            if (!ModelState.IsValid)
            {
                obj.Status = false;
                obj.Message = "Model state is not valid.";
                return obj;
            }

            if (id != agendas.ID)
            {
                obj.Status = false;
                obj.Message = "404 Record Not Found.";
                return obj;
            }

            db.Entry(agendas).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgendasExists(id))
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
            obj.Data = agendas;
            return obj;
        }

        // Post Agenda: Admin
        [ResponseType(typeof(Agendas))]
        public async Task<ResponseObject> PostAgendas(Agendas agendas)
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

                db.Agendas.Add(agendas);
                await db.SaveChangesAsync();

                var people = db.Peoples.Where(x => x.Type == "Member").Select(x => new { x.ID }).ToList();
                foreach (var details in people)
                {
                    try
                    {
                        AgendaUsers add = new AgendaUsers
                        {
                            AgendasID = agendas.ID,
                            User = details.ID,
                            Status = true
                        };

                        db.AgendaUsers.Add(add);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception) { }
                }

                temp.Status = true;
                temp.Message = "201 Created.";
                temp.Data = agendas;
                return temp;
            }
            catch (Exception e)
            {
                temp.Status = false;
                temp.Message = e.InnerException.ToString();
                return temp;
            }
        }

        // Change User Attendence: Admin
        [ResponseType(typeof(void))]
        public async Task<ResponseObject> PutUserStatus(AgendaUsers agendas)
        {
            ResponseObject obj = new ResponseObject();
            if (!ModelState.IsValid)
            {
                obj.Status = false;
                obj.Message = "Model state is not valid.";
                return obj;
            }
            AgendaUsers change = await db.AgendaUsers.FirstOrDefaultAsync(x => x.AgendasID == agendas.AgendasID && x.User == agendas.User);
            if (change == null)
            {
                try
                {
                    db.AgendaUsers.Add(agendas);
                    await db.SaveChangesAsync();
                }
                catch (Exception) { }
            }
            else
            {
                change.Status = agendas.Status; 
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    obj.Status = false;
                    obj.Message = "There was an error.Try again later.";
                    return obj;
                }
            }
            obj.Status = true;
            obj.Message = "Record Successfully Updated";
            obj.Data = change;
            return obj;
        }

        // DELETE: Admin
        [ResponseType(typeof(Agendas))]
        public async Task<Response> DeleteAgendas(int id)
        {
            Response obj = new Response();
            Agendas agendas = await db.Agendas.FindAsync(id);
            if (agendas == null)
            {
                obj.Status = false;
                obj.Message = "404 Not Found";
                return obj;
            }

            var users = db.AgendaUsers.Where(x => x.AgendasID == id).ToList();
            db.AgendaUsers.RemoveRange(users);
            await db.SaveChangesAsync();

            var notes = db.Notes.Where(x => x.AgendasID == id).ToList();

            foreach (var details in notes)
            {
                NotesRequests temp = await db.NotesRequests.FirstOrDefaultAsync(x => x.NotesID == details.ID);
                db.NotesRequests.Remove(temp);
                await db.SaveChangesAsync();
            }
            db.Notes.RemoveRange(notes);
            await db.SaveChangesAsync();

            db.Agendas.Remove(agendas);
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

        private bool AgendasExists(int id)
        {
            return db.Agendas.Count(e => e.ID == id) > 0;
        }
    }
}