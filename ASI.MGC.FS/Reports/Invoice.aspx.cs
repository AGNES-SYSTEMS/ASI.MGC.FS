using System;
using System.Data;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.Model.HelperClasses;
using Microsoft.Reporting.WebForms;

namespace ASI.MGC.FS.Reports
{
    public partial class Invoice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                var invType = "INV";
                var invNo = Request.QueryString["invNo"];
                DataTable dtInvoice = uMethods.ConvertTo(repo.RptInvoice(invNo, invType));

                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\Invoice.rdlc";
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("INV_NO", invType));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("INVTYPE", invNo));
                var rds = new ReportDataSource("DS_Invoice", dtInvoice);
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