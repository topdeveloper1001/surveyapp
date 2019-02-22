<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" CodeFile="Reports.aspx.cs" Inherits="SurveyApp.Reports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Reports</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div class="sectiontitle">
        <asp:Label ID="lblPageTitle" runat="server" Text="" />
    </div>

    <asp:Panel ID="pnlReportParams" runat="server" Visible="false" CssClass="width-med">
    <div class="well" style="margin-left:10px;margin-right:10px;">
        <div id="msgbox"></div>

        <div class="form-horizontal center-block" style="width:75%">
            <div class="form-group">
                <label class="col-sm-4 control-label darker">Client:</label>
                <div class="col-sm-8">
                    <asp:DropDownList CssClass="form-control" ID="cboClient" runat="server" OnSelectedIndexChanged="cboClient_SelectedIndexChanged" AutoPostBack="true" />
                </div>
            </div>

            <div class="form-group">
                <label class="col-sm-4 control-label darker">Location / Business Unit:</label>
                <div class="col-sm-8">
                    <asp:DropDownList CssClass="form-control" ID="cboLocation" runat="server" OnSelectedIndexChanged="cboLocation_SelectedIndexChanged"  AutoPostBack="true" />
                </div>
            </div>

            <div class="form-group">
                <label class="col-sm-4 control-label darker">Audit:</label>
                <div class="col-sm-8">
                    <asp:DropDownList CssClass="form-control" ID="cboSurvey" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <label class="col-sm-4 control-label darker">Date Range (From):</label>
                <div class="col-sm-3">
                    <div class="input-group date" id="fromdate">
                        <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control"  Text="" placeholder="DD MMM YYYY" />
                        <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span>
                    </div>
                </div>

                <label class="col-sm-2 control-label darker">To:</label>
                <div class="col-sm-3">
                    <div class="input-group date" id="todate">
                        <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control"  Text="" placeholder="DD MMM YYYY" />
                        <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span>
                    </div>

                </div>
            </div>
        
            <div class="form-group">
                <div class="col-sm-8 col-sm-offset-4">
                    <asp:Button ID="cmdRunReport" CssClass="btn btn-primary" runat="server" Text="Run Selected Report" 
                        OnCommand="cmdRunReport_Command"/>

                    &nbsp;&nbsp;&nbsp;

                    <asp:Button ID="btnExportReport" runat="server" Text="Export to CSV" CssClass="btn btn-success" OnClick="btnExportReport_Click" />
                
                </div>
            </div>

        </div>
    </div>

    </asp:Panel>
    
    <asp:Label ID="lblReportStatus" runat="server" Text="" />
    
    <asp:Panel ID="pnlReportResults" runat="server" Visible="false">
    <div style="margin-left:10px;margin-right:10px;">
        <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered table-condensed ardenttable" />
    </div>
    </asp:Panel>

</asp:Content>