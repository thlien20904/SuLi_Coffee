using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class RegisterController : Controller
    {
        private WebAppDBEntities2 db = new WebAppDBEntities2();

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string Username, string Email, string Password)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Username = Username,
                    Email = Email,
                    PasswordHash = Password,
                    Role = "User"
                };

                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();

                    // Set thông báo thành công
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";

                    // Redirect về Login
                    return RedirectToAction("Login", "Login");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Console.WriteLine($"Entity \"{eve.Entry.Entity.GetType().Name}\" has the following validation errors:");
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                        }
                    }
                    throw;
                }
            }

            return View();
        }
    }
    }
