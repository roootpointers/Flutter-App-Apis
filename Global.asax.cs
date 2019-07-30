using System; 
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using System.Web.Http;
using System.Web.Routing;
using System.Web.Security;

namespace Project
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes); 
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            try
            { 
                var authCookie1 = HttpContext.Current.Request.Cookies["AdminEatSleepUser1234hytusksdbsdfasdjasdidasdijnasd"];
                if (authCookie1 != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie1.Value);
                    if (authTicket != null && !authTicket.Expired)
                    {
                        var roles = authTicket.UserData.Split(',');
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                    }
                }
            }
            catch (Exception) { }
        }
        protected void Application_BeginRequest()
        {
            try
            {
                if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
                {
                    Response.Flush();
                }
            }
            catch (Exception) { }
        }
    }
}
