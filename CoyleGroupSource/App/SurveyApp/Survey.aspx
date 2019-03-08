 <%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Survey.aspx.cs" Inherits="SurveyApp.Survey" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Safety Recruitment Agency and Consulting Company in Ireland and UK - Coyle Group</title>

    <link href="<%=Page.ResolveUrl("~/css/customicons2.css")%>" rel="stylesheet" />
    <link href="<%=Page.ResolveUrl("~/css/print1.css")%>" rel="stylesheet" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <asp:Panel ID="pnlClientRef" runat="server" CssClass="alert alert-info title-alert" Visible="false">
        <asp:Label ID="lblClientRef" runat="server" Text="" />
    </asp:Panel>

    <asp:Panel ID="pnlStatus" runat="server" Visible="false">
        <asp:Label ID="lblStatus" runat="server" Text="" />
    </asp:Panel>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <asp:PlaceHolder ID="phQuickMenu" runat="server" Visible="false">
    <div class="row">
        <div class="col-sm-9 col-sm-offset-3">

            <nav aria-label="Page navigation" >
            <ul class="pagination">
                <li <asp:Literal ID="litQuickMenuSummary" runat="server" />>
                    <a href="<%=ResolveUrl("~/audit/summary/" + m_idref) %>">
                        <span class="glyphicon glyphicon-list-alt"></span> Summary
                    </a>
                </li>
                <li <asp:Literal ID="litQuickMenuQa" runat="server" />>
                    <a href="<%=ResolveUrl("~/audit/qa/" + m_idref) %>">
                        <span class="glyphicon glyphicon-check"></span> Audit Details 
                    </a>
                </li>
                <li <asp:Literal ID="litQuickMenuClientApproval" runat="server" />>
                    <a href="<%=ResolveUrl("~/audit/clientapproval/" + m_idref) %>">
                        <span class="glyphicon glyphicon-thumbs-up"></span> Client Acceptance 
                    </a>
                </li>
                <li <asp:Literal ID="litQuickMenuClientQa" runat="server" />>
                    <a href="<%=ResolveUrl("~/audit/clientqa/" + m_idref) %>">
                        <span class="glyphicon glyphicon-check"></span> Client Actions
                    </a>
                </li>
                <li <asp:Literal ID="litQuickMenuPrint" runat="server" />>
                    <a href="<%=ResolveUrl("~/audit/print/" + m_idref) %>">
                        <span class="glyphicon glyphicon-print"></span> Print
                    </a>
                </li>
            </ul>
        </nav>
        </div>
    </div>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="phFileUploadDialog" runat="server" Visible="false">
    <div class="modal fade" id="fileupload" tabindex="-1" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">File Upload</h4>
                </div>
                <div class="modal-body">
                    
                    <div class="form-group">
                        <label class="col-sm-3 control-label">Choose File</label>
                        <div class="col-sm-9">
                            <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <br />
                </div>
                <div class="modal-footer">
                    <div class="form-group">
                        <div class="col-sm-8 col-sm-offset-4">
                            <asp:Button ID="cmdUploadFile" CssClass="btn btn-primary" runat="server" Text="Upload" 
                                OnCommand="cmdUploadFile_Command"/>

                            &nbsp;&nbsp;&nbsp;

                            <asp:Button ID="cmdCancelUpload" CssClass="btn btn-default" runat="server" Text="Cancel" 
                                data-dismiss="modal"
                                OnCommand="cmdCancelUpload_Command"/>
                        </div>
                    </div>                        
                </div>
            </div>
        </div>
    </div>
    </asp:PlaceHolder>
    
    <asp:Panel ID="pnlTitleAndAddNewBtn" runat="server" style="padding:10px;" Visible="false">
        <div>
            <h3><asp:Label ID="lblPageTitle" runat="server" Text="" /></h3>
        </div>
        <div class="pull-right" style="padding-right:10px;padding-bottom:10px;">
             <asp:Button ID="cmdAddNewClient" CssClass="btn btn-success btn-xs" runat="server" Text="New Client"
                OnCommand="cmdAddNewClient_Command" Visible="false" />

            <asp:Button ID="cmdAddNewSurvey" CssClass="btn btn-success btn-xs" runat="server" Text="New Audit"
                OnCommand="cmdAddNewSurvey_Command" Visible="false" />
        </div>
    </asp:Panel>
    
    <asp:Panel ID="pnlGrid" runat="server" Visible="false">
        <div style="margin-left:10px;margin-right:10px;">
            <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered table-condensed" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlSummary" runat="server" Visible="false" CssClass="form-horizontal width-med">
        
        <!-- handy way to disable all inputs -->
        <fieldset <asp:Literal ID="litSummaryFieldset" runat="server" /> > 
        
        <div class="form-group">
            <label class="col-sm-2 control-label">Audit</label>
            <div class="col-sm-4">
                <asp:DropDownList ID="cboSurveyTemplate" runat="server" CssClass="form-control" />
            </div>
            
            <label class="col-sm-2 control-label">Client Manager</label>
            <div class="col-sm-4">
                <asp:DropDownList ID="cboClientMgr" runat="server" CssClass="form-control" />
            </div>    
        </div>

        <div class="form-group">
            <label class="col-sm-2 control-label">Date of Audit</label>
            <div class="col-sm-3">
                <div class="input-group date" id="auditdate">
                    <asp:TextBox ID="txtDateOfAudit" runat="server" CssClass="form-control"  Text="" MaxLength="255" placeholder="DD MMM YYYY"  />
                    <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span>
                </div>
            </div>

            <label class="col-sm-3 control-label">Client Administrator</label>
            <div class="col-sm-4">
                <asp:DropDownList ID="cboClientAdmin" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="form-group">
            <label class="col-sm-2 control-label">Consultant</label>
            <div class="col-sm-4">
                <asp:DropDownList ID="cboAuditor" runat="server" CssClass="form-control" />
            </div>

             <label class="col-sm-2 control-label">Site Contact</label>
            <div class="col-sm-4">
                <asp:TextBox ID="txtClientContact" runat="server" CssClass="form-control"  Text="" MaxLength="255" />
            </div>
        </div>
                 
        <div class="form-group">
            <label class="col-sm-2 control-label">Scope of Work</label>
            <div class="col-sm-4">
                <asp:TextBox ID="txtScopeOfWork" runat="server" CssClass="form-control hidden-print"  Text="" TextMode="MultiLine" Rows="4"  />
                <asp:Label ID="lblScopeOfWork" runat="server" Text="" CssClass="print-form-control visible-print" />
            </div>

            <label class="col-sm-2 control-label">Site Description</label>
            <div class="col-sm-4">
                <asp:TextBox ID="txtSiteDesc" runat="server" CssClass="form-control hidden-print"  Text="" MaxLength="500" TextMode="MultiLine" Rows="4"  />
                <asp:Label ID="lblSiteDesc" runat="server" Text="" CssClass="print-form-control visible-print" />
            </div>
        </div>

        <div class="form-group">
            <label class="col-sm-2 control-label">Overall Summary</label>
            <div class="col-sm-10">
                <asp:TextBox ID="txtSummary" runat="server" CssClass="form-control hidden-print"  Text="" TextMode="MultiLine" Rows="10"  />
                <asp:Label ID="lblSummary" runat="server" Text="" CssClass="print-form-control visible-print" />
            </div>
        </div>
            
        <div class="form-group">
            <label class="col-sm-2 control-label">Weather Conditions</label>
            <div class="col-sm-4">
                <asp:TextBox ID="txtWeatherConditions" runat="server" CssClass="form-control"  Text="" MaxLength="500" />
            </div>
        
            <label class="col-sm-2 control-label">Job Status</label>
            <div class="col-sm-4">
                <asp:DropDownList ID="cboStatus" runat="server" CssClass="form-control" />
            </div>
        </div>
            
        <asp:Panel ID="pnlSummarySaveButtons" runat="server" CssClass="col-sm-4 col-sm-offset-8 hidden-print">
            <asp:Button ID="cmdSaveSummary" CssClass="btn btn-primary" runat="server" Text="Save" 
                OnCommand="cmdSaveSummary_Command"/>
           
            &nbsp;&nbsp;&nbsp;

            <asp:Button ID="cmdCancelSummary" CssClass="btn btn-default" runat="server" Text="Cancel" 
                UseSubmitBehavior="false" OnCommand="cmdCancelSummary_Command"/>

            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;

            <asp:Button ID="cmdDeleteSurvey" CssClass="btn btn-danger" runat="server" Text="Delete" 
                UseSubmitBehavior="false" OnCommand="cmdDeleteSurvey_Command"
                OnClientClick="if (!confirm('Are you sure do you want to delete this audit?')) return false;"
                />
            <br />
        </asp:Panel>
            
        </fieldset>
        
        <asp:Panel ID="pnlSummaryCloseButtons" runat="server" CssClass="form-group">
        <div class="col-sm-4 col-sm-offset-8">
                <asp:Button ID="cmdCloseSummary" CssClass="btn btn-default" runat="server" Text="Close" 
                OnCommand="cmdCloseSummary_Command"/>
        </div>
        </asp:Panel>

        <asp:HiddenField ID="hidLocationId" runat="server" />

        <!-- moment - for date validation -->
	    <script src="<%=ResolveUrl("~/lib/moment/moment.js")%>"></script>
        <script src="<%=ResolveUrl("~/js/summary_validation.js")%>"></script>
    </asp:Panel>

    <asp:Panel ID="pnlQandA" runat="server" Visible="false">
        
        <asp:Panel ID="pnlExpandButtons" runat="server" CssClass="well well-sm">
            <asp:LinkButton ID="lnkShowAllYesItems" OnClick="lnkShowAllYesItems_Click" runat="server" CssClass="btn btn-xs btn-link">
                <span class="text-success">
                    <span class="glyphicon glyphicon-ok-circle"></span> Show <b>Yes</b> items only
                </span>
            </asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
          
            <asp:LinkButton ID="lnkShowAllNoItems" OnClick="lnkShowAllNoItems_Click" runat="server" CssClass="btn btn-xs btn-link">
                <span class="text-danger">
                    <span class="glyphicon glyphicon-certificate"></span> Show <b>No</b> items only
                </span>
            </asp:LinkButton>
            &nbsp;&nbsp;&nbsp;

            <asp:LinkButton ID="lnkShowAllNAItems" OnClick="lnkShowAllNAItems_Click" runat="server" CssClass="btn btn-xs btn-link">
                <span class="cellicon">
                    <span class="glyphicon glyphicon-certificate"></span> Show <b>N/A</b> items only
                </span>
            </asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            
            <asp:LinkButton ID="lnkShowAllUnansweredItems" OnClick="lnkShowAllUnansweredItems_Click" runat="server" CssClass="btn btn-xs btn-link">
                <span class="text-warning">
                    <span class="glyphicon glyphicon-unchecked"></span> Show <b>Unanswered</b> items only
                </span>
            </asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            
            <asp:LinkButton ID="lnkShowAllItems" OnClick="lnkShowAllItems_Click" runat="server" CssClass="btn btn-xs btn-link">
                <span class="glyphicon glyphicon-th-list"></span> Show <b>All</b> items
            </asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            
            <div class="right">
                <a href="#" onclick="showAll();" class="btn btn-xs btn-link">+ Show all Details</a>
                &nbsp;&nbsp;&nbsp;
                <a href="#" onclick="hideAll();" class="btn btn-xs btn-link">- Hide all Details</a>
            </div>
        </asp:Panel>
        
        <table class="table table-striped table-bordered table-condensed coylegrouptable">
            <tbody>
                <asp:Repeater ID="rptQandA" runat="server">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="phCategoryHeading" runat="server" Visible="false">
                            <tr class="header">
                                <th>&nbsp;</th>
                                <th>
                                    <asp:Label ID="lblQuestionCategory" runat="server" Text="" />
                                </th>
                                
                                <asp:PlaceHolder ID="phYesNoNAHeading" runat="server" Visible="false">
                                <th class="text-center review-answer">Yes</th>
                                <th class="text-center review-answer">No</th>
                                <th class="text-center review-answer">N/A</th>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="phAgreeDisagreeHeading" runat="server" Visible="false">
                                <th class="text-center review-answer">Strongly Agree</th>
                                <th class="text-center review-answer">Agree</th>
                                <th class="text-center review-answer">Don't Know</th>
                                <th class="text-center review-answer">Disagree</th>
                                <th class="text-center review-answer">Strongly Disagree</th>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="phTextAnswerHeading" runat="server" Visible="false">
                                <th colspan="4">
                                    &nbsp;
                                </th>
                                </asp:PlaceHolder>
                                
                                <th>&nbsp;</th>
                            </tr>
                        
                        </asp:PlaceHolder>

                        <tr>
                            <td>
                                <asp:Panel ID="pnlStatus" runat="server">
                                    <asp:Label ID="lblStatus" runat="server" Text="" style="font-size:1.8em;" />
                                </asp:Panel>
                            </td>
                            <td>
                                <!-- anchor so I can auto-focus back here on postback -->
                                <a name="<%# Eval("questionid") %>"></a>
                                <asp:Label ID="lblQuestionTitle" runat="server" Text="" />
                            </td>

                            <asp:PlaceHolder ID="phYesNoNAAnswers" runat="server" Visible="false">
                            <td class="center-middle">
                                <asp:HyperLink ID="hrefTick6" runat="server">
                                    <asp:Label ID="lblAnswer6" runat="server" Text="" CssClass="glyphicon glyphicon-one-fine-empty-dot cellicon" />
                                </asp:HyperLink>
                            </td>
                            <td class="center-middle">
                                <asp:HyperLink ID="hrefTick7" runat="server">
                                    <asp:Label ID="lblAnswer7" runat="server" Text="" CssClass="glyphicon glyphicon-one-fine-empty-dot cellicon" />
                                </asp:HyperLink>
                            </td>
                            <td class="center-middle">
                                <asp:HyperLink ID="hrefTick8" runat="server">
                                    <asp:Label ID="lblAnswer8" runat="server" Text="" CssClass="glyphicon glyphicon-one-fine-empty-dot cellicon" />
                                </asp:HyperLink>
                            </td>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="phAgreeDisagreeAnswers" runat="server" Visible="false">
                            <td class="center-middle">
                                <asp:HyperLink ID="hrefTick1" runat="server">
                                    <asp:Label ID="lblAnswer1" runat="server" Text="" CssClass="glyphicon glyphicon-one-fine-empty-dot cellicon" />
                                </asp:HyperLink>
                            </td>
                            <td class="center-middle">
                                <asp:HyperLink ID="hrefTick2" runat="server">
                                    <asp:Label ID="lblAnswer2" runat="server" Text="" CssClass="glyphicon glyphicon-one-fine-empty-dot cellicon" />
                                </asp:HyperLink>
                            </td>
                            <td class="center-middle">
                                <asp:HyperLink ID="hrefTick3" runat="server">
                                    <asp:Label ID="lblAnswer3" runat="server" Text="" CssClass="glyphicon glyphicon-one-fine-empty-dot cellicon" />
                                </asp:HyperLink>
                            </td>
                            <td class="center-middle">
                                <asp:HyperLink ID="hrefTick4" runat="server">
                                    <asp:Label ID="lblAnswer4" runat="server" Text="" CssClass="glyphicon glyphicon-one-fine-empty-dot cellicon" />
                                </asp:HyperLink>
                            </td>
                            <td class="center-middle">
                                <asp:HyperLink ID="hrefTick5" runat="server">
                                    <asp:Label ID="lblAnswer5" runat="server" Text="" CssClass="glyphicon glyphicon-one-fine-empty-dot cellicon" />
                                </asp:HyperLink>
                            </td>
                            </asp:PlaceHolder>
                            
                            <td>
                                <asp:HyperLink ID="hrefMore" runat="server" CssClass="btn btn-xs btn-default">
                                    <span class="glyphicon glyphicon-edit"></span> More
                                </asp:HyperLink>
                            </td>
                        </tr>
                        
                        <tr class="moreinfo" <asp:Literal ID="litMoreInfo" runat="server" /> >
                            <td>&nbsp;</td>
                            <td colspan="3">
                                
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <label class="col-sm-2 control-label">Classification</label>
                                        <div class="col-sm-10">
                                            <asp:DropDownList ID="cboClassification" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-2 control-label">Observations / Recommendations</label>
                                        <div class="col-sm-10">
                                            <asp:TextBox ID="txtObservations" runat="server" CssClass="form-control hidden-print"  Text="" TextMode="MultiLine" Rows="4" />
                                            <asp:Label ID="lblObservations" runat="server" Text="" CssClass="print-form-control visible-print" />
                                        </div>
                                    </div>

                                    <asp:Panel ID="pnlFileUploads" runat="server" CssClass="form-group">
                                        <label class="col-sm-2 control-label">Attachments</label>
                                        <div class="col-sm-8">
                                            <ul class="list-group">
                                            <asp:Repeater ID="rptQuestionFileUploads" runat="server">
                                                <ItemTemplate>
                                                <li class="list-group-item">
                                                    
                                                    <span class="glyphicon glyphicon-download-alt"></span>
                                                    &nbsp;

                                                    <asp:HyperLink ID="hrefViewUploadFile" runat="server">
                                                        <%# Eval("sourcefilename") %>
                                                    </asp:HyperLink>

                                                    <asp:Image ID="imgFilePreview" runat="server" CssClass="file-preview" />
                                                    
                                                    <asp:PlaceHolder ID="phRemoveFile" runat="server">
                                                    <span class="pull-right">
                                                        <a style="cursor:pointer;" onclick="deleteFile(<%# DataBinder.Eval(((RepeaterItem)Container.NamingContainer.NamingContainer).DataItem, "questionid")%>, <%# Eval("fileid") %>);">
                                                            <span class="text-danger">
                                                                <span class="glyphicon glyphicon-remove"></span> Remove
                                                            </span>
                                                        </a>
                                                    </span>
                                                    </asp:PlaceHolder>

                                                </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            </ul>
                                        </div>
                                        <div class="col-sm-2">
                                            <asp:HyperLink ID="hrefAddFile" runat="server" CssClass="btn btn-xs btn-default">
                                                <span class="glyphicon glyphicon-upload"></span> Add File
                                            </asp:HyperLink>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </td>
                            <td colspan="2">&nbsp;</td>
                        </tr>
                        

                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

        <div class="right">
            <asp:Button ID="cmdSubmitToHQ" CssClass="btn btn-primary" runat="server" Text="Submit to H/Q" 
                 OnClientClick="if (!confirm('Submit this audit to HQ for processing?')) return false;" 
                 OnCommand="cmdSubmitToHQ_Command" />
        </div>
    
        <br /><br />

        <script src="<%=ResolveUrl("~/js/q_and_a.js")%>"></script>

    </asp:Panel>
    
    <asp:Panel ID="pnlClientApproval" runat="server" Visible="false" CssClass="form-horizontal width-sm">
        
        <div class="well">
            Please click the green "Approve" button in order to release this audit.<br />
            <br />
	        Alternatively, if you have any concerns, enter your comments here and click "Reject"
	    </div>

        <!-- handy way to disable all inputs -->
        <fieldset <asp:Literal ID="litClientApprovalFieldset" runat="server" /> >
        
        <div class="form-group">
            <label class="col-sm-3 control-label">Approval Status</label>
            <div class="col-sm-7">
                <asp:DropDownList ID="cboClientApprovalStatus" runat="server" CssClass="form-control" />
            </div>
        </div>
        
        <div class="form-group">
            <label class="col-sm-3 control-label">Approve/Reject Comments</label>
            <div class="col-sm-7">
                <asp:TextBox ID="txtClientApprovalComments" runat="server" CssClass="form-control"  Text="" TextMode="MultiLine" Rows="8"  />
            </div>
        </div>
        
        <div class="form-group">
            <div class="col-sm-7 col-sm-offset-3">
                <asp:Button ID="cmdClientAccept" CssClass="btn btn-success" runat="server" Text="Approve" 
                    OnCommand="cmdClientAccept_Command"/>
           
                &nbsp;&nbsp;&nbsp;

                <asp:Button ID="cmdClientReject" CssClass="btn btn-danger" runat="server" Text="Reject" 
                    OnCommand="cmdClientReject_Command"/>
           </div>
        </div>
        </fieldset>

        <script src="<%=ResolveUrl("~/js/clientapproval_validation.js")%>"></script>
    </asp:Panel>

    <asp:Panel ID="pnlClientResponses" runat="server" Visible="false">
        
        <asp:Panel ID="pnlRecentlyApproved" runat="server" Visible="false">
            <div class="alert alert-warning alert-dismissable center-block" style="max-width:800px;" id="success-alert">
                <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                <h1>This audit is now released</h1>
                <asp:Panel ID="pnlClientOutstandingItems" runat="server">
                    The following is a list of action items that must be closed out.
                </asp:Panel>
            </div>

            <script>
                // auto-hide the alert after a few seconds
                $("#success-alert").fadeTo(10000, 500).slideUp(500, function(){
                    $("#success-alert").slideUp(500);
                });
            </script>
        </asp:Panel>

        <asp:Panel ID="pnlAllClientItemsClosed" runat="server" Visible="false">
            <div class="alert alert-success center-block" style="max-width:700px;">
                <h1>Congratulations</h1>
                <div>
                    All action items have been closed on this audit.
                </div>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="pnlClientExpandButtons" runat="server" CssClass="well well-sm">
            <asp:LinkButton ID="lnkShowClientOpenOnly" OnClick="lnkShowClientOpenOnly_Click" runat="server" CssClass="btn btn-xs btn-link">
                <span class="text-success">
                    <span class="glyphicon glyphicon-exclamation-sign"></span> Show <b>Open</b> items only
                </span>
            </asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
          
            <asp:LinkButton ID="lnkShowClientClosedItems" OnClick="lnkShowClientClosedItems_Click" runat="server" CssClass="btn btn-xs btn-link">
                <span class="text-warning">
                    <span class="glyphicon glyphicon-off"></span> Show <b>All</b> items 
                </span>
            </asp:LinkButton>
        </asp:Panel>

        <table class="table table-striped table-bordered coylegrouptable">
            <tbody>
                <asp:Repeater ID="rptClientResponses" runat="server">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="phCategoryHeading" runat="server" Visible="false">
                        <thead>
                            <tr>
                                <th>Item</th>
                                <th>Classification</th>
                                <th>Observations / Recommendations</th>
                                <th style="min-width:250px;">Assigned To / Client Actions</th>
                                <th style="width:180px;">Target Date</th>
                                <th style="min-width:220px;">Attachments</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        </asp:PlaceHolder>

                        <tr id="row_q<%# Eval("questionid") %>">
                            <td colspan="6" class="rowheader">
                                <!-- anchor so I can auto-focus back here on postback -->
                                <a name="<%# Eval("questionid") %>"></a>
                                
                                <span class="text-info"><%# Eval("questioncatname") %>:</span>
                                <%# Eval("questiontext") %>
                            </td>
                             <td class="rowheader">
                                <asp:PlaceHolder ID="phCloseButton" runat="server">
                                    <a class="btn btn-xs btn-warning" onclick="closeClientResponse(<%# Eval("questionid") %>);">
                                        <span class="glyphicon glyphicon-off"></span> Close
                                    </a>
                                </asp:PlaceHolder>
                            </td>
                        </tr>
                        <tr id="row_a<%# Eval("questionid") %>">
                            <td>&nbsp;</td>
                            <td>
                                <%# Eval("classificationtitle") %>
                            </td>
                            <td>
                                <asp:Label ID="lblObservations" runat="server" Text="" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtClientResponse" runat="server" CssClass="form-control"  Text="" TextMode="MultiLine" style="height:80px;  " />
                            </td>
                            <td>
                                <div class="input-group date" id="clienttargetdate" questionId="<%# Eval("questionid") %>">
                                    <asp:TextBox ID="txtTargetDate" runat="server" CssClass="form-control"  Text="" MaxLength="50" placeholder="DD MMM YYYY"  />
                                    <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span>
                                </div>
                            </td>
                            <td colspan="2">
                                <ul class="list-group">
                                    <asp:Repeater ID="rptClientFileUploads" runat="server">
                                        <ItemTemplate>
                                    <li class="list-group-item">
                                        <asp:HyperLink ID="hrefViewUploadFile" runat="server">
                                            <span class="glyphicon glyphicon-download-alt"></span>
                                            
                                            <%# Eval("sourcefilename") %>
                                        </asp:HyperLink>

                                        <asp:Image ID="imgFilePreview" runat="server" CssClass="file-preview" />
                                                    
                                        <asp:PlaceHolder ID="phRemoveFile" runat="server">
                                        <span class="pull-right">
                                            <a style="cursor:pointer;" onclick="deleteFile(<%# DataBinder.Eval(((RepeaterItem)Container.NamingContainer.NamingContainer).DataItem, "questionid")%>, <%# Eval("fileid") %>);">
                                                <span class="text-danger">
                                                    <span class="glyphicon glyphicon-remove"></span> Remove
                                                </span>
                                            </a>
                                        </span>
                                        </asp:PlaceHolder>

                                    </li>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <li class="list-group-item">
                                         <asp:HyperLink ID="hrefAddFile" runat="server" CssClass="btn btn-xs btn-default">
                                            <span class="glyphicon glyphicon-upload"></span> Add File
                                        </asp:HyperLink>
                                    </li>
                                </ul>
                            </td>

                        </tr>
                        
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    
        <script src="<%=ResolveUrl("~/js/client_qa.js")%>"></script>
    </asp:Panel>

    <asp:PlaceHolder ID="phFileUploadScripts" runat="server" Visible="false">

    <asp:HiddenField ID="hidSelectedQuestionId" runat="server" />
    <asp:HiddenField ID="hidSelectedFileId" runat="server" />
    <asp:Button ID="cmdRemoveFile" runat="server" Text="" CssClass="hidden" OnCommand="cmdRemoveFile_Command" />

    <script>
        function uploadfile(questionId) {
            // record the current questionId for the file upload
            document.getElementById("<%=hidSelectedQuestionId.ClientID%>").value = questionId;
            $('#fileupload').modal('show');
        }

        function deleteFile(questionId, fileId) {
            // record the current fileId for deletion and the questionid for focus..
            document.getElementById("<%=hidSelectedQuestionId.ClientID%>").value = questionId;
            document.getElementById("<%=hidSelectedFileId.ClientID%>").value = fileId;

            bootbox.confirm({
                scrollTop: 0,
                title: "Confirm file deletion",
                message: "Are you sure you want to delete this file? This cannot be undone.",
                buttons: {
                    cancel: {
                        label: '<i class="fa fa-times"></i> No, cancel'
                    },
                    confirm: {
                        label: '<i class="fa fa-check"></i> Yes, delete the file',
                        className: 'btn-danger'
                    }
                },
                callback: function (result) {
                    if (result == true) {
                        // click the button!
                        document.getElementById("<%=cmdRemoveFile.ClientID%>").click();
                    }
                }
            });
        }

    </script>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="phAjaxManager" runat="server" Visible="false">
        <!-- Ajax panel for communicating with the server  -->
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="upnlAjaxCall" runat="server">
            <ContentTemplate>
                <asp:HiddenField ID="hidParamName" runat="server" />
                <asp:HiddenField ID="hidParamValue1" runat="server" />
                <asp:HiddenField ID="hidParamValue2" runat="server" />
                <asp:HiddenField ID="hidParamValue3" runat="server" />
                <asp:Button ID="cmdMakeAjaxCall" runat="server" Text="" CssClass="hidden" OnCommand="cmdMakeAjaxCall_Command" />
            </ContentTemplate>
        </asp:UpdatePanel>

        <script>
            // call the code-behind to save the current state of the course
             function ajaxCall(paramName, paramValue1, paramValue2, paramValue3) {
                // set the params..
                document.getElementById("<%=hidParamName.ClientID%>").value = paramName;
                document.getElementById("<%=hidParamValue1.ClientID%>").value = paramValue1;
                document.getElementById("<%=hidParamValue2.ClientID%>").value = paramValue2;
                document.getElementById("<%=hidParamValue3.ClientID%>").value = paramValue3;
            
                // click the button!
                document.getElementById("<%=cmdMakeAjaxCall.ClientID%>").click();
            }
        </script>
    </asp:PlaceHolder>

</asp:Content>
