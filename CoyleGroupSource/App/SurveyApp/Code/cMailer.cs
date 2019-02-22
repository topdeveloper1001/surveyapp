using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

using System.Configuration;

namespace SurveyApp
{
    public class cMailer
    {
        private static string m_hostname = "";
        private static string m_username = "";
        private static string m_password = "";
        private static bool m_usessl = false;
        private static int m_portnum = 0;


        // http://docs.aws.amazon.com/ses/latest/DeveloperGuide/send-using-smtp-net.html

        private static void loadApplicationSettings()
        {
            // email settings
            m_hostname = cTools.cStrExt(ConfigurationManager.AppSettings["EmailServer"]);
            m_username = cTools.cStrExt(ConfigurationManager.AppSettings["EmailUsername"]);
            m_password = cTools.cStrExt(ConfigurationManager.AppSettings["EmailPassword"]);
            m_usessl = (cTools.cStrExt(ConfigurationManager.AppSettings["EmailUseSSL"]).ToLower() == "yes");
            m_portnum = cTools.cIntExt(ConfigurationManager.AppSettings["EmailPort"]);
        }
        public static bool sendMail(string toName, string toEmail, string fromName, string fromEmail, string mailSubject, string mailBody)
        {
            bool rtnVal = false;

            // initialise this component on startup..
            if (m_hostname == "" || m_username == "")
                loadApplicationSettings();

            // check we've got all the settings we need now..blank pwd is ok
            if (m_hostname=="" || m_username=="")
            {
                cTools.log("No host name and/or login details provided for the mailer component");
                return(false);
            }

            // Create an SMTP client with the specified host name and port.
            //      Port 587 for the Amazon SES SMTP endpoint. 
            //          because we will use STARTTLS to encrypt the connection.
            //      Port 25 for Office 365
            using (SmtpClient client = new SmtpClient(m_hostname, m_portnum))
            {
                // Create a network credential with your SMTP user name and password.
                client.Credentials = new System.Net.NetworkCredential(m_username, m_password);

                // Use SSL when accessing Amazon SES.
                // no SSL for outlook.com/Office365
                client.EnableSsl = m_usessl;
                try
                {
                    // http://msdn.microsoft.com/en-us/library/System.Net.Mail.SmtpClient(v=vs.110).aspx
                    // set up the mail format
                    MailAddress fromAddress = new MailAddress(fromEmail, fromName);
                    MailAddress toAddress = new MailAddress(toEmail, toName);
                    MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
                    mailMessage.Body = mailBody;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Subject = mailSubject;

                    //  Send the email. 
                    client.Send(mailMessage);
                    rtnVal = true;
                }
                catch (Exception ex)
                {
                    cTools.logException(ex);
                }
            }

            return (rtnVal);
        }

    }
}
