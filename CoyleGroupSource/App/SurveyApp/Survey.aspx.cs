using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SurveyApp
{
    public partial class Survey : System.Web.UI.Page
    {

        #region Member Variables
        // our database class
        private cData m_data;

        // our variables that get set on the cmdline
        private string m_actionref = string.Empty;
        public int m_idref = 0;
        private int m_fkey = 0; // where I need to reference a foreign key (eg: new product for client 101)
        private int m_currentquestionid = 0;    // so we can auto-open the current question on postback
        private bool m_showunansweredonly;
        private WebUser webUser;    //TODO-TODO: get from master page..
        private bool isDebug = false;
        private string m_screenstate = "";

        private string m_prevquestioncategory = string.Empty;
        private bool _isreadonly = false;
        private int m_rowindex = 0;
        private string m_prevclientid = ""; // for client data grid handling
        
        // file uploads
        private const string allowedFileFormats = ".jpg .jpeg .gif .png .tif .tiff .xls .xlsx .pdf .ppt .pptx .txt .doc .docx .csv";

        // alerts
        private const string alertSpan = "<span class=\"text-danger\"><span class=\"glyphicon glyphicon-exclamation-sign\"></span></span>";
        
         
        #endregion

        #region Page Loading  & Startup
        /*
         * ====================================
         *          PAGE LOADING
         * ====================================
         */
        protected void Page_PreLoad()
        {
            // open up the db 
            m_data = new cData();
            if (!m_data.openDB())
            {
                Response.Write("Error opening database. Cannot continue<br><br>" + m_data.lasterrmsg);
                Response.End();
            }
        }

        protected void Page_LoadComplete()
        {
            // cleanup
            m_data.closeDB();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            preInitApp();

            if (!IsPostBack)
                initApp();      // on first load of the page (not a postback), load up our various database bits and bobs..

            updateApp();        // when things get updated (and also on first load), we can enable/disable things depending on what's on screen                
        }

        private void preInitApp()
        {
            // load our command-line every time..
            m_actionref = cTools.cStrExt(Page.RouteData.Values["actionref"]);
            m_idref = cTools.cIntExt(Page.RouteData.Values["id"]);
            m_fkey = cTools.cIntExt(Page.Request.QueryString["fkey"]);
            m_currentquestionid = cTools.cIntExt(Page.Request.QueryString["q"]);
            m_screenstate = getScreenState();

            webUser = cSecurity.getUser();
            isDebug = (cTools.cStrExt(ConfigurationManager.AppSettings["DebugMode"]).ToLower() == "yes");

            saveScreenState("");    //reset for next time..
        }

        private void initApp()
        {
            // run the module now..
            runModule();
        }

        private void updateApp()
        {  
            
        }

        #endregion

        #region Main Page Processing Block
        private void runModule()
        {   
            // do it!
            switch (m_actionref)
            {
                case "clients":
                    loadClientList();
                    break;

                case "list":
                    loadSurveyList(m_idref);
                    break;

                case "add":
                    loadSurveySummary(m_idref, m_fkey);
                    break;

                case "summary":
                    loadSurveySummary(m_idref, m_fkey);
                    break;

                case "qa":
                    if (m_screenstate == "qa-unansweredonly")
                        loadSurveyQandA(m_idref, 0, 1);
                    else
                        loadSurveyQandA(m_idref); 
                    break;


                case "clientapproval":
                    loadClientApproval(m_idref);
                    break;
                    
                case "clientqa":
                    loadClientResponses(m_idref);
                    break;
                    
                case "print":
                    loadSurveySummary(m_idref, m_fkey);
                    loadSurveyQandA(m_idref);
                    if (!isDebug)   // auto-print when not in debug mode
                        cTools.autoPrintPage();
                    break;
            }
        }

        #endregion

        #region Client Listing
        private void loadClientList()
        {
            lblPageTitle.Text = "Clients";

            GridView1.RowDataBound += new GridViewRowEventHandler(handleDataBind_ClientListing);
            string sqlCmd = "spGetClientList "; // note the extra space to handle the userid param
            if (webUser.isadmin || webUser.issuperadmin)
                sqlCmd += "null";  // admin users see all clients
            else
                sqlCmd += webUser.userid;
            m_data.bindToGrid(sqlCmd, GridView1);

            cmdAddNewClient.Visible = webUser.issuperadmin;
            pnlTitleAndAddNewBtn.Visible = cmdAddNewClient.Visible;
            pnlGrid.Visible = true;
        }
        protected void cmdAddNewClient_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/admin/clientedit/");
        }

        private void handleDataBind_ClientListing(object sender, GridViewRowEventArgs e)
        {
            int colClientID = 0;
            int colClientName = 1;
            int colLocationID = 2;
            int colNumSurveys = 5;
            int colButtonsPlaceholder = 6;

            string repeatPrevLine = "&nbsp;&nbsp;&nbsp; '' &nbsp;&nbsp;&nbsp; '' &nbsp;&nbsp;&nbsp; ''";

            // hide id's in the grid 
            e.Row.Cells[colClientID].Visible = false;
            e.Row.Cells[colLocationID].Visible = false;
            e.Row.Cells[colNumSurveys].Visible = false;
            
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    //reset these for the batch of clients..
                    m_prevclientid = "";
                    break;


                case DataControlRowType.DataRow:

                    // don't repeat client names
                    if (m_prevclientid == e.Row.Cells[colClientID].Text)
                        e.Row.Cells[colClientName].Text = repeatPrevLine;

                    // create buttons
                    if (webUser.isadmin || webUser.issuperadmin)
                    {
                        // I want the buttons stop going to a second line
                        e.Row.Cells[colButtonsPlaceholder].Width = 320;

                        // admin user
                        e.Row.Cells[colButtonsPlaceholder].Text = String.Format(
                            "<a class='btn btn-primary btn-xs {3}' href='{6}'>Audits <span class='badge {8}'>{7}</span></a>" +
                            "&nbsp;&nbsp;&nbsp;" +
                            "<a class='btn btn-default btn-xs {3}' href='{0}'>Edit Site</a>" +
                            "&nbsp;&nbsp;&nbsp;" +
                            "<a class='btn btn-default btn-xs {5}' href='{1}'>New Site</a>" +
                            "&nbsp;&nbsp;&nbsp;" +
                            "<a class='btn btn-default btn-xs {4} {5}' href='{2}'>Edit Client</a>",
                            Page.ResolveUrl("~/admin/locationedit/") + e.Row.Cells[colLocationID].Text,
                            Page.ResolveUrl("~/admin/locationedit/0?fkey=") + e.Row.Cells[colClientID].Text,
                            Page.ResolveUrl("~/admin/clientedit/") + e.Row.Cells[colClientID].Text,
                            cTools.iif(e.Row.Cells[colLocationID].Text == "&nbsp;", "disabled"),
                            cTools.iif(false, "disabled"),
                            cTools.iif(m_prevclientid == e.Row.Cells[colClientID].Text, "hidden"),
                            Page.ResolveUrl("~/audit/list/") + e.Row.Cells[colLocationID].Text,
                            e.Row.Cells[colNumSurveys].Text,
                            cTools.iif(e.Row.Cells[colNumSurveys].Text == "0", "hidden"));
                    }
                    else
                    {
                        // standard user..
                        e.Row.Cells[colButtonsPlaceholder].Text = String.Format(
                            "<a class='btn btn-primary btn-xs {2}' href='{0}'>Audits <span class='badge'>{1}</span></a>",
                            Page.ResolveUrl("~/audit/list/") + e.Row.Cells[colLocationID].Text,
                            e.Row.Cells[colNumSurveys].Text,
                            cTools.iif(webUser.isleader && e.Row.Cells[colNumSurveys].Text == "0", "disabled"));
                    }
                    // remember this client for next row..
                    m_prevclientid = e.Row.Cells[colClientID].Text;
                    break;
            }
        }

        #endregion

        #region Survey Management
        
        private void loadSurveyList(int locationId)
        {
            // load up the location & client info..
            LocationData location = m_data.loadLocation(locationId);
            ClientData client = m_data.loadClient(location.clientid);
            lblPageTitle.Text = client.clientname + " - " + location.businessunit + " - " + location.locationname;
        
            // load up the list of surveys for this location
            GridView1.RowDataBound += new GridViewRowEventHandler(handleDataBind_SurveyListing);
            
            string statusIdParam = "null";   // see all audits regardless of status..
            string sqlCmd = "spGetSurveyList " + locationId + ", " + statusIdParam;
            m_data.bindToGrid(sqlCmd, GridView1);
            pnlGrid.Visible = true;

            // any user (except clients) can create an audit
            cmdAddNewSurvey.Visible = (webUser.isadmin || webUser.issuperadmin || webUser.isnewhire);
            pnlTitleAndAddNewBtn.Visible = true;
        }
        private void handleDataBind_SurveyListing(object sender, GridViewRowEventArgs e)
        {
            int colId = 0;
            int colAuditDate = 1;
            int colClientName = 2;
            int colLocationId = 3;
            int colBusinessUnit = 4;
            int colLocationName = 5;
            //int colAuditType = 6;
            int colStatusId = 8;
            //int colStatusName = 9;
            //int colLastModifiedDate = 10;
            int colClientReviewDate = 11;
            int colButtonsPlaceholder = 12;

            // hide id's in the grid 
            e.Row.Cells[colId].Visible = false;
            e.Row.Cells[colClientName].Visible = false;
            e.Row.Cells[colLocationId].Visible = false;
            e.Row.Cells[colBusinessUnit].Visible = false;
            e.Row.Cells[colLocationName].Visible = false;
            e.Row.Cells[colStatusId].Visible = false;
            e.Row.Cells[colClientReviewDate].Visible = false;

            // clients only see non-draft audits

            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    break;

                case DataControlRowType.DataRow:
                    int statusId = cTools.cIntExt(e.Row.Cells[colStatusId].Text);

                    // see how many days have passed since the audit was sent for client review
                    //   oh, and see if it's still in that state..
                    if (statusId == cConstants.statusForClientReview)
                    {
                        string clientReviewDate = e.Row.Cells[colClientReviewDate].Text;
                        TimeSpan ts = Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(clientReviewDate);
                        int numDaysSinceClientReview = cTools.cIntExt(ts.TotalDays); // Will return the difference in Days
                        if (numDaysSinceClientReview > 7)
                        {
                            e.Row.CssClass = "danger";
                            e.Row.Cells[colAuditDate].Text = alertSpan + "&nbsp;" + e.Row.Cells[colAuditDate].Text;
                        }
                    }

                    // create buttons
                    string gridButtons = string.Empty;

                    if (webUser.isadmin || webUser.issuperadmin || webUser.isnewhire)
                    {
                        gridButtons = String.Format(
                            "<a class='btn btn-primary btn-xs' href='{0}'>Open</a>",
                            Page.ResolveUrl("~/audit/summary/") + e.Row.Cells[colId].Text);
                    }
                    else if (webUser.isleader)
                    {  // CLIENTS
                        // clients can't see drafts..
                        if (statusId == cConstants.statusDraft || statusId == cConstants.statusForInternalReview)
                            e.Row.Visible = false;

                        gridButtons = String.Format(
                            "<a class='btn btn-primary btn-xs' href='{0}'>Open</a>",
                            Page.ResolveUrl("~/audit/qa/") + e.Row.Cells[colId].Text);
                    }

                    // add the print button
                    gridButtons += String.Format(
                        "&nbsp;&nbsp;&nbsp" + 
                        "<a class='btn btn-warning btn-xs' href='{0}'>Print</a>",
                        Page.ResolveUrl("~/audit/print/") + e.Row.Cells[colId].Text);

                    e.Row.Cells[colButtonsPlaceholder].Text = gridButtons;
                    break;
            }
        }

        private void loadSurveySummary(int surveyId, int defaultLocationId = 0)
        {
            SurveySummaryData survey;

            if (surveyId != 0)
            {
                survey = m_data.loadSurvey(surveyId);
            }
            else
            {
                survey = new SurveySummaryData();
                survey.locationid = defaultLocationId;
                survey.dateofaudit = cTools.cDateToDateTimePickerFormat(DateTime.Now.ToString());
                if (!webUser.isadmin && !webUser.issuperadmin)
                    survey.auditorid = webUser.userid;  // set it to me - on a NEW audit only !
                survey.statusid = cConstants.statusDraft;
                
                cmdDeleteSurvey.Enabled = false;
            }

            loadSurveyTitles(survey);
            
            // determine if the screen is editable..
            bool isEditableForm = false;
            if (webUser.isadmin || webUser.issuperadmin)
                isEditableForm = true;  //  always editable by admins 
            else if (webUser.userid == survey.auditorid && survey.statusid == cConstants.statusDraft)
                isEditableForm = true;  // editable by the auditor but only if in draft mode

            // disable all inputs and hide all buttons when printing
            if (m_actionref == "print")
                isEditableForm = false;
            
            // put the details of this document up on the screen for editing..
            hidLocationId.Value = survey.locationid.ToString();
            txtDateOfAudit.Text = survey.dateofaudit;
            txtClientContact.Text = survey.clientcontact;
            txtSiteDesc.Text = survey.sitedesc;
            txtScopeOfWork.Text = survey.scopeofwork;
            txtWeatherConditions.Text = survey.weatherconditions;
            txtSummary.Text = survey.summary;

            // print-only versions for showing scrollable text on printed pages..
            lblSummary.Text = cTools.cStrExt(survey.summary).Replace("\n", "<br />");
            lblScopeOfWork.Text = cTools.cStrExt(survey.scopeofwork).Replace("\n", "<br />");
            lblSiteDesc.Text = cTools.cStrExt(survey.sitedesc).Replace("\n", "<br />");

            // load list of templates
            string sqlCmd = "SELECT templatename, templateid FROM templates WHERE deleted=0 ORDER BY templatename ASC";
            m_data.createDropdown(sqlCmd, cboSurveyTemplate, "= Please Select =", "0"); // no other options
            cTools.autoSelectListItem(cboSurveyTemplate, survey.templateid.ToString());
            
            // load status dropdown..
            sqlCmd = "SELECT statusname, statusid FROM status ORDER BY sortorder ASC";
            m_data.createDropdown(sqlCmd, cboStatus); 
            cTools.autoSelectListItem(cboStatus, survey.statusid.ToString());

            // load list of auditors
            // if you're an admin, you get everyone, if you're an auditor, you see only your own name
            sqlCmd = "SELECT " + 
                " u.firstname + ' ' + u.lastname, u.userid " + 
                " FROM users AS u, users_locations AS ul " + 
                " WHERE u.userid = ul.userid AND ul.locationid="+ survey.locationid.ToString();
            if (!webUser.isadmin && !webUser.issuperadmin)
            {
                // if the form's not editable then leave the auditor list in place
                if (isEditableForm) // it's an editable form..so chances are you're the auditor (cos you're defo not admin)
                    sqlCmd += " AND u.userid=" + webUser.userid;
            }
            m_data.createDropdown(sqlCmd, cboAuditor, "= Please Select =", "0"); // no other options
            cTools.autoSelectListItem(cboAuditor, survey.auditorid.ToString());

            // client assigment..load list of users who are "clients" at this client's sites
            LocationData location = m_data.loadLocation(survey.locationid);
            ClientData client = m_data.loadClient(location.clientid);
            sqlCmd = "SELECT DISTINCT " +
                "    u.firstname + ' ' + u.lastname, u.userid " +
                " FROM users AS u, users_locations AS ul, locations AS l " +
                " WHERE u.userid = ul.userid " +
                " AND ul.locationid = l.locationid " +
                " AND u.isleader=1 " + 
                " AND l.clientid = " + client.clientid.ToString();
            m_data.createDropdown(sqlCmd, cboClientMgr, "= Please Select =", "0"); // no other options
            cTools.autoSelectListItem(cboClientMgr, survey.client_mgruserid.ToString());

            m_data.createDropdown(sqlCmd, cboClientAdmin, "= Please Select =", "0"); // no other options
            cTools.autoSelectListItem(cboClientAdmin, survey.client_adminuserid.ToString());
            
            // show the form
            pnlSummary.Visible = true;

            if (isEditableForm)
            {
                // set input focus..
                if (surveyId == 0)  // new 
                    cboSurveyTemplate.Focus();
                else                // existing..
                    txtScopeOfWork.Focus();

                // only admins can change status
                cboStatus.Enabled = (webUser.isadmin || webUser.issuperadmin);

                // choose which buttons to show...
                pnlSummarySaveButtons.Visible = true;
                pnlSummaryCloseButtons.Visible = false;
            }
            else
            {
                // disable all inputs 
                litSummaryFieldset.Text = "disabled";

                // choose which buttons to show...
                pnlSummarySaveButtons.Visible = false;
                pnlSummaryCloseButtons.Visible = true;
            }

            // some extra clean-up when we're using this for summary display-only
            //  I show this on the Q&A for clients so I don't need the second CLOSE button
            if (m_actionref == "print" || webUser.isleader) {
                pnlSummaryCloseButtons.Visible = false;
            }

            configQuickNavbar();
        }

        private void saveSummary()
        {
            // for later comparison purposes...
            SurveySummaryData prevSurveyData;
            if (m_idref != 0)
                prevSurveyData = m_data.loadSurvey(m_idref);
            else
                prevSurveyData = new SurveySummaryData();

            // save changes now..
            SurveySummaryData survey = new SurveySummaryData();
            survey.surveyid = m_idref;
            survey.templateid = cTools.cIntExt(cboSurveyTemplate.SelectedValue);
            survey.locationid = cTools.cIntExt(hidLocationId.Value);
            survey.auditorid = cTools.cIntExt(cboAuditor.SelectedValue);
            survey.dateofaudit = txtDateOfAudit.Text;
            survey.clientcontact = txtClientContact.Text;
            survey.sitedesc = txtSiteDesc.Text;
            survey.scopeofwork = txtScopeOfWork.Text;
            survey.weatherconditions = txtWeatherConditions.Text;
            survey.statusid= cTools.cIntExt(cboStatus.SelectedValue);
            survey.summary = txtSummary.Text;
            survey.client_mgruserid = cTools.cIntExt(cboClientMgr.SelectedValue);
            survey.client_adminuserid = cTools.cIntExt(cboClientAdmin.SelectedValue);

            int newId = m_data.saveSurvey(survey);

            // audit trail 
            if (m_idref == 0)
                m_data.auditEvent(cConstants.eventSurveySummaryNew, newId);
            else
                m_data.auditEvent(cConstants.eventSurveySummaryEdit, newId);

            // push notifications..
            switch (survey.statusid)
            {
                case cConstants.statusDraft:    // draft mode - see if the consultant has changed..
                    if (survey.auditorid != prevSurveyData.auditorid)
                    {
                        // send out the push notification
                        cPushNotifications.handleEvent(cConstants.eventSurveyConsultantChanged_Notification, newId);

                        // tell the user that this has been done
                        cMsgBox.saveUserDialog("The assigned consultant has been notified");
                        
                    }
                    break;
                    
                case cConstants.statusForClientReview:    // for client review - see if this is a changed setting..
                    if (survey.statusid != prevSurveyData.statusid) //status just changed to client review..
                    {
                        // make sure that the client hasn't already approved this audit tho..
                        if (survey.client_approvalstatusid != cConstants.eventClientApprovalAccept)
                        {
                            cPushNotifications.handleEvent(cConstants.eventSurveyStatusReadyForClientReview_Notification, newId);

                            // tell the user that this has been done
                            cMsgBox.saveUserDialog("The assigned client manager has been notified");
                        }
                    }
                    break;
            }

            if (newId != 0)
                Response.Redirect("~/home");
            
        }
        private void deleteSurvey()
        {
            m_data.deleteSurvey(m_idref);

            //audit trail
            m_data.auditEvent(cConstants.eventSurveyDelete, m_idref);

            int locationId = cTools.cIntExt(hidLocationId.Value);
            Response.Redirect("~/audit/list/" + locationId);
        }
       
       
        protected void cmdSaveSummary_Command(object sender, CommandEventArgs e)
        {
            saveSummary();
        }

        protected void cmdCancelSummary_Command(object sender, CommandEventArgs e)
        {
            int locationId = cTools.cIntExt(hidLocationId.Value);
            Response.Redirect("~/audit/list/" + locationId);
        }

        protected void cmdDeleteSurvey_Command(object sender, CommandEventArgs e)
        {
            deleteSurvey();
        }
        
        protected void cmdAddNewSurvey_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/audit/add/0?fkey=" + m_idref);
        }
        protected void cmdCloseSummary_Command(object sender, CommandEventArgs e)
        {
            int locationId = cTools.cIntExt(hidLocationId.Value);
            Response.Redirect("~/audit/list/" + locationId);
        }
        #endregion

        #region Survey Q & A 

        private void loadSurveyQandA(int surveyId, int restrictToAnswerId = 0, int showUnansweredItemsOnly = 0)
        {
            // load up the survey
            SurveySummaryData survey = m_data.loadSurvey(surveyId);

            m_showunansweredonly = (showUnansweredItemsOnly == 1);

            loadSurveyTitles(survey);

            // determine if the screen is editable..
            _isreadonly = !(
                (webUser.isadmin || webUser.issuperadmin)
                ||
                (webUser.userid == survey.auditorid && survey.statusid == cConstants.statusDraft)
                );
            
            // disable all inputs and hide all buttons when printing
            if (m_actionref == "print")
            {
                _isreadonly = true;
                pnlExpandButtons.Visible = false;
                cmdSubmitToHQ.Visible = false;
                
            } 
            
            // now go and load up all of the questions and answers..
            rptQandA.ItemDataBound += new RepeaterItemEventHandler(handleDataBind_QandARepeater);
            m_data.runSqlRepeater("spGetSurveyAnswers " + surveyId + ", " + restrictToAnswerId + ", " + showUnansweredItemsOnly, rptQandA); // 1 = New Hire review questions
            
            cmdSubmitToHQ.Enabled = !_isreadonly;
            pnlQandA.Visible = true;
            phAjaxManager.Visible = true;   // enable auto-saving of answers as they're ticked in real-time
            phFileUploadScripts.Visible = true; // need scripts to manage the file uploads
            phFileUploadDialog.Visible = true;
            configQuickNavbar();
        }
        
        private void saveSurveyAnswer(int questionID, int answerID, int classificationId, string answerText)
        {
            int surveyId = m_idref;
            
            // this saves a single answer - it will handle both the tickboxes and answer texts (optionally both)
            int idCheck = m_data.saveSurveyAnswer(surveyId, cSecurity.userID(), questionID, answerID, classificationId, answerText);

            //audit trail
            string auditDescription = m_data.getQuestion(questionID) + "\n";
            if (answerID != 0)
                auditDescription += "Answer: " + m_data.getAnswer(answerID);
            if (classificationId != 0)
                auditDescription += "Answer: " + m_data.getClassification(classificationId);
            if (answerText != "null")
                auditDescription += "Observations/Recommendations: " + answerText;
            m_data.auditEvent(cConstants.eventQuestionSave, surveyId, auditDescription); 


            if (idCheck == 0)
                cMsgBox.showSaveErrorMsg();
        }
        
        protected void handleDataBind_QandARepeater(object sender, RepeaterItemEventArgs e)
        {
            // only interested in the list items..
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            // get some data I'll need later..
            string questionID = cTools.cStrExt(cRepeater.getDataVal(e, "questionid"));
            int questionType = cTools.cIntExt(cRepeater.getDataVal(e, "questiontype"));
            int answerID = cTools.cIntExt(cRepeater.getDataVal(e, "answerid"));
            string questionCategory = cTools.cStrExt(cRepeater.getDataVal(e, "questioncatname"));
            int classificationId = cTools.cIntExt(cRepeater.getDataVal(e, "classificationid"));
            string questionText = cTools.cStrExt(cRepeater.getDataVal(e, "questiontext"));
            int catNum = cTools.cIntExt(cRepeater.getDataVal(e, "catnum"));
            int questionNum = cTools.cIntExt(cRepeater.getDataVal(e, "questionnum"));
            
            /*
                Show the category when it changes..
            */
            if (questionCategory != m_prevquestioncategory)
            {
                // show the category heading..
                cRepeater.setPlaceholderVisibility(e, "phCategoryHeading", true);

                // show the correct answers for this question type..
                switch (questionType)
                {
                    case cConstants.questionTypeYesNoNA:
                        // Yes, No, N/A
                        cRepeater.setPlaceholderVisibility(e, "phYesNoNAHeading", true);
                        break;

                    case cConstants.questionTypeAgreeDisagree:
                        // strongly agree .. stronly disagree
                        cRepeater.setPlaceholderVisibility(e, "phAgreeDisagreeHeading", true);
                        break;

                    case cConstants.questionTypeTextBox:
                        // textbox has a blank heading..
                        cRepeater.setPlaceholderVisibility(e, "phTextAnswerHeading", true);
                        break;
                }

                m_prevquestioncategory = questionCategory;
            }

            // automatically add numbers to the categories & questions..
            cRepeater.setLabelVal(e, "lblQuestionCategory", catNum + ". " + questionCategory);
            cRepeater.setLabelVal(e, "lblQuestionTitle", "<b>" + catNum + "." + questionNum + "</b> " + questionText);

            /*
                Load up and show the previously entered answer for this question
            */
            if (answerID != 0)
            {
                // turn on the tickbox for the required answer & populate any textboxes
                cRepeater.setItemLabelStyle(e.Item, "lblAnswer" + answerID, cConstants.cssTicked); // this ticks the previously ticked answer

                // show the status lights - red orange green
                string statusIcon = "";
                string statusColor = "";
                switch (questionType)
                {
                    case cConstants.questionTypeYesNoNA:
                        switch (answerID)
                        {
                            case cConstants.answerIdYes:
                                statusColor = cConstants.cssStatusOK_Colour;
                                statusIcon = cConstants.cssStatusOK_Icon;
                                break;
                            case cConstants.answerIdNo:
                                statusColor = cConstants.cssStatusDanger_Colour;
                                statusIcon = cConstants.cssStatusDanger_Icon;
                                break;
                            case cConstants.answerIdNA:
                                statusColor = cConstants.cssStatusWarning_Colour;
                                statusIcon = cConstants.cssStatusWarning_Icon;
                                break;
                        }

                        break;
                }
                cRepeater.setPanelStyle(e, "pnlStatus", statusColor);
                cRepeater.setItemLabelStyle(e.Item, "lblStatus", statusIcon);
            }
            cRepeater.setTextbox(e, "txtObservations", "observations");
            
            // classification dropdown
            string sqlCmd = "SELECT classificationtitle, classificationid FROM classifications WHERE deleted=0 ORDER BY classificationid ASC";
            cRepeater.setDropdown(e, "cboClassification", "classificationid", sqlCmd, m_data, "= Please Select =", "0");



            /*
                Setup click handlers on the client-side so that when a box is clicked, it ticks the item
                    and sends an Ajax request to the server to save without jumping the screen around the place
            */
            switch (questionType)
            {
                case cConstants.questionTypeYesNoNA:
                    // show the tickboxes for the Yes, No, N/A answers
                    cRepeater.setPlaceholderVisibility(e, "phYesNoNAAnswers", true);

                    // set up the event handlers for the tick (client-side)
                    HyperLink hrefTick6 = (HyperLink)(e.Item.FindControl("hrefTick6"));
                    HyperLink hrefTick7 = (HyperLink)(e.Item.FindControl("hrefTick7"));
                    HyperLink hrefTick8 = (HyperLink)(e.Item.FindControl("hrefTick8"));

                    string lblAnswer6 = ((Label)(e.Item.FindControl("lblAnswer6"))).ClientID;
                    string lblAnswer7 = ((Label)(e.Item.FindControl("lblAnswer7"))).ClientID;
                    string lblAnswer8 = ((Label)(e.Item.FindControl("lblAnswer8"))).ClientID;

                    if (!_isreadonly)
                    {
                        hrefTick6.Attributes.Add("onclick",
                            "tickMe(" + questionID + ", 6, " + m_rowindex + ", " + questionType + "); return (false);"
                        );
                        hrefTick6.Attributes.Add("style", "cursor:pointer;");

                        hrefTick7.Attributes.Add("onclick",
                            "tickMe(" + questionID + ", 7, " + m_rowindex + ", " + questionType + "); return (false);"
                            );
                        hrefTick7.Attributes.Add("style", "cursor:pointer;");

                        hrefTick8.Attributes.Add("onclick",
                            "tickMe(" + questionID + ", 8, " + m_rowindex + ", " + questionType + "); return (false);"
                            );
                        hrefTick8.Attributes.Add("style", "cursor:pointer;");
                    }
                    break;

                case cConstants.questionTypeAgreeDisagree:
                    // show the tickboxes for the strongly agree -> stronly disagree
                    cRepeater.setPlaceholderVisibility(e, "phAgreeDisagreeAnswers", true);

                    // set up the event handlers for the tick (client-side)
                    HyperLink hrefTick1 = (HyperLink)(e.Item.FindControl("hrefTick1"));
                    HyperLink hrefTick2 = (HyperLink)(e.Item.FindControl("hrefTick2"));
                    HyperLink hrefTick3 = (HyperLink)(e.Item.FindControl("hrefTick3"));
                    HyperLink hrefTick4 = (HyperLink)(e.Item.FindControl("hrefTick4"));
                    HyperLink hrefTick5 = (HyperLink)(e.Item.FindControl("hrefTick5"));

                    string lblAnswer1 = ((Label)(e.Item.FindControl("lblAnswer1"))).ClientID;
                    string lblAnswer2 = ((Label)(e.Item.FindControl("lblAnswer2"))).ClientID;
                    string lblAnswer3 = ((Label)(e.Item.FindControl("lblAnswer3"))).ClientID;
                    string lblAnswer4 = ((Label)(e.Item.FindControl("lblAnswer4"))).ClientID;
                    string lblAnswer5 = ((Label)(e.Item.FindControl("lblAnswer5"))).ClientID;

                    if (!_isreadonly)
                    {
                        hrefTick1.Attributes.Add("onclick",
                            "tickMe(" + questionID + ", 1, " + m_rowindex + ", " + questionType + "); return (false);"
                        );
                        hrefTick1.Attributes.Add("style", "cursor:pointer;");

                        hrefTick2.Attributes.Add("onclick",
                            "tickMe(" + questionID + ", 2, " + m_rowindex + ", " + questionType + "); return (false);"
                            );
                        hrefTick2.Attributes.Add("style", "cursor:pointer;");

                        hrefTick3.Attributes.Add("onclick",
                            "tickMe(" + questionID + ", 3, " + m_rowindex + ", " + questionType + "); return (false);"
                            );
                        hrefTick3.Attributes.Add("style", "cursor:pointer;");

                        hrefTick4.Attributes.Add("onclick",
                            "tickMe(" + questionID + ", 4, " + m_rowindex + ", " + questionType + "); return (false);"
                            );
                        hrefTick4.Attributes.Add("style", "cursor:pointer;");

                        hrefTick5.Attributes.Add("onclick",
                            "tickMe(" + questionID + ", 5, " + m_rowindex + ", " + questionType + "); return (false);"
                            );
                        hrefTick5.Attributes.Add("style", "cursor:pointer;");
                    }
                    break;
            }

            // set up the auto-save when the content has changed - "Observations" (trigged on losefocus)
            TextBox txtObservations = (TextBox)(e.Item.FindControl("txtObservations"));
            if (!_isreadonly)
            {
                txtObservations.Attributes.Add("onchange",
                "saveObservations(" + questionID + ",'" + txtObservations.ClientID + "');"
                );
            }

            DropDownList cboClassification = (DropDownList)(e.Item.FindControl("cboClassification"));
            if (!_isreadonly)
            {
                cboClassification.Attributes.Add("onchange",
                "saveClassification(" + questionID + ",'" + cboClassification.ClientID + "');"
                );
            }

            // set up actions for the "More" button on each row
            HyperLink hrefMore = (HyperLink)(e.Item.FindControl("hrefMore"));
            hrefMore.Attributes.Add("onclick",
                "toggleMore(" + m_rowindex + "); return (false);"
            );
            hrefMore.Attributes.Add("style", "cursor:pointer;");
            
            // set up the ADD (file uploads) button
            HyperLink hrefAddFile = (HyperLink)(e.Item.FindControl("hrefAddFile"));
            if (!_isreadonly)
            {
                hrefAddFile.Attributes.Add("onclick",
                    "uploadfile(" + questionID + "); return (false);"
                );
                hrefAddFile.Attributes.Add("style", "cursor:pointer;");
            } 
            
            // list all of the file uploads for this question...
            int sourceId = cConstants.sourceAuditor;
            Repeater rptQuestionFileUploads = (Repeater)(e.Item.FindControl("rptQuestionFileUploads"));
            rptQuestionFileUploads.ItemDataBound += new RepeaterItemEventHandler(handleDataBind_QuestionFileUploads);
            m_data.runSqlRepeater("spGetFileUploadsList " + m_idref + ", " + questionID + ", " + sourceId, rptQuestionFileUploads); 
            
            // enable/disable buttons depending on locked status
            txtObservations.Enabled = !_isreadonly;
            cboClassification.Enabled = !_isreadonly;

            // let's get access to the "more info" rows..
            //  we need to give the div a name and a style
            Literal litMoreInfo = (Literal)(e.Item.FindControl("litMoreInfo"));
            litMoreInfo.Text = "id='rowMoreInfo_" + m_rowindex+ "'";
            bool hideMoreInfoRows=true;

            // disable all inputs and hide all buttons when printing
            hrefAddFile.Visible = !_isreadonly;
            
            if (m_actionref == "print")
            {
                hrefMore.Visible = false;
                hrefAddFile.Visible = false;
                hideMoreInfoRows = false;
                
                // don't show empty attachments row..
                if (rptQuestionFileUploads.Items.Count == 0)
                    cRepeater.setPanelVisibility(e, "pnlFileUploads", false);
                
            }

            // when printing, textareas don't show all of the content so hide them and use labels instead 
            //  we have to do this on loadup - also on change of the textbox too in case they change something 
            //  then print from the edit screen
            cRepeater.setLabelBr(e, "lblObservations", "observations");

            // now hide all "more info" rows until user chooses to see them..
            //   don't auto-hide when showing only unanswered q's
            if (hideMoreInfoRows && questionID != m_currentquestionid.ToString() && !m_showunansweredonly)
                litMoreInfo.Text += " style='display:none'";

            m_rowindex++;   // keep a count of the rows as we need them to target the tickboxes
        }

        // data bind on the files list..
        protected void handleDataBind_QuestionFileUploads(object sender, RepeaterItemEventArgs e)
        {
            // only interested in the list items..
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            string uploadsUrl = cTools.cStrExt(ConfigurationManager.AppSettings["UploadsURL"]);
            string serverFilename = cTools.cStrExt(cRepeater.getDataVal(e, "serverfilename"));

            // set up the ADD (file uploads) button
            HyperLink hrefViewUploadFile = (HyperLink)(e.Item.FindControl("hrefViewUploadFile"));
            hrefViewUploadFile.NavigateUrl = uploadsUrl + "/" + m_idref.ToString() + "/" + serverFilename;
            hrefViewUploadFile.Target = "_blank";
            hrefViewUploadFile.Attributes.Add("style", "cursor:pointer;");

            // hide any buttons that we either don't have access to or don't want printed
            if (_isreadonly || m_actionref == "print")
                cRepeater.setPlaceholderVisibility(e, "phRemoveFile", false);
            
            // when printing, actually show a preview of the image..
            if (m_actionref == "print")
            {
                // but only if the file is an image!
                if (serverFilename.ToLower().EndsWith(".jpg") || serverFilename.ToLower().EndsWith(".jpeg") || serverFilename.ToLower().EndsWith(".png") || serverFilename.ToLower().EndsWith(".gif")) {
                    Image imgFilePreview = (Image)(e.Item.FindControl("imgFilePreview"));
                    imgFilePreview.ImageUrl = uploadsUrl + "/" + m_idref.ToString() + "/" + serverFilename;
                }
            }
                
        }

        protected void cmdSubmitToHQ_Command(object sender, CommandEventArgs e)
        {
            int surveyId = m_idref;
            int numQuestionsUnanswered = m_data.getNumQuestionsUnanswered(surveyId);
            
            // if there are unanswered questions then we can't sent to hq..
            if (numQuestionsUnanswered == 0)
            {
                string sqlCmd = "UPDATE surveys SET statusid=" + cConstants.statusForInternalReview + " WHERE surveyid=" + surveyId;
                m_data.execSql(sqlCmd);

                // audit trail..
                m_data.auditEvent(cConstants.eventQuestionSubmitToHQ, surveyId);

                // tell the user that this is now sorted..
                cMsgBox.saveUserDialog("The survey has now been sent to HQ for processing");

                // send out an email to Coyle Group that this is ready now..
                cPushNotifications.handleEvent(cConstants.eventQuestionSubmitToHQ_Notification, surveyId);

                // go back to the list of audits for this site
                Response.Redirect("~/home");
            }
            else
            {
                
                string userMsg = string.Format(
                    "You still have {0} unanswered questions.<br><br>" + 
                    "Please complete all questions before sending this survey to HQ.<br><br>" + 
                    "This screen will now show the {0} unanswered questions to help you to complete this audit.",
                    numQuestionsUnanswered);

                // audit trail..
                m_data.auditEvent(cConstants.eventQuestionSubmitToHQFail, surveyId, userMsg.Replace("<br>", "\n"));


                cMsgBox.saveUserDialog(userMsg);
                saveScreenState("qa-unansweredonly");
                Response.Redirect("~/audit/qa/" + m_idref); //this page!
            }
            
        }

        protected void lnkShowAllYesItems_Click(object sender, EventArgs e)
        {
            loadSurveyQandA(m_idref, cConstants.answerIdYes);
        }
        
        protected void lnkShowAllNoItems_Click(object sender, EventArgs e)
        {
            loadSurveyQandA(m_idref, cConstants.answerIdNo);
        }

        protected void lnkShowAllNAItems_Click(object sender, EventArgs e)
        {
            loadSurveyQandA(m_idref, cConstants.answerIdNA);
        }

        protected void lnkShowAllUnansweredItems_Click(object sender, EventArgs e)
        {
            loadSurveyQandA(m_idref, 0, 1);
        }

        protected void lnkShowAllItems_Click(object sender, EventArgs e)
        {
            loadSurveyQandA(m_idref);
        }
        #endregion
        
        #region Client Approval
        private void loadClientApproval(int surveyId)
        {
            // load up the survey
            SurveySummaryData survey = m_data.loadSurvey(surveyId);
            loadSurveyTitles(survey);

            txtClientApprovalComments.Text = survey.client_approvalcomments;

            // load status dropdown..
            string sqlCmd = "SELECT approvalstatus, approvalstatusid FROM approvalstatus ORDER BY approvalstatusid ASC";
            m_data.createDropdown(sqlCmd, cboClientApprovalStatus);
            cTools.autoSelectListItem(cboClientApprovalStatus, survey.client_approvalstatusid.ToString());

            // determine if the screen is editable..
            // if it's for client review and I've been assigned as the client manager
            _isreadonly = !(
                (webUser.isadmin || webUser.issuperadmin)
                ||
                (survey.statusid == cConstants.statusForClientReview && webUser.userid == survey.client_mgruserid)
            );

            // disable all inputs 
            if (_isreadonly)
                litClientApprovalFieldset.Text = "disabled";

            // approval status dropdown is read-only as the buttons control the selection
            cboClientApprovalStatus.Enabled = false;
            pnlClientApproval.Visible = true;
            configQuickNavbar();
        }

        private void saveClientApproval()
        {
            SurveySummaryData survey = m_data.loadSurvey(m_idref);

            // we use the full SurveySummaryData class but we only save a couple of items
            survey.client_approvalstatusid = cTools.cIntExt(cboClientApprovalStatus.SelectedValue);
            survey.client_approvalcomments= txtClientApprovalComments.Text;
            
            int newId = m_data.saveSurveyClientApproval(survey);
            if (newId != 0)
            {
                // if approved, go straight to the client qa screen
                // if rejected, go back to home page..    
                switch (survey.client_approvalstatusid) {
                    case cConstants.approvalStatus_Approved:
                        // if it's approved, automatically set the status of the audit to "Released"
                        survey.statusid = cConstants.statusReleased;
                        m_data.saveSurvey(survey);  // save the change of status to the db

                        // audit trail..
                        m_data.auditEvent(cConstants.eventClientApprovalAccept, survey.surveyid, survey.client_approvalcomments);

                        // send a notification to the coyle group
                        cPushNotifications.handleEvent(cConstants.eventClientApprovalAccept_Notification, newId);
                        
                        // now go on to the client QA listing..
                        //  auto-show the message that there are items pending.
                        HttpContext.Current.Session["ShowRecentApproval"] = "Y";
                        Response.Redirect("~/audit/clientqa/" + survey.surveyid);
                        break;

                    case cConstants.approvalStatus_Rejected:
                        // reset the status to "for internal review"
                        survey.statusid = cConstants.statusForInternalReview;
                        m_data.saveSurvey(survey);  // save the change of status to the db

                        // audit trail..
                        m_data.auditEvent(cConstants.eventClientApprovalReject, survey.surveyid, survey.client_approvalcomments);

                        // send a notification to the coyle group
                        cPushNotifications.handleEvent(cConstants.eventClientApprovalReject_Notification, newId);
                        
                        // let the user know we have processed their action..
                        cMsgBox.saveUserDialog(cConstants.msgAuditRejected);

                        Response.Redirect("~/home");
                        break;
                }
            }
        }
        protected void cmdClientAccept_Command(object sender, CommandEventArgs e)
        {
            cTools.autoSelectListItem(cboClientApprovalStatus, cConstants.approvalStatus_Approved.ToString());
            saveClientApproval();
        }

        protected void cmdClientReject_Command(object sender, CommandEventArgs e)
        {
            cTools.autoSelectListItem(cboClientApprovalStatus, cConstants.approvalStatus_Rejected.ToString());
            saveClientApproval();
        }
        #endregion

        #region Client Responses Q & A

        private void loadClientResponses(int surveyId, bool showClosedItemsToo = false)
        {
            // load up the survey
            SurveySummaryData survey = m_data.loadSurvey(surveyId);
            loadSurveyTitles(survey);

            // determine if the screen is editable..
            _isreadonly = !(
                (webUser.isadmin || webUser.issuperadmin)
                ||
                (
                    (survey.statusid == cConstants.statusReleased)
                    &&
                    (webUser.userid == survey.client_mgruserid || webUser.userid == survey.client_adminuserid)
                )
            );

            int showClosedItems = 0;
            if (showClosedItemsToo)
                showClosedItems = 1;

            // now go and load up all of the questions and answers..
            rptClientResponses.ItemDataBound += new RepeaterItemEventHandler(handleDataBind_ClientResponsesRepeater);
            m_data.runSqlRepeater("spGetSurveyClientResponses " + surveyId + ", " + showClosedItems, rptClientResponses);
            
            // if recently approved, let the user know that there are items to close out now..
            bool showRecentlyApprovedMsg = (cTools.cStrExt(HttpContext.Current.Session["ShowRecentApproval"]) == "Y");
            HttpContext.Current.Session["ShowRecentApproval"] = ""; // reset for next time
            if (showRecentlyApprovedMsg)
            {
                pnlRecentlyApproved.Visible = true;
                pnlClientOutstandingItems.Visible = (rptClientResponses.Items.Count != 0);
            }
            else
            {
                if (rptClientResponses.Items.Count == 0)
                    pnlAllClientItemsClosed.Visible = true;
            }
            
            // show the qa list and enable all js/ajax
            phAjaxManager.Visible = true;   // enable auto-saving of answers as they're ticked in real-time
            phFileUploadScripts.Visible = true; // need file upload scripts
            phFileUploadDialog.Visible = true;
            pnlClientResponses.Visible = true;

            configQuickNavbar();
        }
        
        protected void handleDataBind_ClientResponsesRepeater(object sender, RepeaterItemEventArgs e)
        {
            // only interested in the list items..
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            // get some data I'll need later..
            string questionId = cTools.cStrExt(cRepeater.getDataVal(e, "questionid"));
            string questionCategory = cTools.cStrExt(cRepeater.getDataVal(e, "questioncatname"));
            bool itemClosed = (cTools.cIntExt(cRepeater.getDataVal(e, "closed")) == 1);

            /*
                Show the category when it changes..
            */
            if (questionCategory != m_prevquestioncategory)
            {
                // show the category heading..
                cRepeater.setPlaceholderVisibility(e, "phCategoryHeading", true);

                m_prevquestioncategory = questionCategory;
            }

            // load up labels and textboxes..
            cRepeater.setLabelBr(e, "lblObservations", "observations");
            cRepeater.setTextbox(e, "txtClientResponse", "clientresponse");
            cRepeater.setTextbox(e, "txtTargetDate", "targetdate");

            // list all of the file uploads for this question...
            int sourceId = cConstants.sourceClient;
            Repeater rptClientFileUploads = (Repeater)(e.Item.FindControl("rptClientFileUploads"));
            rptClientFileUploads.ItemDataBound += new RepeaterItemEventHandler(handleDataBind_QuestionFileUploads);
            m_data.runSqlRepeater("spGetFileUploadsList " + m_idref + ", " + questionId + ", " + sourceId, rptClientFileUploads);

            // set up the ADD (file uploads) button
            HyperLink hrefAddFile = (HyperLink)(e.Item.FindControl("hrefAddFile"));
            if (!_isreadonly)
            {
                hrefAddFile.Attributes.Add("onclick",
                    "uploadfile(" + questionId + "); return (false);"
                );
                hrefAddFile.Attributes.Add("style", "cursor:pointer;");
            }

            // set up the auto-save when the content has changed
            TextBox txtClientResponse = (TextBox)(e.Item.FindControl("txtClientResponse"));
            if (!_isreadonly)
            {
                txtClientResponse.Attributes.Add("onchange",
                "saveClientResponse(" + questionId + ",'" + txtClientResponse.ClientID + "');"
                );
            }

            //just grab this for later..
            TextBox txtTargetDate = (TextBox)(e.Item.FindControl("txtTargetDate"));

            // enable/disable inputs..
            hrefAddFile.Visible = !_isreadonly;
            txtTargetDate.Enabled = !_isreadonly;
            txtClientResponse.Enabled = !_isreadonly;

            
            // change the behaviour of the close button
            if (itemClosed || _isreadonly)
            cRepeater.setPlaceholderVisibility(e, "phCloseButton", false);
                
        }
        private void saveClientResponse(int questionID, string clientResponse, string targetDate, string closeItem)
        {
            int surveyId = m_idref;
            
            // save the response.
            int idCheck = m_data.saveClientResponse(surveyId, cSecurity.userID(), questionID, clientResponse, targetDate, closeItem);

            //audit trail
            string auditDescription = m_data.getQuestion(questionID) + "\n";
            if (clientResponse != "null")       
                auditDescription += "Assigned To/Client Actions: " + clientResponse;
            if (targetDate != "null")       
                auditDescription += "Target Date: " + targetDate;
            if (closeItem == "null")
                m_data.auditEvent(cConstants.eventClientResponseSave, surveyId, auditDescription);
            else
            {
                // close item
                m_data.auditEvent(cConstants.eventClientResponseClose, surveyId);

                // see if this is the last item pending ..
                string sqlCmd = "spGetSurveyClientNumOpenItems " + surveyId;
                int numOpenItems = cTools.cIntExt(m_data.runSql(sqlCmd));

                if (numOpenItems == 0)
                {
                    // send a notification to the client admins on close of last item..
                    cPushNotifications.handleEvent(cConstants.eventClientResponsesClosed_NotifyClientMgr, surveyId);
                    cPushNotifications.handleEvent(cConstants.eventClientResponsesClosed_NotifyClientAdmin, surveyId);
                }
            }

            if (idCheck == 0)
                cMsgBox.showSaveErrorMsg();
        }

        protected void lnkShowClientOpenOnly_Click(object sender, EventArgs e)
        {
            loadClientResponses(m_idref, false);
        }

        protected void lnkShowClientClosedItems_Click(object sender, EventArgs e)
        {
            loadClientResponses(m_idref, true);
        }
        #endregion

        #region File Upload Handling

        private void uploadFileToServer(int sourceId)
        {
            string uploadsFolder = cTools.cStrExt(ConfigurationManager.AppSettings["UploadsFolder"]);
            int surveyId = m_idref;
            int questionId = cTools.cIntExt(hidSelectedQuestionId.Value);
            string invalidFiles = string.Empty;

            cTools.log("actionref:" + m_actionref + " ..  questionId:" + questionId);

            // add a sub-folder for this user (so all files aren't stored in one directory)
            string outputFolder = uploadsFolder + "\\" + surveyId.ToString();    // sets up a folder using the surveyId

            // create the folder if it's not already there
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);

            // process each file and upload accordingly..
            HttpFileCollection uploadedFiles = Request.Files;
            for (int i = 0; i < uploadedFiles.Count; i++)
            {
                HttpPostedFile userPostedFile = uploadedFiles[i];

                try
                {
                    if (userPostedFile.ContentLength > 0)
                    {
                        // store the original filename
                        string originalFilename = userPostedFile.FileName;

                        // filter out any spaces from the upload file - removes the extension
                        string filenameAlphaNumOnly = Path.GetFileNameWithoutExtension(originalFilename);
                        filenameAlphaNumOnly = cTools.cAlphaNumeric(filenameAlphaNumOnly); //(cAlphaNumeric removes the .pdf)

                        // get the file extension 
                        string fileExt = Path.GetExtension(originalFilename);   

                        //  datestamp the upload so we're not overwriting anything - note the DOT is already in fileExt {2}
                        string uniqueFilename = String.Format("{0}_{1}{2}",
                            filenameAlphaNumOnly,
                            DateTime.Now.ToString("ddMMyyhhss"),
                            fileExt);

                        // check that the file is of the allowed extension..
                        if (allowedFileFormats.Contains(fileExt.ToLower()))
                        {
                            // upload the file now to our destination folder..
                            userPostedFile.SaveAs(outputFolder + "\\" + uniqueFilename);

                            m_data.saveFileUpload(0, surveyId, questionId, sourceId, originalFilename, uniqueFilename);

                            //audit trail
                            string auditDescription = m_data.getQuestion(questionId) + "\n" +
                                "File: " + originalFilename;
                            m_data.auditEvent(cConstants.eventFileUpload, sourceId, auditDescription);
                        }
                        else
                        {
                            invalidFiles += "<li>" + originalFilename + "</li>";
                        }
                        
                    }
                }
                catch (Exception Ex)
                {
                    // no good..
                    cTools.log("*** Error *** Upload file failed:" + Ex.Message);
                }
            }

            if (invalidFiles != string.Empty)
            {
                cTools.log("Invalid files in upload dialog:" + invalidFiles);
                cMsgBox.saveUserDialog("Invalid file format found in the following files:<br><br><ul>" + invalidFiles + "</ul>") ;
            }
        }

        private void deleteFile(int fileId)
        {
            string uploadsFolder = cTools.cStrExt(ConfigurationManager.AppSettings["UploadsFolder"]);
            int surveyId = m_idref;
            string sourceFolder = uploadsFolder + "\\" + surveyId.ToString();
            string destinationFolder = uploadsFolder + "\\_DeletedItems\\" + surveyId.ToString();

            // move the deleted file to a deleted folder
            try
            {   
                // create the folder if it's not already there
                if (!System.IO.Directory.Exists(destinationFolder))
                    System.IO.Directory.CreateDirectory(destinationFolder);

                // get the name of the file (the server-version)
                string sqlCmd = "SELECT serverfilename FROM fileuploads WHERE fileid=" + fileId;
                string serverFilename = m_data.runSql(sqlCmd);
                if (serverFilename != "")
                {
                    // move the file..
                    sourceFolder += "\\" + serverFilename;
                    destinationFolder += "\\" + serverFilename;
                    System.IO.File.Move(sourceFolder, destinationFolder);

                    //audit trail
                    sqlCmd = "SELECT sourcefilename FROM fileuploads WHERE fileid=" + fileId;
                    string originalFilename = m_data.runSql(sqlCmd);
                    int questionId = m_data.getQuestionIdFromFileId(fileId);
                    string auditDescription = m_data.getQuestion(questionId) + "\n" +
                        "File: " + originalFilename;
                    m_data.auditEvent(cConstants.eventFileDelete, surveyId, auditDescription);

                }
            }
            catch (Exception Ex)
            {
                // no good..
                cTools.log("*** Error *** File deletion failed:" + Ex.Message);
                cTools.log("Move [" + sourceFolder + "] to [" + destinationFolder + "]");
            }
    
            // remove from this survey
            m_data.deleteFile(fileId);
        }

        protected void cmdUploadFile_Command(object sender, CommandEventArgs e)
        {
            int questionId = cTools.cIntExt(hidSelectedQuestionId.Value);
            int sourceId = 0;
            
            // some files are uploaded by auditors, others by clients
            switch (m_actionref)
            {
                case "qa":
                    sourceId = cConstants.sourceAuditor;
                    break;

                case "clientqa":
                    sourceId = cConstants.sourceClient;
                    break;
            }

            // upload the file and store in db
            uploadFileToServer(sourceId);

            // refresh this page and go straight to the current question..
            switch (m_actionref)
            {
                case "qa":
                    sourceId = cConstants.sourceAuditor;
                    break;

                case "clientqa":
                    sourceId = cConstants.sourceClient;
                    break;
            }

            // go back to this screen and auto-focus on the current question
            Response.Redirect("~/audit/" + m_actionref  + "/" + m_idref + "?q=" + questionId + "#" + questionId); //this page!
        }
        
        protected void cmdCancelUpload_Command(object sender, CommandEventArgs e)
        {

        }
        protected void cmdRemoveFile_Command(object sender, CommandEventArgs e)
        {
            int questionId = cTools.cIntExt(hidSelectedQuestionId.Value);
            int fileId = cTools.cIntExt(hidSelectedFileId.Value);

            deleteFile(fileId);

            // refresh this page and go straight to the current question..
            Response.Redirect("~/audit/" + m_actionref + "/" + m_idref + "?q=" + questionId + "#" + questionId); //this page!
        }

        #endregion

        #region "Ajax Call Handling"

        protected void cmdMakeAjaxCall_Command(object sender, CommandEventArgs e)
        {
            cTools.log("Ajax Call (" + m_actionref + "): " + hidParamName.Value + " = " + hidParamValue1.Value + " | " + hidParamValue2.Value + " | " + hidParamValue3.Value);

            int questionId = cTools.cIntExt(hidParamName.Value);

            switch (m_actionref)
            {
                case "qa":
                    int answerId = cTools.cIntExt(hidParamValue1.Value);
                    string answerText = cTools.cStrExt(hidParamValue2.Value);
                    int classificationId = cTools.cIntExt(hidParamValue3.Value);

                    saveSurveyAnswer(questionId, answerId, classificationId, answerText);
                    break;

                case "clientqa":
                    string clientResponse = cTools.cStrExt(hidParamValue1.Value);
                    string targetDate = cTools.cStrExt(hidParamValue2.Value);
                    string closeItem = cTools.cStrExt(hidParamValue3.Value);

                    saveClientResponse(questionId, clientResponse, targetDate, closeItem);
                    break;
            }
            
        }

        #endregion

        #region Page Tools
        private void loadSurveyTitles(SurveySummaryData survey)
        {
            LocationData location = m_data.loadLocation(survey.locationid);
            ClientData client = m_data.loadClient(location.clientid);

            // convert template id to name 
            string sqlCmd = "SELECT templatename FROM templates WHERE templateid=" + survey.templateid;
            string templateName = m_data.runSql(sqlCmd);

            lblClientRef.Text = string.Format("{0} - {1} - {2}<br />" + 
                "<small>{3} - {4}</small>",
                client.clientname,
                location.businessunit,
                location.locationname,
                templateName,
                survey.dateofaudit);
            
            pnlClientRef.Visible = true;

            // show the current status..
            switch (survey.client_approvalstatusid)
            {
                case cConstants.approvalStatus_Approved:
                    pnlStatus.CssClass += "alert alert-success title-alert";
                    if (survey.statusid == cConstants.statusReleased)
                        lblStatus.Text = "RELEASED";
                    else
                        lblStatus.Text = "APPROVED BY CLIENT";
                    pnlStatus.Visible = true;
                    break;

                case cConstants.approvalStatus_Rejected:
                    lblStatus.Text = "REJECTED BY CLIENT";
                    pnlStatus.CssClass += "alert alert-danger title-alert";
                    pnlStatus.Visible = true;
                    break;
            }
            
        }
        
        private void saveScreenState(string screenState)
        {
            HttpContext.Current.Session["ScreenState"] = screenState;
        }

        private string getScreenState()
        {
            return cTools.cStrExt(HttpContext.Current.Session["ScreenState"]);
        }


        private void configQuickNavbar()
        {
            
            switch (m_actionref)
            {
                case "summary":
                    litQuickMenuSummary.Text = "class=\"active\"";
                    //litQuickMenuPrev.Text = "class=\"disabled\"";
                    //hrefQuickMenuNext.NavigateUrl = "~/audit/qa/" + m_idref;
                    phQuickMenu.Visible = true;
                    break;

                case "qa":
                    litQuickMenuQa.Text = "class=\"active\"";
                    //hrefQuickMenuPrev.NavigateUrl = "~/audit/summary/" + m_idref;
                    //hrefQuickMenuNext.NavigateUrl = "~/audit/clientqa/" + m_idref;
                    phQuickMenu.Visible = true;
                    break;

                case "clientapproval":
                    litQuickMenuClientApproval.Text = "class=\"active\"";
                    phQuickMenu.Visible = true;
                    break;

                case "clientqa":
                    litQuickMenuClientQa.Text = "class=\"active\"";
                   // hrefQuickMenuPrev.NavigateUrl = "~/audit/qa/" + m_idref;
                    //hrefQuickMenuNext.NavigateUrl = "~/audit/print/" + m_idref;
                    phQuickMenu.Visible = true;
                    break;

                case "print":
                    litQuickMenuPrint.Text = "class=\"active\"";
                    //hrefQuickMenuPrev.NavigateUrl = "~/audit/clientqa/" + m_idref;
                    //litQuickMenuNext.Text = "class=\"disabled\"";
                    phQuickMenu.Visible = true;
                    break;

            }

        }
        
        #endregion

    }
}