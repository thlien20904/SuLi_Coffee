using System;
using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class NguoiDungController : Controller
    {
        private WebAppDBEntities2 db = new WebAppDBEntities2();

        // Hiển thị danh sách Users
        public ActionResult NguoiDung()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        // GET: Hiển thị form thêm User
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.Roles = new SelectList(new[] { "User", "Admin" });
            return View();
        }


        // POST: Xử lý thêm User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(User user)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng Username
                if (db.Users.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Username đã tồn tại!");
                    return View(user);
                }

                // Kiểm tra trùng Email
                if (db.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại!");
                    return View(user);
                }

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("NguoiDung");
            }
            return View(user);
        }

        // GET: Hiển thị form sửa User
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Xử lý sửa User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                // Check Username trùng nhưng phải loại trừ user hiện tại
                if (db.Users.Any(u => u.Username == user.Username && u.Id != user.Id))
                {
                    ModelState.AddModelError("Username", "Username đã tồn tại!");
                    return View(user);
                }

                // Check Email trùng nhưng phải loại trừ user hiện tại
                if (db.Users.Any(u => u.Email == user.Email && u.Id != user.Id))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại!");
                    return View(user);
                }

                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("NguoiDung");
            }
            return View(user);
        }

        // GET: Xác nhận xóa User
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Xử lý xóa User
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
            return RedirectToAction("NguoiDung"); // sửa ở đây
        }
    }
}
