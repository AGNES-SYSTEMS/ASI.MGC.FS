using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using System.Web.Security;

namespace ASI.MGC.FS.Controllers
{
    public class AccountManagementController : Controller
    {
        IUnitOfWork _unitOfWork;
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
                if (isValid(model.Email, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError("","Invalid Email/ Password.");
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
        public ActionResult Registration(UserRegistartionViewModal model)
        {
            if (ModelState.IsValid)
            {
                var crypto = new SimpleCrypto.PBKDF2();
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
                return RedirectToAction("LogIn", "AccountManagement");
            }
            else
            {
                ModelState.AddModelError("", "One or More values are incorrect.");
            }
            return View();
        }

        private bool isValid(string email, string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            bool isValid = false;

            var user = (from mesUsers in _unitOfWork.Repository<MESUser>().Query().Get()
                           where mesUsers.Email.Equals(email)
                           select mesUsers).FirstOrDefault();
            if (user != null)
            {
                if (user.Password == crypto.Compute(password,user.PasswordSalt))
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
    }
}