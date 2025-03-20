using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        // GET: Admin/Users
        public ActionResult Users()
        {
            return View();
        }
    }
}