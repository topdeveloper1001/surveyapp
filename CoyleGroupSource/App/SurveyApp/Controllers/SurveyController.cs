using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SurveyApp.Controllers
{
    public class SurveyController : ApiController
    {
        
        public string GetServeyList(int UserId)
        {
            cData m_data = new cData();
            if (!m_data.openDB())
            {

            }
            WebUser webUser = m_data.loadUser(UserId);

            string locationIdParam = "null";    // see audits for all locations
            string statusIdParam1 = string.Empty;
            string statusIdParam2 = string.Empty;
            string userIdParam = string.Empty;

            // show the list of pending surveys on the home page..
            if (webUser.isadmin || webUser.issuperadmin)        // admins 
            {
                statusIdParam1 = "null"; //admins see all jobs regardless of status
                statusIdParam2 = "null";
                userIdParam = "null";        // null = see all audits for all locations (admin-only)
            }
            else if (webUser.isnewhire)                         // contractors
            {
                statusIdParam1 = cConstants.statusDraft.ToString();
                statusIdParam2 = cConstants.statusDraft.ToString();
                userIdParam = webUser.userid.ToString();
            }
            else if (webUser.isleader)                          // clients
            {
                statusIdParam1 = cConstants.statusForClientReview.ToString();
                statusIdParam2 = cConstants.statusReleased.ToString();
                userIdParam = webUser.userid.ToString();
            }
            else
            {
                // this could be if the user who's logged in doesn't have a role associated with them..
                userIdParam = "-1";        // null = see all audits for all locations (admin-only)
            }

            
            string sqlCmd = "spGetSurveyList " + locationIdParam + ", " + userIdParam + ", " + statusIdParam1 + ", " + statusIdParam2;
            
            string res = m_data.bindToJsonString(sqlCmd);
            m_data.closeDB();

            return res;
            
        }

        
    }
}
