using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.ExtendedAPI
{
    public class MesAuthorize : AuthorizeAttribute
    {
        private readonly Guid[] _allowedroles;
        private bool _isUserExists;
        private bool _isUserNotInRole;
        private string _userIdentity;
        private Guid[] _matchedRoles;
        private string _clientMachineMac;
        readonly IUnitOfWork _unitOfWork;
        public MesAuthorize(params string[] roles)
        {
            _unitOfWork = new UnitOfWork();
            _isUserExists = false;
            _isUserNotInRole = false;
            _userIdentity = string.Empty;
            _clientMachineMac = string.Empty;
            _matchedRoles = null;
            _allowedroles = (from mesRoles in _unitOfWork.Repository<MESRole>().Query().Get()
                             where roles.Contains(mesRoles.RoleName)
                             select mesRoles.RoleID).ToArray();
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            _clientMachineMac = GetClientMachineMacAddress();
            if (!string.IsNullOrEmpty(httpContext.User.Identity.Name))
            {
                _userIdentity = httpContext.User.Identity.Name;
                var requestedUser = (from mesUsers in _unitOfWork.Repository<MESUser>().Query().Get()
                                     where mesUsers.Email.Equals(_userIdentity)
                                     select mesUsers).SingleOrDefault();
                if (requestedUser != null && IsClientMachineAuthenticated(_clientMachineMac, requestedUser))
                {
                    _isUserExists = true;

                    _matchedRoles = (from userRoles in _unitOfWork.Repository<MESUserRole>().Query().Get()
                                     where _allowedroles.Contains(userRoles.RoleID) &&
                                         userRoles.UserID.Equals(requestedUser.UserID)
                                     select userRoles.RoleID).ToArray();
                    if (_matchedRoles != null && _matchedRoles.Count() > 0)
                    {
                        authorize = true;
                        SaveUserLoginDetails(httpContext, requestedUser);
                    }
                    else
                    {
                        _isUserNotInRole = true;
                    }
                }
            }
            return authorize;
        }
        private void SaveUserLoginDetails(HttpContextBase httpContext, MESUser requestedUser)
        {
            HttpRequestBase request = httpContext.Request;
            var userLoginDetails = _unitOfWork.Repository<MESUserLoginDetail>().Create();
            userLoginDetails.UserID = requestedUser.UserID;
            userLoginDetails.LoginDate = DateTime.Now.Date;
            userLoginDetails.LoginTime = DateTime.Now.TimeOfDay;
            userLoginDetails.LoginDuration = "0";
            userLoginDetails.Browser = Convert.ToString(request.Browser.Browser);
            userLoginDetails.UserIdentityName = request.LogonUserIdentity != null ? Convert.ToString(request.LogonUserIdentity.Name) : null;
            userLoginDetails.UserHostAddress = Convert.ToString(request.UserHostAddress);
            userLoginDetails.UserHostName = Convert.ToString(request.UserHostName);
            userLoginDetails.MacAddress = Convert.ToString(_clientMachineMac);
            userLoginDetails.IsMobileDevice = Convert.ToBoolean(request.Browser.IsMobileDevice);
            userLoginDetails.MobileDeviceManufacturer =
                Convert.ToString(request.Browser.MobileDeviceManufacturer);
            userLoginDetails.MobileDeviceModal = Convert.ToString(request.Browser.MobileDeviceModel);
            userLoginDetails.IsActive = true;
            _unitOfWork.Repository<MESUserLoginDetail>().Insert(userLoginDetails);
            _unitOfWork.Save();
        }
        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    var userIdentity = filterContext.HttpContext.User.Identity;
        //    base.OnAuthorization(filterContext);
        //}
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
            if (!_isUserNotInRole)
            {
                HttpRequestBase request = filterContext.HttpContext.Request;
                var unAuthenticatedRequest = _unitOfWork.Repository<MESUnAuthenticatedUsersAccess>().Create();
                unAuthenticatedRequest.Browser = Convert.ToString(request.Browser.Browser);
                unAuthenticatedRequest.UserIdentityName = request.LogonUserIdentity != null
                    ? Convert.ToString(request.LogonUserIdentity.Name)
                    : null;
                unAuthenticatedRequest.UserHostAddress = Convert.ToString(request.UserHostAddress);
                unAuthenticatedRequest.UserHostName = Convert.ToString(request.UserHostName);
                unAuthenticatedRequest.MacAddress = Convert.ToString(_clientMachineMac);
                unAuthenticatedRequest.IsMobileDevice = Convert.ToBoolean(request.Browser.IsMobileDevice);
                unAuthenticatedRequest.MobileDeviceManufacturer =
                    Convert.ToString(request.Browser.MobileDeviceManufacturer);
                unAuthenticatedRequest.MobileDeviceModal = Convert.ToString(request.Browser.MobileDeviceModel);
                unAuthenticatedRequest.Email = _userIdentity;
                unAuthenticatedRequest.ExistedUser = _isUserExists;
                _unitOfWork.Repository<MESUnAuthenticatedUsersAccess>().Insert(unAuthenticatedRequest);
                _unitOfWork.Save();
            }
        }
        private bool IsClientMachineAuthenticated(string clientMachineMac, MESUser requestedUser)
        {
            if (!requestedUser.isSuperUser)
            {
                var clientMachine = (from mesMachines in _unitOfWork.Repository<MESMachine>().Query().Get()
                                     where mesMachines.MacAddress.Equals(clientMachineMac)
                                     select mesMachines).SingleOrDefault();
                if (clientMachine != null)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }
        private string GetClientMachineMacAddress()
        {
            return (from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
        }
    }
}