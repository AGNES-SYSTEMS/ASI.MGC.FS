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
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
namespace ASI.MGC.FS.Controllers
{
    public class AccountManagementController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        public AccountManagementController()
        {
            _unitOfWork = new UnitOfWork();
        }
        #region get client mac
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
        #endregion

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
                    saveDetails(model);
                    ModelState.AddModelError("", "Invalid Email/ Password.");
                }
            }
            return View(model);
        }

        private string GetClientMachineMacAddress()
        {
            string macAddress = string.Empty;
            macAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            try
            {
                string userip = Request.UserHostAddress;
                string strClientIP = Request.UserHostAddress.ToString().Trim();
                Int32 ldest = inet_addr(strClientIP);
                Int32 lhost = inet_addr("");
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                string mac_src = macinfo.ToString("X");
                if (mac_src == "0")
                {
                    //if (userip == "127.0.0.1")
                    //    Response.Write("visited Localhost!");
                    //else
                    //    Response.Write("the IP from" + userip + "" + "<br>");
                    //return;
                }

                while (mac_src.Length < 12)
                {
                    mac_src = mac_src.Insert(0, "0");
                }

                string mac_dest = "";

                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i % 2))
                    {
                        if (i == 10)
                        {
                            mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                        else
                        {
                            mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                    }
                }
                macAddress = mac_dest;
                //Response.Write("welcome" + userip + "<br>" + ",the mac address is" + mac_dest + "."

                // + "<br>");
            }
            catch (Exception err)
            {
                Response.Write(err.Message);
            }

            return macAddress;
        }
        private void saveDetails(UserLoginViewModel model)
        {
            HttpContext httpContext = System.Web.HttpContext.Current;
            HttpRequest request = httpContext.Request;
            var unAuthenticatedRequest = _unitOfWork.Repository<MESUnAuthenticatedUsersAccess>().Create();
            unAuthenticatedRequest.Browser = Convert.ToString(request.Browser.Browser);
            unAuthenticatedRequest.UserIdentityName = request.LogonUserIdentity != null
                ? Convert.ToString(request.LogonUserIdentity.Name)
                : null;
            unAuthenticatedRequest.UserHostAddress = Convert.ToString(request.UserHostAddress);
            unAuthenticatedRequest.UserHostName = Convert.ToString(request.UserHostName);
            unAuthenticatedRequest.MacAddress = GetClientMachineMacAddress();
            unAuthenticatedRequest.IsMobileDevice = Convert.ToBoolean(request.Browser.IsMobileDevice);
            unAuthenticatedRequest.MobileDeviceManufacturer =
                Convert.ToString(request.Browser.MobileDeviceManufacturer);
            unAuthenticatedRequest.MobileDeviceModal = Convert.ToString(request.Browser.MobileDeviceModel);
            unAuthenticatedRequest.Email = model.Email;
            unAuthenticatedRequest.ExistedUser = false;
            _unitOfWork.Repository<MESUnAuthenticatedUsersAccess>().Insert(unAuthenticatedRequest);
            _unitOfWork.Save();
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
                        select mesUsers).SingleOrDefault();
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