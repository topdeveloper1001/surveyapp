using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SurveyApp
{
    public static class cPushNotifications
    {
       
        public static void handleEvent(int eventId, int surveyId)
        {
            // open up the db 
            cData m_data = new cData();
            if (!m_data.openDB())
                return;

            /*
                load up the audit details & convert id's to text
            */
            SurveySummaryData survey = m_data.loadSurvey(surveyId);
            LocationData location = m_data.loadLocation(survey.locationid);
            ClientData client = m_data.loadClient(location.clientid);

            // convert id's to text
            string sqlCmd = "SELECT templatename FROM templates WHERE templateid=" + survey.templateid;
            string templateName = m_data.runSql(sqlCmd);
            string jobRef = client.clientname + " - " + location.businessunit + " - " + location.locationname + " - " + templateName + " - " + survey.dateofaudit;
            

            /*
                Set up the email, SMTP settings and links to the job   
             */
            //load up the standard template file.
            string emailTemplate = loadTemplateFile();
            if (emailTemplate == string.Empty)
                return;

            // get our email server settings..
            string emailSenderName = cTools.cStrExt(ConfigurationManager.AppSettings["SenderName"]);
            string emailSenderEmail = cTools.cStrExt(ConfigurationManager.AppSettings["SenderEMail"]);
            string emailRootFolder = cTools.cStrExt(ConfigurationManager.AppSettings["EmailFolder"]);
            int notifyUserOnSendToHQ = cTools.cIntExt(ConfigurationManager.AppSettings["NotifyUserOnSendToHQ"]);

            string emailSubjectLine = string.Empty;
            string emailHtml =  string.Empty;
            string notificationsBlock = string.Empty;
            string sendToEmail = string.Empty;
            string sendToName = string.Empty;
            string siteUrl = "http://" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
            if (siteUrl.Contains("localhost"))
                siteUrl += "/coylegroup";   // dev server
            string linkToJob = siteUrl + "/audit/summary/" + surveyId;  // all notification links go to summary page..

            /*
                Customise the email depending on what action was just taken
             */
            switch (eventId)
            {
                case cConstants.eventSurveyConsultantChanged_Notification:
                    emailSubjectLine = "Audit Notification: Contractor Assigned";
                    notificationsBlock = "You have been assigned as the <b>Contractor</b> for the following audit.";
                    if (survey.auditorid != 0)
                    {
                        WebUser userDetails = m_data.loadUser(survey.auditorid);
                        sendToEmail = userDetails.emailaddress;
                        sendToName = userDetails.firstname;
                    }
                    break;

                case cConstants.eventQuestionSubmitToHQ_Notification:
                    emailSubjectLine = "Audit Notification: For Internal Review";
                    notificationsBlock = "This audit has been <b>Sent to HQ</b> for internal review.";
                    if (notifyUserOnSendToHQ != 0)
                    {
                        WebUser userDetails = m_data.loadUser(notifyUserOnSendToHQ);
                        sendToEmail = userDetails.emailaddress;
                        sendToName = userDetails.firstname;
                    }
                    break;

                case cConstants.eventSurveyStatusReadyForClientReview_Notification:
                    emailSubjectLine = "Audit Notification: Pending Client Approval";
                    notificationsBlock = "An audit is available <b>Pending Client Approval</b>";
                    if (survey.client_mgruserid != 0)
                    {
                        WebUser userDetails = m_data.loadUser(survey.client_mgruserid);
                        sendToEmail = userDetails.emailaddress;
                        sendToName = userDetails.firstname;
                    }
                    break;

                case cConstants.eventClientApprovalAccept_Notification:
                    emailSubjectLine = "Audit Notification: Approved by Client";
                    notificationsBlock = "This audit has been <b>Approved by the Client</b>.";
                    if (survey.client_approvalcomments != "")
                    {
                        notificationsBlock += "<br><br>Client Comments:<br>" +
                            "<i>" + survey.client_approvalcomments + "</i>";
                    }

                    if (notifyUserOnSendToHQ != 0)
                    {
                        WebUser userDetails = m_data.loadUser(notifyUserOnSendToHQ);
                        sendToEmail = userDetails.emailaddress;
                        sendToName = userDetails.firstname;
                    }
                    break;

                case cConstants.eventClientApprovalReject_Notification:
                    emailSubjectLine = "Audit Notification: Rejected by Client";
                    notificationsBlock = "This audit has been <b>Rejected by the Client</b>.";
                    if (survey.client_approvalcomments != "")
                    {
                        notificationsBlock += "<br><br>Client Comments:<br>" +
                            "<i>" + survey.client_approvalcomments + "</i>";
                    }
                    if (notifyUserOnSendToHQ != 0)
                    {
                        WebUser userDetails = m_data.loadUser(notifyUserOnSendToHQ);
                        sendToEmail = userDetails.emailaddress;
                        sendToName = userDetails.firstname;
                    }
                    break;

                case cConstants.eventClientResponsesClosed_NotifyClientMgr:
                    emailSubjectLine = "Audit Notification: All Client Actions Closed";
                    notificationsBlock = "Congratulations - All action items have been <b>closed</b> on this audit.";
                    if (survey.client_mgruserid != 0)
                    {
                        WebUser userDetails = m_data.loadUser(survey.client_mgruserid);
                        sendToEmail = userDetails.emailaddress;
                        sendToName = userDetails.firstname;
                    }
                    break;

                case cConstants.eventClientResponsesClosed_NotifyClientAdmin:
                    emailSubjectLine = "Audit Notification: All Client Actions Closed";
                    notificationsBlock = "Congratulations - All action items have been <b>closed</b> on this audit.";
                    // don't send if no admin user set or if it's the same as the manager..
                    if (survey.client_adminuserid != 0 && survey.client_adminuserid != survey.client_mgruserid)
                    {
                        WebUser userDetails = m_data.loadUser(survey.client_adminuserid);
                        sendToEmail = userDetails.emailaddress;
                        sendToName = userDetails.firstname;
                    }
                    break;
            }

            // while testing... 
            // sendToName += " (" + sendToEmail + ")";
            // sendToEmail = "joebloggs@crystalsoft.ie";

            // if no email associated with this task then off ye go..
            if (sendToEmail == string.Empty)
                return;

            // record in the audit trail that we've handled this notification
            string auditEntry = string.Format("To: {0} ({1})\n{2}",
                sendToName, sendToEmail, notificationsBlock);
            m_data.auditEvent(eventId, surveyId, auditEntry);
            
            // start with a fresh template & replace with content for this user
            emailHtml = emailTemplate;
            emailHtml = emailHtml.Replace("%sendtoname%", sendToName);
            emailHtml = emailHtml.Replace("%content%", notificationsBlock);
            emailHtml = emailHtml.Replace("%datestamp%", cTools.cDateAndShortTime(DateTime.Now));
            emailHtml = emailHtml.Replace("%linktojob%", linkToJob);
            emailHtml = emailHtml.Replace("%jobref%", jobRef);

            // set up today's email folder..
            string folderName = String.Format("{0}/{1}",
                emailRootFolder,
                DateTime.Now.ToString("ddMMMyyyy"));   // folder changes daily
            System.IO.Directory.CreateDirectory(folderName);

            string mailFilename = String.Format("{0}/notifications_{1}.htm",
                folderName,
                sendToEmail);                         // append all emails for person per day to one file

            // write the file to the local server for later retrieval
            cFiles.writeFile(mailFilename, emailHtml, true);

            // send the email now..we store the output in a subfolder in the logs if enabled
            cTools.log("Sending notification email to: " + sendToEmail);

            // send the email now..unless it's on my dev machine..
            if (!siteUrl.Contains("localhost"))
                cMailer.sendMail(sendToName, sendToEmail, emailSenderName, emailSenderEmail, emailSubjectLine, emailHtml);

            // send me a copy..
            cMailer.sendMail("Gary Alerts", "gary@crystalsoft.ie", emailSenderName, emailSenderEmail, emailSubjectLine, emailHtml);

            // cleanup
            m_data.closeDB();
        }

        private static string loadTemplateFile()
        {
            string emailTemplate = string.Empty;
            string emailTemplateFile = cTools.cStrExt(ConfigurationManager.AppSettings["PushNotificationsTemplate"]);
            
            // check & load up the email template file..
            if (emailTemplateFile != string.Empty)
                if (cFiles.fileExists(emailTemplateFile))
                {
                    emailTemplate = cFiles.loadFile(emailTemplateFile);
                    if (emailTemplate == string.Empty)
                        cTools.log("Template file not valid or blank: " + emailTemplateFile);
                }
                else
                    cTools.log("Template file not found at: " + emailTemplateFile);
            else 
                cTools.log("Location of template file not set in app.config.");

            return (emailTemplate);
        }
    }
}