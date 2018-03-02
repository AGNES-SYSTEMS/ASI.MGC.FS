using System;
using System.Data;
using System.Web.UI;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.Model.HelperClasses;
using Microsoft.Reporting.WebForms;

namespace ASI.MGC.FS.Reports
{
    public partial class BalanceSheet : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportViewer1.KeepSessionAlive = true;
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                var startDate = Convert.ToDateTime(Request.QueryString["startDate"]);
                var endDate = Convert.ToDateTime(Request.QueryString["endDate"]);
                DataTable dtBalanceSheet = uMethods.ConvertTo(repo.RptBalanceSheet(startDate, endDate));

                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\BalanceSheet.rdlc";
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("STARTDATE", startDate.ToShortDateString()));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("ENDDATE", endDate.ToShortDateString()));
                var rds = new ReportDataSource("DS_BalanceSheet", dtBalanceSheet);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.DataBind();
                ReportViewer1.LocalReport.Refresh();
                Response.Clear(); if (Request.QueryString["isExportMode"] != "1")
                {
                    byte[] bytes = ReportViewer1.LocalReport.Render("PDF");
                    var fileNamewithType = "inline;filename=BalanceSheet.pdf";
                    Response.AddHeader("Content-Disposition", fileNamewithType);
                    Response.ContentType = "application/pdf";
                    Response.BinaryWrite(bytes);
                    Response.End();
                }
            }
        }
    }
}