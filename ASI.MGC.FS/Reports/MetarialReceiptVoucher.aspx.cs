using System;
using System.Web.UI;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.Model.HelperClasses;
using Microsoft.Reporting.WebForms;

namespace ASI.MGC.FS.Reports
{
    public partial class MetarialReceiptVoucher : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);
            this.ReportViewer1.LocalReport.Refresh();
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                var mrvNo = Request.QueryString["MRVNO"];
                var dtMaterialReceiptVoucher = uMethods.ConvertTo(repo.RptMaterialReceiptVoucher(mrvNo));
                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\MetarialReceiptVocher.rdlc";
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("MRVNO", mrvNo));
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DS_MaterialReceiptVoucher", dtMaterialReceiptVoucher));
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();
                //Response.Clear();
                //byte[] bytes = ReportViewer1.LocalReport.Render("PDF");
                //var fileNamewithType = "inline;filename=" + mrvNo + ".pdf";
                //Response.AddHeader("Content-Disposition", fileNamewithType);
                //Response.ContentType = "application/pdf";
                //Response.BinaryWrite(bytes);
                //Response.End();
            }
        }

        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            IUnitOfWork iuWork = new UnitOfWork();
            ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
            UtilityMethods uMethods = new UtilityMethods();
            var mrvNo = Request.QueryString["MRVNO"];
            var dtMrvJobDetails = uMethods.ConvertTo(repo.Rpt_MrvJobDetails(mrvNo));
            e.DataSources.Add(new ReportDataSource("DS_MRVJobDetails", dtMrvJobDetails));
        }
    }
}