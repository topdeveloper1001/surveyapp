using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SurveyApp
{
    public class cMsgBox
    {
        // javascript
        
        private const string jsSaveError = "alert('There was an error saving your changes. Please notify support immediately');";
        private const string jsShowDialog = "bootbox.alert({{ message: \"{0}\", backdrop: true }});";


        public static void saveUserDialog(string msgText)
        {
            HttpContext.Current.Session["UserDialog"] = msgText;
        }

        public static string getUserDialog()
        {
            return cTools.cStrExt(HttpContext.Current.Session["UserDialog"]);
        }

        public static void displayPendingDialogs(string userMsg = "")
        {
            // we can set it on the cmd-line or if that's blank, load up a saved msg
            if (userMsg == string.Empty)
                userMsg = getUserDialog();

            // if there's anything to display.. carry on
            if (userMsg != string.Empty)
            {
                string jsDialog = string.Format(jsShowDialog, userMsg);
                cTools.addJavascriptToPage(jsDialog);

                saveUserDialog(string.Empty);   // clear last msg
            }
        }

        public static void showSaveErrorMsg()
        {
            cTools.addJavascriptToPage(jsSaveError);
        }
    }
}