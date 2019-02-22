using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SurveyApp
{
    public partial class Home : System.Web.UI.Page
    {

        #region Member Variables
        // our database class
        private cData m_data;

        private WebUser webUser; 
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
            webUser = cSecurity.getUser();
        }

        private void initApp()
        {
            
        }

        private void updateApp()
        {
            // admin users get auto-accept terms, regular users must agree terms before continuing
            if (!webUser.isadmin && !webUser.issuperadmin) {
                if (cTools.cStrExt(HttpContext.Current.Session["UserAgreesTerms"]) != "Y")
                {
                    // show one OR the other..
                    if (webUser.isnewhire)
                        phAcceptTerms_User.Visible = true;
                    else if (webUser.isleader)
                        phAcceptTerms_Client.Visible = true;
                    pnlTerms.Visible = true;
                    return;
                }
            }
            
            if (cTools.cStrExt(HttpContext.Current.Session["UserAgreesTerms_ThankYouMsg"]) == "PENDING")
            {
                pnlTermsAccepted.Visible = true;
                HttpContext.Current.Session["UserAgreesTerms_ThankYouMsg"] = "";
            }

            // always check for a logged-in user...
            if (cSecurity.userID() != 0)
                loadSurveyList();
        }
        protected void chkIsAdmin_CheckedChanged(object sender, EventArgs e)
        {
            HttpContext.Current.Session["UserAgreesTerms"] = "Y";
            HttpContext.Current.Session["UserAgreesTerms_ThankYouMsg"] = "PENDING";
            Response.Redirect("~/home");    //refresh the page!
        }
        #endregion

        #region Survey Listing
        private void loadSurveyList()
        {
            string pageTitle = "";
            string locationIdParam = "null";    // see audits for all locations
            string statusIdParam1 = string.Empty;
            string statusIdParam2 = string.Empty;
            string userIdParam = string.Empty;
         
            // show the list of pending surveys on the home page..
            if (webUser.isadmin || webUser.issuperadmin)        // admins 
            {
                pageTitle = "Current Audits";
                statusIdParam1 = "null"; //admins see all jobs regardless of status
                statusIdParam2 = "null"; 
                userIdParam = "null";        // null = see all audits for all locations (admin-only)
            }
            else if (webUser.isnewhire)                         // contractors
            {
                pageTitle = "Audits in Progress";
                statusIdParam1 = cConstants.statusDraft.ToString();
                statusIdParam2 = cConstants.statusDraft.ToString();
                userIdParam = webUser.userid.ToString();
            }
            else if (webUser.isleader)                          // clients
            {
                pageTitle = "Audits for Review";
                statusIdParam1 = cConstants.statusForClientReview.ToString();
                statusIdParam2 = cConstants.statusReleased.ToString();
                userIdParam = webUser.userid.ToString();
            }
            else
            {
                // this could be if the user who's logged in doesn't have a role associated with them..
                pageTitle = "xxxxxxxx";
                userIdParam = "-1";        // null = see all audits for all locations (admin-only)
            }

            lblGridTitle.Text = pageTitle;
            lblGridTitle.Font.Bold = true;

            // load up the list of surveys for this location
            GridView1.RowDataBound += new GridViewRowEventHandler(handleDataBind_SurveyListing);
            string sqlCmd = "spGetSurveyList " + locationIdParam + ", " + userIdParam + ", " + statusIdParam1 + ", " + statusIdParam2;
            cTools.log(sqlCmd);
            m_data.bindToGrid(sqlCmd, GridView1);
            pnlGrid.Visible = true;

            if (GridView1.Rows.Count == 0)
            {
                lblGridFooter.Text = "&nbsp;&nbsp;&nbsp;&nbsp;- No records found -";
                lblGridFooter.CssClass = "text-warning";
            }

        }


        private void handleDataBind_SurveyListing(object sender, GridViewRowEventArgs e)
        {
            int colId = 0;
            int colAuditDate = 1;
            int colClientName = 2;
            int colLocationId = 3;
            int colAuditType = 6;
            int colStatusId = 8;
            int colStatusName = 9;
            int colLastModifiedDate = 10;
            int colClientReviewDate = 11;
            int colButtonsPlaceholder = 12;

            // hide id's in the grid 
            e.Row.Cells[colId].Visible = false;
            e.Row.Cells[colClientName].Visible = false;
            e.Row.Cells[colLocationId].Visible = false;
            e.Row.Cells[colAuditType].Visible = false;
            e.Row.Cells[colLastModifiedDate].Visible = false;
            e.Row.Cells[colStatusId].Visible = false;
            e.Row.Cells[colClientReviewDate].Visible = false;

            // I want the date to stop going to a second line
            e.Row.Cells[colAuditDate].Width = 100;
            
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    break;

                case DataControlRowType.DataRow:
                    /*
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
                            e.Row.Cells[colDateOfAudit].Text = alertSpan + "&nbsp;" + e.Row.Cells[colDateOfAudit].Text;
                        }
                    }
                    */


                    // create buttons
                    string gridButtons = String.Format(
                            "<a class='btn btn-primary btn-xs' href='{0}'>Open</a>", 
                            Page.ResolveUrl("~/audit/summary/") + e.Row.Cells[colId].Text);
                    e.Row.Cells[colButtonsPlaceholder].Text = gridButtons;
                    break;
            }
        }

        #endregion

    }
}