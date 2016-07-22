using System;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.Controllers
{
    public class AllocationMasterController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

        public AllocationMasterController()
        {
            _unitOfWork = new UnitOfWork();
        }

        // GET: AllocationMaster
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllocationMasterList(string sidx, string sord, int page, int rows)
        {
            var allocationMasterList = (from allocationMaster in _unitOfWork.Repository<ALLOCATIONMASTER>().Query().Get()
                                        select allocationMaster).Select(a => new { a.ALCODE_ALD, a.ALDESCRIPTION });
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = allocationMasterList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                allocationMasterList = allocationMasterList.OrderByDescending(a => a.ALCODE_ALD);
                allocationMasterList = allocationMasterList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                allocationMasterList = allocationMasterList.OrderBy(a => a.ALCODE_ALD);
                allocationMasterList = allocationMasterList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = allocationMasterList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountDetailsList(string sidx, string sord, int page, int rows, string accountType, string searchById, string searchByName)
        {
            switch (accountType)
            {
                case "AP":
                    var allocationMasterApList = (from allocationMaster in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                                  where allocationMaster.TYPE_ARM.Equals("AP")
                                                  select allocationMaster).Select(a => new { AccountCode = a.ARCODE_ARM, AccountDetail = a.DESCRIPTION_ARM });

                    if (!string.IsNullOrEmpty(searchById))
                    {
                        allocationMasterApList = allocationMasterApList.Where(a => a.AccountCode.Contains(searchById));
                    }
                    if (!string.IsNullOrEmpty(searchByName))
                    {
                        allocationMasterApList = allocationMasterApList.Where(a => a.AccountDetail.Contains(searchByName));
                    }
                    int pageApIndex = Convert.ToInt32(page) - 1;
                    int pageApSize = rows;
                    int totalApRecords = allocationMasterApList.Count();
                    int totalApPages = (int)Math.Ceiling(totalApRecords / (float)pageApSize);
                    if (sord.ToUpper() == "DESC")
                    {
                        allocationMasterApList = allocationMasterApList.OrderByDescending(a => a.AccountCode);
                        allocationMasterApList = allocationMasterApList.Skip(pageApIndex * pageApSize).Take(pageApSize);
                    }
                    else
                    {
                        allocationMasterApList = allocationMasterApList.OrderBy(a => a.AccountCode);
                        allocationMasterApList = allocationMasterApList.Skip(pageApIndex * pageApSize).Take(pageApSize);
                    }
                    var jsonApData = new
                    {
                        total = totalApPages,
                        page,
                        records = totalApRecords,
                        rows = allocationMasterApList
                    };
                    return Json(jsonApData, JsonRequestBehavior.AllowGet);
                case "AR":
                    var allocationMasterArList = (from allocationMaster in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                                  where allocationMaster.TYPE_ARM.Equals("AR")
                                                  select allocationMaster).Select(a => new { AccountCode = a.ARCODE_ARM, AccountDetail = a.DESCRIPTION_ARM });

                    if (!string.IsNullOrEmpty(searchById))
                    {
                        allocationMasterArList = allocationMasterArList.Where(a => a.AccountCode.Contains(searchById));
                    }
                    if (!string.IsNullOrEmpty(searchByName))
                    {
                        allocationMasterArList = allocationMasterArList.Where(a => a.AccountDetail.Contains(searchByName));
                    }
                    int pageArIndex = Convert.ToInt32(page) - 1;
                    int pageArSize = rows;
                    int totalArRecords = allocationMasterArList.Count();
                    int totalArPages = (int)Math.Ceiling(totalArRecords / (float)pageArSize);
                    if (sord.ToUpper() == "DESC")
                    {
                        allocationMasterArList = allocationMasterArList.OrderByDescending(a => a.AccountCode);
                        allocationMasterArList = allocationMasterArList.Skip(pageArIndex * pageArSize).Take(pageArSize);
                    }
                    else
                    {
                        allocationMasterArList = allocationMasterArList.OrderBy(a => a.AccountCode);
                        allocationMasterArList = allocationMasterArList.Skip(pageArIndex * pageArSize).Take(pageArSize);
                    }
                    var jsonArData = new
                    {
                        total = totalArPages,
                        page,
                        records = totalArRecords,
                        rows = allocationMasterArList

                    };
                    return Json(jsonArData, JsonRequestBehavior.AllowGet);
                case "GL":
                    var allocationMasterGlList = (from allocationMaster in _unitOfWork.Repository<GLMASTER>().Query().Get()
                                                  select allocationMaster).Select(a => new { AccountCode = a.GLCODE_LM, AccountDetail = a.GLDESCRIPTION_LM });
                    if (!string.IsNullOrEmpty(searchById))
                    {
                        allocationMasterGlList = allocationMasterGlList.Where(a => a.AccountCode.Contains(searchById));
                    }
                    if (!string.IsNullOrEmpty(searchByName))
                    {
                        allocationMasterGlList = allocationMasterGlList.Where(a => a.AccountDetail.Contains(searchByName));
                    }
                    int pageGlIndex = Convert.ToInt32(page) - 1;
                    int pageGlSize = rows;
                    int totalGlRecords = allocationMasterGlList.Count();
                    int totalGlPages = (int)Math.Ceiling(totalGlRecords / (float)pageGlSize);
                    if (sord.ToUpper() == "DESC")
                    {
                        allocationMasterGlList = allocationMasterGlList.OrderByDescending(a => a.AccountCode);
                        allocationMasterGlList = allocationMasterGlList.Skip(pageGlIndex * pageGlSize).Take(pageGlSize);
                    }
                    else
                    {
                        allocationMasterGlList = allocationMasterGlList.OrderBy(a => a.AccountCode);
                        allocationMasterGlList = allocationMasterGlList.Skip(pageGlIndex * pageGlSize).Take(pageGlSize);
                    }
                    var jsonGlData = new
                    {
                        total = totalGlPages,
                        page,
                        records = totalGlRecords,
                        rows = allocationMasterGlList

                    };
                    return Json(jsonGlData, JsonRequestBehavior.AllowGet);
                case "BA":
                    var allocationMasterList = (from allocationMaster in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                                                select allocationMaster).Select(a => new { AccountCode = a.BANKCODE_BM, AccountDetail = a.BANKNAME_BM });
                    if (!string.IsNullOrEmpty(searchById))
                    {
                        allocationMasterList = allocationMasterList.Where(a => a.AccountCode.Contains(searchById));
                    }
                    if (!string.IsNullOrEmpty(searchByName))
                    {
                        allocationMasterList = allocationMasterList.Where(a => a.AccountDetail.Contains(searchByName));
                    }
                    int pageIndex = Convert.ToInt32(page) - 1;
                    int pageSize = rows;
                    int totalRecords = allocationMasterList.Count();
                    int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
                    if (sord.ToUpper() == "DESC")
                    {
                        allocationMasterList = allocationMasterList.OrderByDescending(a => a.AccountCode);
                        allocationMasterList = allocationMasterList.Skip(pageIndex * pageSize).Take(pageSize);
                    }
                    else
                    {
                        allocationMasterList = allocationMasterList.OrderBy(a => a.AccountCode);
                        allocationMasterList = allocationMasterList.Skip(pageIndex * pageSize).Take(pageSize);
                    }
                    var jsonData = new
                    {
                        total = totalPages,
                        page,
                        records = totalRecords,
                        rows = allocationMasterList

                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }
}