<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashMemo.aspx.cs" Inherits="ASI.MGC.FS.Reports.CashMemo" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <rsweb:reportviewer id="ReportViewer1" hasprintbutton="true" runat="server" style="margin-right: 0; margin-top: 20px" width="1332" height="600px">
            </rsweb:reportviewer>
        </div>
    </form>
</body>
</html>
