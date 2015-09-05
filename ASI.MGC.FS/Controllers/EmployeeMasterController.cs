using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.Controllers
{
    public class EmployeeMasterController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

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

        public JsonResult GetEmployeeIDs(string term)
        {
            IList<string> lstEmpCodes = (from empList in _unitOfWork.Repository<EMPLOYEEMASTER>().Query().Get()
                                         where empList.EMPCODE_EM.Contains(term)
                                         select empList).Distinct().Select(x => x.EMPCODE_EM).ToList();
            return Json(lstEmpCodes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeNames(string term)
        {
            IList<string> lstEmpCodes = (from empList in _unitOfWork.Repository<EMPLOYEEMASTER>().Query().Get()
                                         where empList.EMPCODE_EM.Contains(term)
                                         select empList).Distinct().Select(x => x.EMPFNAME_EM).ToList();
            return Json(lstEmpCodes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeDetails(string empCode, string empName)
        {
            EMPLOYEEMASTER objEmployee = null;
            if (!string.IsNullOrEmpty(empCode) && !string.IsNullOrWhiteSpace(empCode))
            {
                objEmployee = (from custList in _unitOfWork.Repository<EMPLOYEEMASTER>().Query().Get()
                               where custList.EMPCODE_EM.Equals(empCode)
                               select custList).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(empName) && !string.IsNullOrWhiteSpace(empName))
            {
                objEmployee = (from custList in _unitOfWork.Repository<EMPLOYEEMASTER>().Query().Get()
                               where custList.EMPFNAME_EM.Equals(empName)
                               select custList).FirstOrDefault();
            }
            return Json(objEmployee, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeDetailsList(string sidx, string sord, int page, int rows, string empCode, string empName)
        {
            var empList = (from employees in _unitOfWork.Repository<EMPLOYEEMASTER>().Query().Get()
                           select employees).Select(a => new { a.EMPCODE_EM, a.EMPFNAME_EM});
            if (!string.IsNullOrEmpty(empCode))
            {
                empList = (from employees in _unitOfWork.Repository<EMPLOYEEMASTER>().Query().Get()
                           where employees.EMPCODE_EM.Contains(empCode)
                           select employees).Select(a => new { a.EMPCODE_EM, a.EMPFNAME_EM});
            }
            if (!string.IsNullOrEmpty(empName))
            {
                empList = (from employees in _unitOfWork.Repository<EMPLOYEEMASTER>().Query().Get()
                           where employees.EMPFNAME_EM.Contains(empName)
                           select employees).Select(a => new { a.EMPCODE_EM, a.EMPFNAME_EM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = empList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                empList = empList.OrderByDescending(a => a.EMPCODE_EM);
                empList = empList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                empList = empList.OrderBy(a => a.EMPCODE_EM);
                empList = empList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = empList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}