using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.Model.HelperClasses;
using Microsoft.Reporting.WebForms;

namespace ASI.MGC.FS.Reports
{
    public partial class SalesInvoice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                var invType = "INV";
                var invNo = "INV/1001/2015";
                DataTable dtSalesInvoice = uMethods.ConvertTo(repo.RptSalesInvoice(invNo, invType));
                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\SalesInvoice.rdlc";
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("INVNO", invType));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("INVTYPE", invNo));
                var rds = new ReportDataSource("DS_SalesInvoice", dtSalesInvoice);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.DataBind();
                ReportViewer1.LocalReport.Refresh();
                Response.Clear();
                byte[] bytes = ReportViewer1.LocalReport.Render("PDF");
                var fileNamewithType = "inline;filename=" + invNo + ".pdf";
                Response.AddHeader("Content-Disposition", fileNamewithType);
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.End();
            }
        }
    }
}