using System;
using System.Web;
using System.Configuration;                         // so I can access web.config

namespace SurveyApp
{
    public class cSecurity
    {
        private const string ardentNetwork = "Ardent";
        private const string bsaNetwork = "BSA";

        static cSecurity()
        {
            
            // on application startup, we check to see if we've to override the userid - use this if requested..
            int forceUserID = cTools.cIntExt(ConfigurationManager.AppSettings["ForceUserID"]);
            if (forceUserID != 0)
            {
                cTools.log("forceUserID=" + forceUserID.ToString());
                loadUser(forceUserID);
            }
        }

        // load up the user details and store in the session
        public static void loadUser(int userId)
        {
            WebUser webUser = new WebUser();

            cData db = new cData();
            if (db.openDB())
            {
                webUser = db.loadUser(userId);
                db.closeDB();   // cleanup
            }

            HttpContext.Current.Session["user.id"] = userId.ToString();
            HttpContext.Current.Session["user.object"] = webUser;

        }

        public static void logout()
        {
            // audit trail..
            if (userID() != 0) { 
                cData db = new cData();
                if (db.openDB())
                {
                    db.auditEvent(cConstants.eventUserLogout);
                    db.closeDB();   // cleanup
                }
            }            
                
            HttpContext.Current.Session["user.id"] = "0";
            HttpContext.Current.Session["user.object"] = null;
            HttpContext.Current.Session["UserAgreesTerms"] = "";
            HttpContext.Current.Response.Redirect("~/login");
        }

        public static bool checkApplicationLogin(string loginName, string loginPassword)
        {
            int userId = 0;

            // open db..
            cData db = new cData();
            db.openDB();

            userId = db.getUserIDFromLogin(loginName, loginPassword, "application");
            loadUser(userId);

            // audit trail..
            if (userId == 0)
                db.auditEvent(cConstants.eventUserLoginFailed, 0, loginName);  // login failed..
            else
                db.auditEvent(cConstants.eventUserLogin, 0, "Application Login");  // login successful
            
            db.closeDB();   // cleanup
            
            return (userId != 0);
        }
        
        public static int userID()
        {
            return cTools.cIntExt(HttpContext.Current.Session["user.id"]);
        }

        public static WebUser getUser()
        {
            // no longer using session as the photos change and I want this updated every time..
            WebUser webUser = new WebUser();

            cData db = new cData();
            if (db.openDB())
            {
                webUser = db.loadUser(userID());
                db.closeDB();   // cleanup
            }

            return (webUser);
        }
    }
}
