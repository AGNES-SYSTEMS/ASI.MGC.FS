using System;
using System.Data;
using System.Web.UI;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.Model.HelperClasses;
using Microsoft.Reporting.WebForms;

namespace ASI.MGC.FS.Reports
{
    public partial class StockReceipt : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                var voucherNo = Request.QueryString["voucherNo"];
                DataTable dtStockReceipt = uMethods.ConvertTo(repo.RptStockReceipt(voucherNo));
                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\StockReceipt.rdlc";
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("VOUCHERNO", voucherNo));
                var rds = new ReportDataSource("DS_StockReceipt", dtStockReceipt);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.DataBind();
                ReportViewer1.LocalReport.Refresh();
                //Response.Clear();
                //byte[] bytes = ReportViewer1.LocalReport.Render("PDF");
                //var fileNamewithType = "inline;filename=" + voucherNo + ".pdf";
                //Response.AddHeader("Content-Disposition", fileNamewithType);
                //Response.ContentType = "application/pdf";
                //Response.BinaryWrite(bytes);
                //Response.End();
            }
        }
    }
}