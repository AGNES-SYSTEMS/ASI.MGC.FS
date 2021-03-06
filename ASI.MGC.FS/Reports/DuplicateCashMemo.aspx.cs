﻿using System;
using System.Data;
using System.Web.UI;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.Model.HelperClasses;
using Microsoft.Reporting.WebForms;

namespace ASI.MGC.FS.Reports
{
    public partial class DuplicateCashMemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportViewer1.KeepSessionAlive = true;
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                var invNo = Request.QueryString["cmNo"];
                var invType = "CM";
                DataTable dtCashMemo = uMethods.ConvertTo(repo.RptCashMemo(invNo, invType));

                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\CashMemo.rdlc";
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("INVNO", invNo));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("INVTYPE", invType));
                var rds = new ReportDataSource("DS_CashMemo", dtCashMemo);
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