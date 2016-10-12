using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using SimpleCrypto;
using ASI.MGC.FS.WebCommon;
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
        [AllowAnonymous]
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
            IList<SelectListItem> userRoles = new List<SelectListItem>();
            var roles = (from mesRoles in _unitOfWork.Repository<MESRole>().Query().Get()
                         where mesRoles.isActive.Equals(true)
                         select mesRoles);
            foreach (var role in roles)
            {
                userRoles.Add(new SelectListItem { Text = role.RoleName, Value = Convert.ToString(role.RoleID) });
            }
            UserRegistartionViewModal userRegistartion = new UserRegistartionViewModal
            {
                UserRoles = userRoles
            };
            return View(userRegistartion);
        }

        [HttpPost]
        public ActionResult Registration(UserRegistartionViewModal model)
        {
            //bool success = false;
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
                    mesUser.isSuperUser = model.IsSuperUser;
                    _unitOfWork.Repository<MESUser>().Insert(mesUser);
                    _unitOfWork.Save();

                    foreach (var role in model.SelectedRoles)
                    {
                        var userRole = _unitOfWork.Repository<MESUserRole>().Create();
                        userRole.UserID = mesUser.UserID;
                        userRole.RoleID = Guid.Parse(role);
                        userRole.IsActive = true;
                        _unitOfWork.Repository<MESUserRole>().Insert(userRole);
                        _unitOfWork.Save();
                    }
                    //success = true;
                    return RedirectToAction("Registration", "AccountManagement");
                }
                else
                {
                    ModelState.AddModelError("", "One or More fields are not correct.");
                }
            }
            catch (Exception)
            {
                //success = false;
            }
            //return Json(success, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Registration", "AccountManagement");
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
            HttpContext httpContext = System.Web.HttpContext.Current;
            var userIdentity = httpContext.User.Identity.Name;
            var signingOutUser = (from mesUsers in _unitOfWork.Repository<MESUser>().Query().Get()
                                  where mesUsers.Email.Equals(userIdentity)
                                  select mesUsers).SingleOrDefault();
            var currentLoginDetails = signingOutUser != null
                ? (from mesLoginDetails in _unitOfWork.Repository<MESUserLoginDetail>().Query().Get()
                   where mesLoginDetails.UserID.Equals(signingOutUser.UserID) && mesLoginDetails.IsActive.Equals(true)
                   select mesLoginDetails).FirstOrDefault()
                : null;
            if (currentLoginDetails != null)
            {
                currentLoginDetails.IsActive = false;
                _unitOfWork.Repository<MESUserLoginDetail>().Update(currentLoginDetails);
                _unitOfWork.Save();
            }
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