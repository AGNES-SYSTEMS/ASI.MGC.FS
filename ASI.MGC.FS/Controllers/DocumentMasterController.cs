using System;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class DocumentMasterController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo tzInfo;
        DateTime today;
        public DocumentMasterController()
        {
            _unitOfWork = new UnitOfWork();
            tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            today = TimeZoneInfo.ConvertTime(DateTime.Now, tzInfo);
        }
        // GET: DocumentMaster
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetByDocType(string sidx, string sord, int page, int rows, string docType)
        {
            var docList = (from documents in _unitOfWork.Repository<DOCCUMENTMASTER>().Query().Get()
                           select documents).Select(a => new { a.DOCABBREVIATION_DM, a.DESCRIPTION_DM });
            if (!string.IsNullOrEmpty(docType))
            {
                docList = (from documents in _unitOfWork.Repository<DOCCUMENTMASTER>().Query().Get()
                           where documents.DOCTYPE_DM.Equals(docType)
                           select documents).Select(a => new { a.DOCABBREVIATION_DM, a.DESCRIPTION_DM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = docList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                docList = docList.OrderByDescending(a => a.DOCABBREVIATION_DM);
                docList = docList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                docList = docList.OrderBy(a => a.DOCABBREVIATION_DM);
                docList = docList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = docList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocNo(string docType)
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            string docNo = CommonModelAccessUtility.GetDocNo(unitOfWork, docType);
            return Json(docNo, JsonRequestBehavior.AllowGet);
        }
    }
}