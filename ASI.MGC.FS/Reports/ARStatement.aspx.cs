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
    public partial class ARStatement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                var startDate = Convert.ToDateTime("01/01/2007");
                var endDate = Convert.ToDateTime("12/31/2007");
                DataTable dtArStatement = uMethods.ConvertTo(repo.RptArStatement(startDate, endDate));

                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\ARStatement.rdlc";
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("STARTDATE", startDate.ToShortDateString()));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("ENDDATE", endDate.ToShortDateString()));
                var rds = new ReportDataSource("DS_ARStatement", dtArStatement);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.DataBind();
                ReportViewer1.LocalReport.Refresh();
                Response.Clear();
                byte[] bytes = ReportViewer1.LocalReport.Render("PDF");
                var fileNamewithType = "inline;filename=ArStatement.pdf";
                Response.AddHeader("Content-Disposition", fileNamewithType);
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.End();
            }
        }
    }
}