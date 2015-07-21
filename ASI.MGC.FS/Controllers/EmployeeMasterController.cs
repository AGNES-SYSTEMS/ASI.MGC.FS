using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASI.MGC.FS.Controllers
{
    public class EmployeeMasterController : Controller
    {
        IUnitOfWork _unitOfWork;

        public EmployeeMasterController()
        {
            _unitOfWork = new UnitOfWork();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmployeeList()
        {
            return View();
        }

        public JsonResult getEmployeeIDs(string term)
        {
            IList<string> lstEmpCodes = (from empList in _unitOfWork.Repository<EMPLOYEEMASTER>().Query().Get()
                                         where empList.EMPCODE_EM.Contains(term)
                                         select empList).Distinct().Select(x => x.EMPCODE_EM).ToList();
            return Json(lstEmpCodes, JsonRequestBehavior.AllowGet);
        }
    }
}