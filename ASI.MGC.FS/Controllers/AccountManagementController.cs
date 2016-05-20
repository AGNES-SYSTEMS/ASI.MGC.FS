using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using SimpleCrypto;

namespace ASI.MGC.FS.Controllers
{
    public class AccountManagementController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        public AccountManagementController()
        {
            _unitOfWork = new UnitOfWork();
        }

        // GET: AccountManagement
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(UserLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (IsValid(model.Email, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email/ Password.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Registration(UserRegistartionViewModal model)
        {
            bool success = false;
            try
            {
                if (ModelState.IsValid)
                {
                    var crypto = new PBKDF2();
                    var encrypPassword = crypto.Compute(model.Password);
                    var mesUser = _unitOfWork.Repository<MESUser>().Create();
                    mesUser.UserID = Guid.NewGuid();
                    mesUser.Email = model.Email;
                    mesUser.Password = encrypPassword;
                    mesUser.PasswordSalt = crypto.Salt;
                    mesUser.UserName = model.UserName;
                    mesUser.CreatedDate = DateTime.Now;
                    mesUser.isActive = true;

                    _unitOfWork.Repository<MESUser>().Insert(mesUser);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        private bool IsValid(string email, string password)
        {
            var crypto = new PBKDF2();
            bool isValid = false;

            var user = (from mesUsers in _unitOfWork.Repository<MESUser>().Query().Get()
                        where mesUsers.Email.Equals(email)
                        select mesUsers).FirstOrDefault();
            if (user != null)
            {
                if (user.Password == crypto.Compute(password, user.PasswordSalt))
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn", "AccountManagement");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangePassword(ChangePassword model)
        {
            bool success = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (IsValid(model.Email, model.CurrentPassword))
                    {
                        var user = (from mesUsers in _unitOfWork.Repository<MESUser>().Query().Get()
                                    where mesUsers.Email.Equals(model.Email)
                                    select mesUsers).FirstOrDefault();
                        if (user != null)
                        {
                            var crypto = new PBKDF2();
                            var encrypPassword = crypto.Compute(model.Password);
                            user.Password = encrypPassword;
                            user.PasswordSalt = crypto.Salt;
                            _unitOfWork.Repository<MESUser>().Update(user);
                            _unitOfWork.Save();
                            success = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
    }
}