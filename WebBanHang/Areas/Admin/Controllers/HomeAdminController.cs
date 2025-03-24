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
        [HttpGet]
        public ActionResult GlobalSearch(string query, string filterType)
        {
            if (string.IsNullOrEmpty(query))
            {
                TempData["Error"] = "Vui lòng nhập từ khóa tìm kiếm.";
                return RedirectToAction("HomeAdmin");
            }

            switch (filterType)
            {
                case "food":
                    return RedirectToAction("Info", "SanPhamAdmin", new { area = "Admin", search = query });
                case "invoice":
                    return RedirectToAction("HoaDon", "BaoCao", new { area = "Admin", search = query });
                case "staff":
                    return RedirectToAction("NhanVien", "NhanVien", new { area = "Admin", search = query });
                case "ingredient":
                    return RedirectToAction("KhoAdmin", "KhoAdmin", new { area = "Admin", search = query });
                default:
                    return RedirectToAction("HomeAdmin", "HomeAdmin", new { area = "Admin", search = query });
            }
        }
    }
}
