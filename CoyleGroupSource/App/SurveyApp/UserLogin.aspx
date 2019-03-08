<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="UserLogin.aspx.cs" Inherits="SurveyApp.AppLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="css/login1.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    Welcome to the Coyle Group Audits Database.<br />
    <br />
    This is a secure system and you will need to provide your login details to access the site.<br />
    <br />


    <div class="userloginbox center-block form-horizontal">

        <div class="userlogin_outer">
                
            <asp:Panel ID="pnlUserLoginError" runat="server" CssClass="well alert-danger" Visible="false">
                Login credentials not valid.
            </asp:Panel>
                    
            <asp:Panel ID="applogin" CssClass="tab-pane fade in active" runat="server" DefaultButton="cmdApplicationLogin">
                <p>Please enter your login detail below</p>
                                                
                <div class="input-group" style="padding-bottom:10px;">
                    <span class="input-group-addon"><span class="glyphicon glyphicon-user"></span></span>
                    <asp:TextBox ID="txtUsername" placeholder="User Name" runat="server" CssClass="form-control"  Text="" />
                </div> 
                        
                <div class="input-group" style="padding-bottom:10px;">
                    <span class="input-group-addon"><span class="glyphicon glyphicon-lock"></span></span>
                    <asp:TextBox ID="txtPassword" placeholder="Password" autocomplete="off" TextMode="Password" runat="server" CssClass="form-control"  Text="" />
                </div> 
                    
    
                <div class="input-group center-block">
                    <asp:Button ID="cmdApplicationLogin" CssClass="btn btn-primary center-block" runat="server" Text="LOG IN" 
                        OnCommand="cmdApplicationLogin_Command"/>
                </div>
            </asp:Panel>
                
        </div>
                
        <br />

        <div class="userlogin_inner">
            To request an account, <br />please contact the site administrators. 
        </div>
            
    </div>
    
</asp:Content>