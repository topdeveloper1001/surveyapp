using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SurveyApp
{
    public partial class Reports : System.Web.UI.Page
    {

        #region Member Variables
        // our database class
        private cData m_data;
        WebUser _webUser;
        private string _selectedReport = string.Empty;
        #endregion

        #region Page Loading  & Startup
        /*
         * ====================================
         *          PAGE LOADING
         * ====================================
         */
        protected void Page_PreLoad()
        {
            Server.ScriptTimeout = 300; // 5 mins for some reports (unlikely but better to handle it now)

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
            initPage();

            if (!IsPostBack)
                initApp();      // on first load of the app, load up our various database bits and bobs..

            updateApp();        // when things get updated (and also on first load), we can enable/disable things depending on what's on screen                
        }

        private void initPage()
        {
            _webUser = cSecurity.getUser();
            _selectedReport= cTools.cStrExt(Page.RouteData.Values["reportref"]);
            GridView1.Visible = false;  // hide the report when things change onscreen - eg different ddn selection
        }

        private void initApp()
        {
            // customise the screen based on the user's role..leader vs buddy
            lblPageTitle.Text = "REPORTING MODULE";
            pnlReportParams.Visible = true;

            // reload the previous selection in case the user moves to other screens and back again
            string selectedClientId = cTools.cStrExt(Session["Reports-ClientId"]);
            string selectedLocationId = cTools.cStrExt(Session["Reports-LocationId"]);
            string selectedSurveyId= cTools.cStrExt(Session["Reports-SurveyId"]);
            string selectedFromDate = cTools.cStrExt(Session["Reports-StartDate"]);
            string selectedToDate = cTools.cStrExt(Session["Reports-EndDate"]);

            // load up the dropdowns now..and set defaults..
            loadClientsForUser();
            cTools.autoSelectListItem(cboClient, selectedClientId);

            loadLocationsForSelectedClient();
            cTools.autoSelectListItem(cboLocation, selectedLocationId);

            loadSurveysForSelectedLocation();   //clears previous selections..
            cTools.autoSelectListItem(cboSurvey, selectedSurveyId);

            // set other defaults from previous run
            txtDateFrom.Text = selectedFromDate;
            txtDateTo.Text = selectedToDate;
            
            btnExportReport.Visible = false;
            selectReport(); // I need a way to refresh enabled/disabled based on the previously selected report
        }

        

        private void updateApp()
        {

        }

        #endregion

        #region "Main Page Processing Block"

        private void loadClientsForUser()
        {
            if (_webUser.isadmin || _webUser.issuperadmin)  // sa can select a client
                m_data.createDropdown("spGetClientDropdown null", cboClient, "== All ==", "");
            else
                m_data.createDropdown("spGetClientDropdown " + _webUser.userid, cboClient); 
            
        }
        private void loadLocationsForSelectedClient()
        {
            int clientId = cTools.cIntExt(cboClient.SelectedValue);
            
            string sqlCmd = "spGetLocationsDropdown " + clientId + ", ";
            if (_webUser.isadmin || _webUser.issuperadmin)
                sqlCmd += "null";  // admin users see all clients
            else
                sqlCmd += _webUser.userid;
            m_data.createDropdown(sqlCmd, cboLocation, "== All ==", "");
        }
        private void loadSurveysForSelectedLocation()
        {
            int locationId = cTools.cIntExt(cboLocation.SelectedValue);
          
            string sqlCmd = "spGetSurveysDropdown " + locationId + ", ";
            if (_webUser.isadmin || _webUser.issuperadmin)
                sqlCmd += "null";  // admin users see all clients
            else
                sqlCmd += _webUser.userid;
            m_data.createDropdown(sqlCmd, cboSurvey, "== All ==", "");

        }
        private void selectReport()
        {
            // default to all inputs disabled (handy when they click "please select")
            cboClient.Enabled = false;
            cboLocation.Enabled = false;
            cboSurvey.Enabled = false;
            txtDateFrom.Enabled = false;
            txtDateTo.Enabled = false;
            
            switch (_selectedReport)
            {
                case "audittrail":
                    cboClient.Enabled = true;
                    cboLocation.Enabled = true;
                    cboSurvey.Enabled = true;
                    //txtDateFrom.Enabled = true;
                    //txtDateTo.Enabled = true;
                    break;

                case "classificationssummary":
                    cboClient.Enabled = true;
                    cboLocation.Enabled = true;
                    //txtDateFrom.Enabled = true;
                    //txtDateTo.Enabled = true;
                    break;

            }

            // each time a report is chosen, hide the previous one 
            pnlReportResults.Visible = false;
        }
        private void runReport()
        {
            string sqlCmd = "";
            string startDate, endDate;

            // remember the settings for next time..
            Session["Reports-ClientId"] = cboClient.SelectedValue;
            Session["Reports-LocationId"] = cboLocation.SelectedValue;
            Session["Reports-SurveyId"] = cboSurvey.SelectedValue;
            Session["Reports-StartDate"] = txtDateFrom.Text;
            Session["Reports-EndDate"] = txtDateTo.Text;

            int clientId = cTools.cIntExt(cboClient.SelectedValue);
            int locationId = cTools.cIntExt(cboLocation.SelectedValue);
            int surveyId = cTools.cIntExt(cboSurvey.SelectedValue);
            startDate = cTools.cDateOnly(txtDateFrom.Text);    // helps us to set a standard date format for the db
            endDate = cTools.cDateOnly(txtDateTo.Text);

            // set up our row handler..
            GridView1.RowDataBound += new GridViewRowEventHandler(GridView1_DataBinding);

            switch (_selectedReport)
            {
                case "audittrail":    // date range..
                    // don't run the report unless I've everything I need
                    if (surveyId == 0)
                    {
                        msgBox("Please select an audit for this report");
                        return;
                    }

                    sqlCmd = "spReport_AuditTrail " + surveyId;
                    break;

                case "classificationssummary":
                    int userId = _webUser.userid;   // pass the user so we can restrict the report to their access only
                    if (_webUser.isadmin || _webUser.issuperadmin)
                        userId = 0; //open up to all locations when you're an admin
                    sqlCmd = "spReport_ClassificationsSummary " + userId + ", " +  clientId + ", " + locationId + ",'" + startDate + "', '" + endDate + "'";
                    break;

                default:    // nothing selected..or an unsupport report
                    return;

            }
            
            // now run the report + show details on screen..
            m_data.bindToGrid(sqlCmd, GridView1);
            GridView1.Visible = true;       //show the grid now that we've run the report
            
            pnlReportResults.Visible = (GridView1.Rows.Count > 0);
            btnExportReport.Visible = (GridView1.Rows.Count > 0);

            if (GridView1.Rows.Count == 0)
                lblReportStatus.Text = "No data for report";
            else
                lblReportStatus.Text = "";
        }

         #endregion

        #region "Data Binders"
        private void handleDataBind_AuditTrail(GridViewRowEventArgs e)
        {
            int colEventDetails = 3;
            
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    e.Row.Cells[colEventDetails].Text = e.Row.Cells[colEventDetails].Text.Replace("\n", "<br />");
                    break;
            }
        }
        #endregion

        #region "Page Event Handlers"
        
        protected void cmdRunReport_Command(object sender, CommandEventArgs e)
        {
            runReport();
        }

        protected void btnExportReport_Click(object sender, EventArgs e)
        {
            exportReport("csv");
        }

       
        protected void GridView1_DataBinding(object sender, GridViewRowEventArgs e)
        {

            switch (_selectedReport)
            {
                case "audittrail":
                    handleDataBind_AuditTrail(e);
                    break;
            }

        }
        protected void cboLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadSurveysForSelectedLocation();
        }

        protected void cboClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadLocationsForSelectedClient();
            loadSurveysForSelectedLocation();   //clears previous selections..

        }
        #endregion

        #region "Import/Export"

        private void exportReport(string exportFormat)
        {
            // name the export file something useful!
            string exportFilename = _selectedReport;
            switch (_selectedReport)
            {
                case "audittrail":    // date range..
                    exportFilename = "AuditTrailReport";
                    break;

                case "classificationssummary":
                    exportFilename = "ClassificationsSummaryReport";
                    break;
            }

            string fileExt = "";
            switch (exportFormat)
            {
                case "csv":
                    fileExt = "csv";
                    break;

                case "tab":
                    fileExt = "txt";
                    break;

                default:
                    return;

            }

            // set up the output filename using the report title, datestamp + file extension
            string fileName = String.Format("{0}_{1}.{2}", exportFilename, DateTime.Now.ToString("ddMMMyyyy_HHmm"), fileExt);
            
            // convert what we see on-screen in the grid to a string for export purposes
            string clientSource = getGridDataAsCSV(exportFormat);
            
            // let's set up our export now..
            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ";");
            Page.Response.Charset = "";
            Page.Response.ContentType = "application/text";

            //send content to the browser now..
            Response.Output.Write(clientSource);
            Response.Flush();
            Response.End();
        }
    

        // Exporting to various formats// https://code.msdn.microsoft.com/office/Export-GridView-to-07c9f836
        private string getGridDataAsCSV(string exportFormat)
        {
            string delimChar = "|";
            /*
                * Note: If you set the "autogeneratedcolumns" property to true then the count will always show 0:
                *  GridView1.Columns.Count won't work so we have to see if there's data in the header row and use that to get the num cols..
                *  In fact, it doesn't even create the columns collection so we can't even get the header text this way either..
                */
            int numGridCols = 0;
            int gridDataStartCol = 0;
            if (GridView1.Rows.Count > 0)
                numGridCols = GridView1.Rows[0].Cells.Count;

            // by default we start the data at col 0 but in fact, it's col 1 since we have a chkbox in 0
            gridDataStartCol = 0;
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();

            // set up the delimiter
            switch (exportFormat)
            {
                case "csv":
                    delimChar = ",";
                    break;

                case "tab":
                    delimChar = "\t";
                    break;

                default:
                    break;
            }

            // build up & format the output first..
            // get the grid header and create the first row..
            for (int k = gridDataStartCol; k < numGridCols; k++)
            {
                string headerText = GridView1.HeaderRow.Cells[k].Text;
                headerText = cleanForCSVExport(headerText);
                stringBuilder.Append(headerText + delimChar);        //add data + delimiter

            }
            stringBuilder.Append("\r\n"); //append new line 

            // get each row from the data part of the grid and create the individual csv lines..
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                for (int k = gridDataStartCol; k < numGridCols; k++)
                {
                    string cellText = GridView1.Rows[i].Cells[k].Text;
                    if (GridView1.Rows[i].Cells[k].ToolTip != "") { // in case we've changed the content, use the original (if set) stored in the tooltip
                        cellText = GridView1.Rows[i].Cells[k].ToolTip;
                    }
                    cellText = cleanForCSVExport(cellText);
                    stringBuilder.Append(cellText + delimChar);    //add data + delimiter
                }
                stringBuilder.Append("\r\n");       //append new line 
            }

            // send the data back to the calling fxn
            return (stringBuilder.ToString());

        }

        
        private string cleanForCSVExport(string currentText)
        {
            currentText = currentText.Replace(",", "");
            currentText = currentText.Replace("&nbsp;", " ");
            currentText = currentText.Replace("&amp;", "&");

            return (currentText);
        }
        #endregion

        #region Handy Tools
        private void msgBox(string msgToUser)
        {
            string myScript = " $(\"#msgbox\").append('<div class=\"alert alert-danger\" id=\"alertmsgbox\">" + msgToUser + "</div>'); " +
            " $(\"#alertmsgbox\").alert(); " +
            " window.setTimeout(function () { $(\"#alertmsgbox\").alert('close'); }, 3000); ";

            Type cstype = this.GetType();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(cstype, "alertscript", myScript, true);
        }


        #endregion

       
    }
}