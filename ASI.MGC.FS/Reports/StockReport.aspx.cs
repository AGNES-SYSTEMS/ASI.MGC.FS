﻿using System;
using System.Data;
using System.Web.UI;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.Model.HelperClasses;
using Microsoft.Reporting.WebForms;

namespace ASI.MGC.FS.Reports
{
    public partial class StockReport : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportViewer1.KeepSessionAlive = true;
            if (!Page.IsPostBack)
            {
                IUnitOfWork iuWork = new UnitOfWork();
                ReportRepository repo = iuWork.ExtRepositoryFor<ReportRepository>();
                UtilityMethods uMethods = new UtilityMethods();
                DataTable dtStockReport = uMethods.ConvertTo(repo.RptStockReport());
                ReportViewer1.LocalReport.ReportPath = "Reports\\RDLC Files\\StockReport.rdlc";
                var rds = new ReportDataSource("DS_StockReport", dtStockReport);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.DataBind();
                ReportViewer1.LocalReport.Refresh();
                Response.Clear();
                if (Request.QueryString["isExportMode"] != "1")
                {
                    byte[] bytes = ReportViewer1.LocalReport.Render("PDF");
                    var fileNamewithType = "inline;filename=StockReport.pdf";
                    Response.AddHeader("Content-Disposition", fileNamewithType);
                    Response.ContentType = "application/pdf";
                    Response.BinaryWrite(bytes);
                    Response.End();
                }
            }
        }
    }
}