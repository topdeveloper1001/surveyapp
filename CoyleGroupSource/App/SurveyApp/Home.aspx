<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" CodeFile="Home.aspx.cs" Inherits="SurveyApp.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="row">
        <div class="col-sm-7">
            <br />
            Welcome to the Coyle Group Audits Database.<br />
            <br />

            <asp:Panel ID="pnlTerms" runat="server" CssClass="alert alert-warning" Visible="false">
                
                Please be aware that this system is externally monitored to ensure no breaches of confidentiality or data protection.<br />
                <br />
                Please report any concerns immediately to Coyle Group Ltd<br />
                <br />

                <div class="checkbox checkbox-primary">
                    <asp:CheckBox ID="chkIsAdmin" runat="server" OnCheckedChanged="chkIsAdmin_CheckedChanged" AutoPostBack="true" />
                    <asp:PlaceHolder ID="phAcceptTerms_User" runat="server" Visible="false">
                        <label>I accept the above statement. I have read, understood & will comply with the Coyle Group <a href="<%=ResolveUrl("~/downloads/CoyleGroup_CodeofConduct_SafetyAuditors.pdf") %>" target="_blank"><span class="glyphicon glyphicon-share-alt"></span> Code of Conduct  Safety Auditor</a> document</label>
                    </asp:PlaceHolder>
                    
                    <asp:PlaceHolder ID="phAcceptTerms_Client" runat="server" Visible="false">
                        <label>I accept the above statement</label>
                    </asp:PlaceHolder>

                </div>
            </asp:Panel>

            <asp:Panel ID="pnlTermsAccepted" runat="server" CssClass="alert alert-success" Visible="false">
                Thank you for agreeing to the terms and conditions outlined previously.<br />
                <br />
                You now have access to this system using the menu in the top right of the screen.
            </asp:Panel>

            <asp:Panel ID="pnlGrid" runat="server" Visible="false">
                <asp:Label ID="lblGridTitle" runat="server" Text="" />
                <div style="margin-left:10px;margin-right:10px;">
                    <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered table-condensed" />
                </div>
                <asp:Label ID="lblGridFooter" runat="server" Text="" />
            </asp:Panel>
        </div>
        <div class="col-sm-5">
            <div class="contentbutton dropshadow" >
                <br /><br />
                <img src="<%=ResolveUrl("~/images/HomeImg.png")%>" class="img-responsive" />
            </div>
        </div>
    </div>

</asp:Content>
