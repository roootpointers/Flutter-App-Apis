using Project.Models.Database; 
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq; 
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Project.Controllers
{
    public class AuthController : ApiController
    {
        private AdminContext db = new AdminContext();

        [ResponseType(typeof(void))]
        public async Task<ResponseObject> Login(Login temp)
        {
            ResponseObject response = new ResponseObject();
            if (!ModelState.IsValid)
            {
                response.Status = false;
                response.Message = "Model state is not valid.";
                return response; 
            }

            Peoples people = await db.Peoples.FirstOrDefaultAsync(x => x.Email.Equals(temp.Email.Trim()) && x.Password.Equals(temp.Password) && x.Type.Equals(temp.Type));
            if (people == null)
            {
                response.Status = false;
                response.Message = "Username or Password is not right.";
                return response; 
            }
            else
            {
                response.Status = true;
                response.Message = "Login Successfull.";
                response.Data = await db.Peoples.Select(x => new { x.ID, x.Image, x.FullName, x.Email, x.Type }).FirstOrDefaultAsync(x=>x.ID == people.ID);
                return response; 
            } 
        }

        [ResponseType(typeof(void))]
        public async Task<Response> ChangePassword(int id, ChangePassword temp)
        {
            Response obj = new Response();
            if (!ModelState.IsValid)
            {
                obj.Status = false;
                obj.Message = "Model state is not valid.";
                return obj;
            }

            Peoples people = await db.Peoples.FirstOrDefaultAsync(x=>x.ID == id);
            if (id != temp.ID)
            {
                obj.Status = false;
                obj.Message = "404 Record Not Found.";
                return obj; 
            }
            else if(temp.OldPassword.Equals(people.Password))
            {
                people.Password = temp.NewPassword;

                db.Entry(people).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                    obj.Status = true;
                    obj.Message = "Password Successfully Changed.";
                    return obj;
                }
                catch (DbUpdateConcurrencyException)
                {
                    obj.Status = false;
                    obj.Message = "There was an error.Try again later.";
                    return obj; 
                }
            }
            else
            {
                obj.Status = false;
                obj.Message = "Make sure that the old password is right.";
                return obj; 
            }  
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        } 
    }
}
