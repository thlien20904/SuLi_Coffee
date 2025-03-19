using System.Web.Mvc;

namespace YourNamespace.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                filterContext.Result = RedirectToAction("Login", "Login", new { area = "" });
            }
            base.OnActionExecuting(filterContext);
        }

        public ActionResult HomeAdmin()
        {
            return View();
        }
    }
}
