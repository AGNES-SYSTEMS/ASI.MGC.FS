using System;
using System.Data;
using System.Web.UI;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.Model.HelperClasses;
using Microsoft.Reporting.WebForms;

namespace ASI.MGC.FS.Reports
{
    public partial class CashPaymentVP : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportViewer1.KeepSessionAlive = true;
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                const string voucherType = "CP";
                var voucherCode = Request.QueryString["docNo"];
                repo.Sp_GetVoucherDetails(voucherType, voucherCode);
                DataTable dtCashPayment = uMethods.ConvertTo(repo.RptCashPayment(voucherType, voucherCode));
                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\CashpaymentVoucher.rdlc";
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("VTYPE", voucherType));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("VCODE", voucherCode));
                var rds = new ReportDataSource("DS_CashPaymentVoucher", dtCashPayment);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.DataBind();
                ReportViewer1.LocalReport.Refresh();
                //Response.Clear();
                //byte[] bytes = ReportViewer1.LocalReport.Render("PDF");
                //var fileNamewithType = "inline;filename=" + voucherCode + ".pdf";
                //Response.AddHeader("Content-Disposition", fileNamewithType);
                //Response.ContentType = "application/pdf";
                //Response.BinaryWrite(bytes);
                //Response.End();
            }
        }
    }
}