using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SurveyApp
{
    public partial class AppLogin : System.Web.UI.Page
    {

        #region Member Variables
        private int userLoginErrorCount;
        #endregion

        #region Page Loading  & Startup
        /*
         * ====================================
         *          PAGE LOADING
         * ====================================
         */

        protected void Page_Load(object sender, EventArgs e)
        {
            cTools.log("Login Loadup");

            if (!IsPostBack)
                initApp();      // on first load of the app, load up our various database bits and bobs..

            updateApp();        // when things get updated (and also on first load), we can enable/disable things depending on what's on screen                
        }

        private void initApp()
        {
            txtUsername.Focus();
        }

        private void updateApp()
        {
            pnlUserLoginError.Visible = false;  // reset each time..
            
            userLoginErrorCount = cTools.cIntExt(Session["userLoginErrorCount"]);
            if (userLoginErrorCount >= 3)
            {
                Response.Clear();
                Response.Write("Too many failed login attempts. System halted!");
                Response.End();
            }
        }

        #endregion

        #region Page Event Handlers

        protected void cmdApplicationLogin_Command(object sender, CommandEventArgs e)
        {
            if (txtUsername.Text == "" || txtPassword.Text == "")
                return;

            if (cSecurity.checkApplicationLogin(txtUsername.Text, txtPassword.Text))
            {
                Response.Redirect("~/home");
            }
            else
            {
                pnlUserLoginError.Visible = true;
                userLoginErrorCount++;

                Session["userLoginErrorCount"] = userLoginErrorCount;
            }
        }

        #endregion

    }

}