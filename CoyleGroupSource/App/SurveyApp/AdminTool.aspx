<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminTool.aspx.cs" CodeFile="AdminTool.aspx.cs" Inherits="SurveyApp.AdminTool" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <asp:PlaceHolder ID="phUserEditValidation" runat="server" Visible="false">
        <script>
            // List of options at: http://bootstrapvalidator.com/validators/
            //  http://www.jqueryscript.net/form/Powerful-Form-Validation-Plugin-For-jQuery-Bootstrap-3.html

            $(document).ready(function () {
                $('#frm')
                    .formValidation({
                        framework: 'bootstrap',
                        icon: {
                            valid: 'glyphicon glyphicon-ok',
                            invalid: 'glyphicon glyphicon-remove',
                            validating: 'glyphicon glyphicon-refresh'
                        },
                        live: 'submitted',
                        message: 'This is a required input and cannot be blank',
                        fields: {
                            ctl00$ContentPlaceHolder1$txtUserFirstname: { validators: { notEmpty: {} } },
                            ctl00$ContentPlaceHolder1$txtUserLastname: { validators: { notEmpty: {} } }
                        }
                    })
                    .on('err.field.fv', function (e, data) {
                        if (data.fv.getSubmitButton()) {
                            data.fv.disableSubmitButtons(false);
                        }
                    })
                    .on('success.field.fv', function (e, data) {
                        if (data.fv.getSubmitButton()) {
                            data.fv.disableSubmitButtons(false);
                        }
                    });
            });

        </script>
        </asp:PlaceHolder>

    <asp:PlaceHolder ID="phClientEditValidation" runat="server" Visible="false">
        <script>
            // List of options at: http://bootstrapvalidator.com/validators/
            //  http://www.jqueryscript.net/form/Powerful-Form-Validation-Plugin-For-jQuery-Bootstrap-3.html

            $(document).ready(function () {
                $('#frm')
                    .formValidation({
                        framework: 'bootstrap',
                        icon: {
                            valid: 'glyphicon glyphicon-ok',
                            invalid: 'glyphicon glyphicon-remove',
                            validating: 'glyphicon glyphicon-refresh'
                        },
                        live: 'submitted',
                        message: 'This is a required input and cannot be blank',
                        fields: {
                            ctl00$ContentPlaceHolder1$txtClientName: { validators: { notEmpty: {} } }
                        }
                    })
                    .on('err.field.fv', function (e, data) {
                        if (data.fv.getSubmitButton()) {
                            data.fv.disableSubmitButtons(false);
                        }
                    })
                    .on('success.field.fv', function (e, data) {
                        if (data.fv.getSubmitButton()) {
                            data.fv.disableSubmitButtons(false);
                        }
                    });
            });

        </script>
        </asp:PlaceHolder>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="row">
        <div class="col-md-6">
            <h4><asp:Label ID="lblPageTitle" runat="server" Text="-" /></h4>
        </div>
        <div class="pull-right" style="margin-right:30px;">
            <asp:Button ID="cmdAddNewUser" CssClass="btn btn-success btn-xs" runat="server" Text="New User"
                OnCommand="cmdAddNewUser_Command" Visible="false" />
            
            <asp:Button ID="cmdAddAll" CssClass="btn btn-success btn-xs" runat="server" Text="Add All"
                OnCommand="cmdAddAll_Command" Visible="false" />
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="cmdRemoveAll" CssClass="btn btn-danger btn-xs" runat="server" Text="Remove All"
                OnCommand="cmdRemoveAll_Command" Visible="false" />
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="cmdCloseEditUserAccess" CssClass="btn btn-default" runat="server" Text="Close" 
                OnCommand="cmdCloseEditUserAccess_Command"/>
        </div>
    </div>

    <asp:Panel ID="pnlGrid" runat="server" Visible="false">
        <div style="margin-left:10px;margin-right:10px;">
            <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered table-condensed" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlClientEdit" runat="server" Visible="false" CssClass="form-horizontal">
        
    <div class="form-group hidden">
        <label class="col-sm-3 control-label">Client Ref</label>
        <div class="col-sm-9">
            <asp:TextBox ID="txtClientRef" runat="server" CssClass="form-control"  Text=""  MaxLength="50"/>
        </div>
    </div>

    <div class="form-group">
        <label class="col-sm-3 control-label">Client Name</label>
        <div class="col-sm-9">
            <asp:TextBox ID="txtClientName" runat="server" CssClass="form-control"  Text="" MaxLength="255" />
        </div>
    </div>
        
    <div class="form-group">
        <div class="col-sm-9 col-sm-offset-3">
            <asp:Button ID="cmdSaveClient" CssClass="btn btn-primary" runat="server" Text="Save" 
                OnCommand="cmdSaveClient_Command"/>

            &nbsp;&nbsp;&nbsp;

            <asp:Button ID="cmdCancelEditClient" CssClass="btn btn-default" runat="server" Text="Cancel" 
                OnCommand="cmdCancelEditClient_Command"/>

             &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;

             <asp:Button ID="cmdDeleteClient" CssClass="btn btn-danger" runat="server" Text="Delete" 
                    OnCommand="cmdDeleteClient_Command"
                    OnClientClick="if (!confirm('Are you sure do you want to delete this client?')) return false;"
                    />

        </div>
    </div>
    </asp:Panel>
        
    <asp:Panel ID="pnlLocationEdit" runat="server" Visible="false" CssClass="form-horizontal">
        
    <div class="form-group">
        <label class="col-sm-3 control-label">Client</label>
        <div class="col-sm-6">
            <asp:Label ID="lblLocationClientName" runat="server" Text="" CssClass="form-control" />
        </div>
    </div>
    
        <div class="form-group">
        <label class="col-sm-3 control-label">Business Unit</label>
        <div class="col-sm-6">
            <asp:TextBox ID="txtLocationBusinessUnit" runat="server" CssClass="form-control"  Text="" MaxLength="255" />
        </div>
    </div>

    <div class="form-group">
        <label class="col-sm-3 control-label">Site Name</label>
        <div class="col-sm-6">
            <asp:TextBox ID="txtLocationName" runat="server" CssClass="form-control"  Text="" MaxLength="255" />
        </div>
    </div>
        
    <div class="form-group">
        <label class="col-sm-3 control-label">Site Address</label>
        <div class="col-sm-6">
            <asp:TextBox ID="txtLocationAddress" runat="server" CssClass="form-control"  Text="" MaxLength="255" />
        </div>
    </div>

    <div class="form-group">
        <div class="col-sm-9 col-sm-offset-3">
            <asp:Button ID="cmdSaveLocation" CssClass="btn btn-primary" runat="server" Text="Save" 
                OnCommand="cmdSaveLocation_Command"/>
           
            &nbsp;&nbsp;&nbsp;

            <asp:Button ID="cmdCancelLocationt" CssClass="btn btn-default" runat="server" Text="Cancel" 
                OnCommand="cmdCancelLocation_Command"/>

            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;

            <asp:Button ID="cmdDeleteLocation" CssClass="btn btn-danger" runat="server" Text="Delete" 
                OnCommand="cmdDeleteLocation_Command"
                OnClientClick="if (!confirm('Are you sure do you want to delete this location?')) return false;"
                />
            
        </div>
    </div>
    <asp:HiddenField ID="hidLocationClientId" runat="server" />
    </asp:Panel>
    
    <asp:Panel ID="pnlUserEdit" runat="server" Visible="false" CssClass="form-horizontal">
        <div class="row">
            <!-- left side - left side - left side - left side - left side - left side - left side -->
            <div class="col-sm-6">
                <div class="inputgroup-titlebar">USER DETAILS</div>
                <div class="inputgroup">
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Firstname</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtUserFirstname" runat="server" CssClass="form-control"  Text="" />
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Lastname</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtUserLastname" runat="server" CssClass="form-control"  Text="" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-4 control-label">EMail Address</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtUserEMail" runat="server" CssClass="form-control"  Text="" />
                        </div>
                    </div>
                </div>

                <div class="inputgroup-titlebar">LOGIN INFORMATION</div>
                <div class="inputgroup">
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Username</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtUserLoginName" runat="server" CssClass="form-control"  Text="" autocomplete="off" />
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Password</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtUserPassword" runat="server" CssClass="form-control"  Text="" TextMode="Password" autocomplete="off" />
                        </div>
                    </div>
    
                    <div class="inputgroup-break"></div>

                    <i>
                        When editing an existing user, leave the password box blank unless you want to reset the password
                    </i>
                </div>
                
            </div>

            <!-- right side  - right side  - right side  - right side  - right side  - right side  -->
            <div class="col-sm-6">
                    
                <div class="inputgroup-titlebar">SYSTEM ACCESS</div>
                <div class="inputgroup">
                        
                    <div class="form-group">
                        <label class="col-sm-3 control-label">Role(s)</label>
                        <div class="col-sm-9">
                            <div class="checkbox checkbox-primary">
                                <asp:CheckBox ID="chkIsNewHire" runat="server" />
                                <label>Consultant</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-9 col-sm-offset-3">
                            <div class="checkbox checkbox-primary">
                                <asp:CheckBox ID="chkIsLeader" runat="server" />
                                <label>Client</label>
                            </div>
                        </div>
                    </div>
                    <hr style="padding:0;margin:0" />
                    <div class="text-center"><i>- Internal Use Only -</i></div>
                    <div class="form-group">
                        <div class="col-sm-9 col-sm-offset-3">
                            <div class="checkbox checkbox-primary">
                                <asp:CheckBox ID="chkIsAdmin" runat="server" />
                                <label>Website Admin</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-9 col-sm-offset-3">
                            <div class="checkbox checkbox-primary">
                                <asp:CheckBox ID="chkIsSuperAdmin" runat="server" />
                                <label>Super Admin</label>
                            </div>
                        </div>
                    </div>
                </div>
                
 
                <!-- FORM BUTTONS - SAVE, CANCEL, DELETE -->
                <br />
                <div class="form-group">
                    <div class="col-sm-9 col-sm-offset-3">
                        <asp:Button ID="cmdSaveUser" CssClass="btn btn-primary" runat="server" Text="SUBMIT" 
                            OnCommand="cmdSaveUser_Command"/>

                        &nbsp;&nbsp;&nbsp;

                        <asp:Button ID="cmdCancelEditUser" CssClass="btn btn-default" runat="server" Text="CANCEL" 
                            UseSubmitBehavior="false"
                            OnCommand="cmdCancelEditUser_Command"/>

                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;

                            <asp:Button ID="cmdDeleteUser" CssClass="btn btn-danger" runat="server" Text="Delete" 
                                OnCommand="cmdDeleteUser_Command"
                                UseSubmitBehavior="false"
                                OnClientClick="if (!confirm('Are you sure do you want to delete this user?')) return false;"
                                />

                    </div>
                </div>
                    
                <!-- hide for now -->
                <div class="hidden">
                        <div class="form-group">
                            <asp:CheckBox ID="chkIsBuddy" runat="server" />
                        <label class="col-sm-4 control-label">Start Date</label>
                        <div class="col-sm-5">
                            <div class="input-group date" id="startdate">
                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control"  Text="" placeholder="MM/DD/YYYY" />
                                <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span>
                            </div>
                        </div>
                        <div class="col-sm-2 col-sm-offset-1">
                            <div class="checkbox checkbox-primary">
                                <asp:CheckBox ID="chkIsRN" runat="server" />
                                <label>RN</label>
                            </div>
                        </div>
                    </div>
                        <div class="form-group">
                        <label class="col-sm-4 control-label">Facility</label>
                        <div class="col-sm-8">
                            <asp:DropDownList ID="cboUserFacility" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                    <div class="inputgroup-break"></div>

                    <div class="form-group">
                        <label class="col-sm-3 control-label">Leader</label>
                        <div class="col-sm-9">
                            <asp:DropDownList ID="cboLeader" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    
                    <div class="form-group">
                    <label class="col-sm-3 control-label">Buddy</label>
                    <div class="col-sm-9">
                        <asp:DropDownList ID="cboBuddy" runat="server" CssClass="form-control" />
                    </div>
                    <div class="inputgroup-titlebar">DEACTIVATION</div>
                <div class="inputgroup">
                    <div class="form-group">
                        <label class="col-sm-3 control-label">Inactive Date</label>
                        <div class="col-sm-5">
                            <div class="input-group date" id="inactivedate">
                                <asp:TextBox ID="txtInactiveDate" runat="server" CssClass="form-control"  Text="" placeholder="MM/DD/YYYY" />
                                <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span>
                            </div>
                        </div>
                    </div>
                </div>
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Network Username</label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="txtNetworkLogin" runat="server" CssClass="form-control"  Text="" />
                        </div>
                    </div>
                </div>




            </div>
            
        </div>

    </asp:Panel>

</asp:Content>
