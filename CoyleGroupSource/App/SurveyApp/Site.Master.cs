using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration; // so I can access web.config
using System.Reflection;

namespace SurveyApp
{
    public partial class Site : System.Web.UI.MasterPage
    {

        #region Member Variables
        
        // our database class
        private cData m_data;

        private bool isDebug = false;
       
        #endregion

        #region Page Loading  & Startup
        /*
         * ====================================
         *          PAGE LOADING
         * ====================================
         */
         
        //  Note: Page_PreLoad() not firing so using Page_Load() instead
        protected void Page_Load(object sender, EventArgs e)
        {
            // open up the db 
            m_data = new cData();
            if (!m_data.openDB())
            {
                Response.Write("Error opening database. Cannot continue<br><br>" + m_data.lasterrmsg);
                Response.End();
            }

            initPage();

            // on first load of the app, load up the doc listing..
            if (!Page.IsPostBack)
                initApp();
            
            // cleanup
            m_data.closeDB();
        }
        
        private void initPage()
        {
            // check user login on every page..
            if (cSecurity.userID() == 0)
            {
                pnlMainMenu.Visible = false;

                // go to login page - but not if we're already on the login page (obvs)!
                if (!HttpContext.Current.Request.FilePath.Contains("/login"))
                    Response.Redirect("~/login");
            }
                    
            isDebug = (cTools.cStrExt(ConfigurationManager.AppSettings["DebugMode"]).ToLower() == "yes");

            pnlMainMenu.Visible = true;

            // on dev pc, once someone has logged in once, I can show the dev panel
            pnlLoginSwitch.Visible = isDebug;

            // show a message if this is a debug/dev/preview version
            //      this is meant to remind me not to publish preview versions without copying the correct web.config!
            string versionStamp = cTools.cStrExt(ConfigurationManager.AppSettings["VersionStamp"]);
            if (versionStamp != "")
            {
                lblVersionStamp.Text = versionStamp;
                pnlVersionStamp.Visible = true;
            }

            // show the current date and time..
            lblDateAndTime.Text = cTools.cDateExt(DateTime.Now);

            // include extra css/js if we need the date picker (adding jQuery twice breaks modals)
            if (HttpContext.Current.Request.FilePath.Contains("/audit/summary") 
                || HttpContext.Current.Request.FilePath.Contains("/audit/clientqa")
                || HttpContext.Current.Request.FilePath.Contains("/reports/"))
                phDatePickerIncludes.Visible = true;
            
            if (HttpContext.Current.Request.FilePath.Contains("/home")
                || HttpContext.Current.Request.FilePath.Contains("/audit/summary")
                || HttpContext.Current.Request.FilePath.Contains("audit/clientapproval")
                || HttpContext.Current.Request.FilePath.Contains("audit/print"))
                    pnlContentWrapper.CssClass = "content watermark";

            if (HttpContext.Current.Request.FilePath.Contains("/home"))
            {
                pnlTwitterFeed.Visible = true;
                pnlTwitterHeightSetter.Visible = true;
            }

            // show any dialogs we might need on the front end ..
            cMsgBox.displayPendingDialogs();
        }
        
        private void initApp()
        {
            lblVersion.Text = "v" + cVersion.BuildNum;

            // load up data about this user that I need to customise the site..
            WebUser webUser = cSecurity.getUser();
            if (webUser.isadmin || webUser.issuperadmin)
            {
                phAdminMenu.Visible = true;
                phAuditTrailReportMenu.Visible = true;
                phReportsMenu.Visible = true;
            }
            else
            {
                // standard users only get access when they agree to the terms & conditions of use..
                phUserMenu.Visible = (cTools.cStrExt(HttpContext.Current.Session["UserAgreesTerms"]) == "Y");
                phReportsMenu.Visible = (webUser.isleader); // only clients can run reports, not consultants
            }
        }

        #endregion
        
        #region Page Event Handlers
        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            cSecurity.logout();
        }

        protected void lnkSwitchUserAdmin_Click(object sender, EventArgs e)
        {
            cSecurity.loadUser(1);
            Response.Redirect("~/home");
        }

        protected void lnkSwitchUserStandard_Click(object sender, EventArgs e)
        {
            cSecurity.loadUser(3);
            Response.Redirect("~/home");
        }

        protected void lnkSwitchUserClient_Click(object sender, EventArgs e)
        {
            cSecurity.loadUser(5);
            Response.Redirect("~/home");
        }
        #endregion
    }
}