﻿using System;
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
    public partial class BankReceiptVP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportViewer1.KeepSessionAlive = true;
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                const string voucherType = "BR";
                var voucherCode = Request.QueryString["docNo"];
                repo.Sp_GetVoucherDetails(voucherType, voucherCode);
                DataTable dtBankReceipt = uMethods.ConvertTo(repo.RptBankReceipt(voucherType, voucherCode));
                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\BankReceipt.rdlc";
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("VTYPE", voucherType));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("VCODE", voucherCode));
                var rds = new ReportDataSource("DS_BankReceipt", dtBankReceipt);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.DataBind();
                ReportViewer1.LocalReport.Refresh();
                Response.Clear();
                byte[] bytes = ReportViewer1.LocalReport.Render("PDF");
                var fileNamewithType = "inline;filename=" + voucherCode + ".pdf";
                Response.AddHeader("Content-Disposition", fileNamewithType);
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.End();
            }
        }
    }
}