using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;

using System.Configuration; // so I can access web.config
using System.Globalization; //date formats
using System.Web.UI;

namespace SurveyApp
{
    public class cTools
    {
        #region DataType Conversions
        // convert an object to Yes or No depending on its value
        public static string cBoolToYN(object dataVal)
        {
            bool isYes = false;

            if (dataVal is bool)
            {
                if ((bool)dataVal) isYes = true;
            }
            else if (dataVal is string)
            {
                if (dataVal.ToString().ToUpper() == "Y" || dataVal.ToString().ToUpper() == "TRUE") isYes = true;
            }

            if (isYes)
                return ("Yes");
            else
                return ("No");
        }

        
        public static int cBoolTo1Or0(object dataVal)
        {
            bool isYes = false;

            if (dataVal is bool)
            {
                if ((bool)dataVal) isYes = true;
            }
            else if (dataVal is string)
            {
                if (dataVal.ToString().ToUpper() == "Y" || dataVal.ToString().ToUpper() == "TRUE") isYes = true;
            }

            if (isYes)
                return (1);
            else
                return (0);
        }

        public static string cDateExt(object obj)
        {
            string rtnVal = "";
            DateTime dateObj;

            if (obj != null)
            {
                if (!DBNull.Value.Equals(obj))
                {

                    dateObj = (DateTime)obj;
                    rtnVal = dateObj.ToString("MMM dd, yyyy - HH:mm"); // http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
                }
            }

            return (rtnVal);
        }

        public static string cDateOnly(object obj)
        {
            string rtnVal = "";
            DateTime dateObj;

            if (obj != null)
            {
                if (!DBNull.Value.Equals(obj))
                {
                    if (obj.ToString() != "")
                    {
                        dateObj = (DateTime)obj;
                        rtnVal = dateObj.ToString("dd-MMM-yyyy"); // http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
                    }
                }
            }

            return (rtnVal);
        }

        public static string cDateAndShortTime(object obj)
        {
            string rtnVal = "";
            DateTime dateObj;

            if (obj != null)
            {
                if (!DBNull.Value.Equals(obj))
                {
                    if (obj.ToString() != "")
                    {
                        dateObj = (DateTime)obj;
                        rtnVal = dateObj.ToString("dd-MMM-yyyy H:mm"); // http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
                    }
                }
            }

            return (rtnVal);
        }

        public static string cArdentDate(object obj)
        {
            string rtnVal = "";

            if (obj.ToString() != "")
            {
                try
                {
                    DateTime dateObj = Convert.ToDateTime(obj);
                    rtnVal = dateObj.ToString("dd-MMM-yyyy"); // http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
                }
                catch (Exception e)
                {
                    cTools.log("** ERROR ** cArdentDate() - " + e.Message);
                }

                if (rtnVal.IndexOf("1900") != -1)
                    rtnVal = "";
            }
            return (rtnVal);
        }

        public static string cArdentUserDate(string dateValue)
        {
            string rtnDate = dateValue;     //default to return what was input
            string pattern = "MM/dd/yyyy";  // the expected pattern the user will enter
            DateTime parsedDate;

            if (DateTime.TryParseExact(dateValue, pattern, null, DateTimeStyles.None, out parsedDate))
                rtnDate = parsedDate.ToString("dd-MMM-yyyy");   // return the format I want for the db

            return (rtnDate);
                
        }

        public static string cDateToDateTimePickerFormat(string dateValue)
        {
            string rtnVal = "";

            if (dateValue != "")
            {
                try
                {
                    DateTime dateObj = Convert.ToDateTime(dateValue);
                    rtnVal = dateObj.ToString("dd MMM yyyy"); // http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
                }
                catch (Exception e)
                {
                    cTools.log("** ERROR ** cDateToDateTimePickerFormat() - " + e.Message);
                }

                if (rtnVal.IndexOf("1900") != -1)
                    rtnVal = "";
            }
            return (rtnVal);
        }

        // if something is null, just set it to ""
        public static string cStrExt(object obj)
        {
            string rtnVal = "";

            if (obj != null)
            {
                if (!DBNull.Value.Equals(obj))
                {
                    rtnVal = obj.ToString();
                }
            }

            return (rtnVal);
        }

        public static int cIntExt(object obj)
        {
            int rtnVal = 0;

            try
            {
                if (obj != null)
                {
                    if (!DBNull.Value.Equals(obj))
                    {
                        if (obj.ToString() != "")
                        {
                            rtnVal = Convert.ToInt32(obj);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // thats ok..just means the input wasn't a number
            }
            return (rtnVal);
        }

        #endregion

        #region Grid Management
        public static void addBlankRowtoGrid(GridView oGrid, int rowPos)
        {
            // create an empty grid row that we will add later..
            GridViewRow newGridRow = new GridViewRow(-1, -1, DataControlRowType.DataRow, DataControlRowState.Normal);

            // create an array of cells to match what's in the grid at the moment..
            for (int i = 0; i < oGrid.HeaderRow.Cells.Count; i++)
            {
                TableCell newCell = new TableCell();
                newCell.Text = "-";

                // add each cell to the new row
                newGridRow.Cells.Add(newCell);
            }

            // put the row into the grid
            oGrid.Controls[0].Controls.AddAt(rowPos, newGridRow);
        }

        #endregion

        #region Handy Tools

        /*
         * ================================================
         *          SOME HANDY SUPPORTING FUNCTIONS 
         * ================================================
         */

        public static void addJavascriptToPage(string clientScript)
        {
            // get the current page handler
            var page = HttpContext.Current.CurrentHandler as Page;

            ClientScriptManager csm = page.ClientScript;
            string script = "$(document).ready(function () { " +
                clientScript +
                "});";
            csm.RegisterClientScriptBlock(page.GetType(), "clientScript", script, true);

        }

        public static void autoPrintPage()
        {
            string jsAutoPrintPage = "window.print();";
            addJavascriptToPage(jsAutoPrintPage);
        }

        public static string iif(bool testCondition, string trueVal, string falseVal = "")
        {
            if (testCondition)
                return (trueVal);
            else
                return (falseVal);
        }
        public static string repeatStr(string strToRepeat, int numRepeats)
        {
            string rtnVal = "";

            for (var i = 1; i < numRepeats; i++)
            {
                rtnVal += strToRepeat;
            }
            return (rtnVal);
        }

        public static string cAlphaNumeric(string str, bool allowSpaces = false, string replaceSpaceChar = "")
        {
            // 3 x quicker than regex!
            char[] arr = str.ToCharArray();
            if (allowSpaces)
            {
                arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
                                                  || char.IsWhiteSpace(c)
                                                  || c == '-')));
            }
            else
            {
                arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
                                              || c == '_')));
            }
            str = new string(arr);

            if (allowSpaces && replaceSpaceChar != "")
                str = str.Replace(" ", replaceSpaceChar);

            return (str);
        }

        public static void autoSelectListItem(DropDownList oCombo, string itemVal)
        {
            oCombo.SelectedIndex = -1;  // reset previously selected item..

            foreach (ListItem oVal in oCombo.Items) {
                if (oVal.Value == itemVal)
                {
                    oVal.Selected = true;
                    return; //get outta here!
                }
            }
        }

        public static void dropdownReplaceAll(DropDownList oCombo, string searchText, string replaceText)
        {
            foreach (ListItem oVal in oCombo.Items)
            {
                oVal.Text = oVal.Text.Replace(searchText, replaceText);
            }
        }

        public static void autoSelectRadioItem(RadioButtonList oRadioList, string itemVal)
        {
            oRadioList.SelectedIndex = -1;  // reset previously selected item..

            foreach (ListItem oVal in oRadioList.Items)
            {
                if (oVal.Value == itemVal)
                {
                    oVal.Selected = true;
                    return; //get outta here!
                }
            }
        }
        public static string convertCrToBr(string str)
        {
            return (str.Replace(Environment.NewLine, "<br />"));
        }

        // find a block in a larger source text and replace the block with a new one (or append)
        public static string replaceBlock(string sourceText, string startKey, string endKey, string newBlock, string mustIncludeText = "")
        {
            int startPos, endPos, loopCheck, newStartPos;
            int maxLoops = 99;
            string tempSource, tempNewBlock;
            string blockText, searchText;
            bool isOkToReplaceBlock;

            loopCheck = 0;
            newStartPos = 0;
            do
            {
                isOkToReplaceBlock = true;  // always true unless set to false..

                startPos = sourceText.IndexOf(startKey, newStartPos);
                if (startPos > -1)
                    endPos = sourceText.IndexOf(endKey, startPos + 1);
                else
                    endPos = -1;

                if (startPos > -1 && endPos > -1)
                {
                    // get me the block itself
                    blockText = midStr(sourceText, startPos+1, ((endPos - startPos)+1));

                    /*
                     * we can specify text that the block must include or must not include..using the ! prefix to determine if it's not to include..
                     */
                    searchText = mustIncludeText;
                    if (searchText != "") {
                        //let's see if it's an include or an exclude..
                        if (searchText.IndexOf("!") == 0) {
                            //must NOT include the search term
                            searchText = midStr(searchText, 2);
                            //must include the search term
                            if (blockText.IndexOf(searchText) != -1)
                                isOkToReplaceBlock = false; // search text does exist..skip this block..  

                        } else  {
                            //must include the search term
                            if (blockText.IndexOf(searchText) == -1)
                                isOkToReplaceBlock = false; // search text does not exist..skip this block..  
                        }
                    }

                     // we have an option to check if the block must include a certain term..
                    if (isOkToReplaceBlock) { 

                        // parse the target block for any tags..
                        tempNewBlock = newBlock;
                        tempNewBlock = tempNewBlock.Replace("%block%", blockText);

                        // limit length of text..
                        for (int limitChars = 10; limitChars <= 100; limitChars++)
                        {
                            string limitText = blockText;
                            string tagText = "%block-limit" + limitChars.ToString() + "%";
                            if (limitText.Length > limitChars)
                                limitText = leftStr(blockText, limitChars) + "...";
                            tempNewBlock = tempNewBlock.Replace(tagText, limitText);
                        }

                        // now remove the block and replace with the target content 
                        tempSource = leftStr(sourceText, startPos) +
                            tempNewBlock +
                            midStr(sourceText, endPos + lenStr(endKey) + 1);

                        sourceText = tempSource;

                        // start looking from the END of the new block of text..
                        newStartPos = startPos + lenStr(tempNewBlock);
                    } else {
                        // start looking from the END of the current block of text..
                        newStartPos = startPos + lenStr(blockText);
                    }

                    if (newStartPos > lenStr(sourceText))
                        break;
                }
                else
                {
                    //handy for testing..
                    //sourceText += "*** WARNING - No matching items found in Module cTools.replaceBlock() ***";
                    break;
                }

                loopCheck++;

            } while (loopCheck < maxLoops);

            if (loopCheck >= maxLoops)
                sourceText += "*** ERROR - Max Loops Reached in Module cTools.replaceBlock() ***";

            return (sourceText);
        }

        public static string leftStr(string str, int count)
        {
            if (string.IsNullOrEmpty(str) || count <= 0)
                return string.Empty;
            else
                return str.Substring(0, Math.Min(count, str.Length));
        }

        // get me the mid of a string (last param is optional)
        // Note: substring is ZERO-based but my midStr is 1-based..
        public static string midStr(string str, int startPos, int lengthOfStringToInclude = 0)
        {
            if (string.IsNullOrEmpty(str) || startPos <= 0 || startPos > lenStr(str))
            {
                return string.Empty;
            }
            else
            {
                //log("running module midStr..str:" + str + ", startpos:" + startPos.ToString() + ", lenStr(str)=" + lenStr(str).ToString());

                if (lengthOfStringToInclude != 0)
                    return str.Substring(startPos - 1, lengthOfStringToInclude); // the +1 is to convert from 1-based to zero-based
                else
                    return str.Substring(startPos - 1);
            }
        }

        // 1-based NOT zero-based!
        public static int lenStr(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;
            else
                return (str.Length);
        }

        public static string[] getBlocks(string sourceText, string startKey, string endKey)
        {
            int startPos, endPos, loopCheck;
            int maxLoops = 99;
            int currIndex = 0;
            string[] textBlocks;
            textBlocks = new string[maxLoops];

            loopCheck = 0;
            do
            {
                // find me a match for the searchtext in the source..
                startPos = sourceText.IndexOf(startKey);
                if (startPos > -1)
                    endPos = sourceText.IndexOf(endKey, startPos + 1);
                else
                    endPos = -1;

                // did you find a match?
                if (startPos > -1 && endPos > -1)
                {
                    // give me all the text before the block + the block itself
                    textBlocks[currIndex++] = leftStr(sourceText, startPos);                                // give me all the text before the block
                    textBlocks[currIndex++] = midStr(sourceText, startPos + 1, ((endPos - startPos) + 1));  // give me the block itself

                    // now remove both from the original source and we start looking again on the next loop...
                    sourceText = midStr(sourceText, endPos + lenStr(endKey) + 1);
                }
                else
                {
                    // no more matches for the search string..let's get outta here and clean up..
                    break;
                }

                loopCheck++;

            } while (loopCheck < maxLoops);

            // remember that while we went thru the loop, we added the pretext and the block but never the post-text, that needs to be done now..
            textBlocks[currIndex++] = sourceText;

            if (loopCheck >= maxLoops)
                textBlocks[currIndex++] = "*** ERROR - Max Loops Reached in Module cTools.replaceBlock() ***";

            // resize the array to just the items I need..
            Array.Resize(ref textBlocks, currIndex);

            return (textBlocks);
        }

        public static void splitText(string sourceText, string delimText, ref string firstPart, ref string secondPart)
        {
            int startPos;
            firstPart = "";
            secondPart = "";

            // find me a match for the searchtext in the source..
            startPos = sourceText.IndexOf(delimText);

            if (startPos > -1)
            {
                firstPart = leftStr(sourceText, startPos);      
                secondPart = midStr(sourceText, startPos + 2);  
            }
            else
            {
                firstPart = sourceText;
            }

        }


        public static string leftStrFindByText(string sourceText, string searchText)
        {
            int startPos;
            string textPart = "";
            
            // find me a match for the searchtext in the source..
            startPos = sourceText.IndexOf(searchText);

            if (startPos > -1)
                textPart = leftStr(sourceText, startPos);
            else
                textPart = sourceText;
            
            return textPart;
        }
        #endregion

        #region Logging

        public static void logException(Exception ex)
        {
            string errorTitle = "An error has been detected";
            string errorText = ex.Message;

            if (ex.InnerException != null)
                errorTitle = ex.InnerException.Message;
            log(errorTitle + "\r\n" + errorText);
        }

        // log to a file..
        public static void log(string logText)
        {
            bool isLoggingEnabled = (cTools.cStrExt(ConfigurationManager.AppSettings["Logging"]).ToLower() == "yes");
            if (!isLoggingEnabled)
                return;
            
            string logPath = HttpContext.Current.Server.MapPath("~/App_Data");
            string logFilename = String.Format("{0}/applog_{1}.txt", logPath, DateTime.Now.ToString("ddMMMyyyy"));   // changes daily
            string outText;

            // add the sessionid, datestamp, some tabs & newline char so I can read the file!
            //   http://stackoverflow.com/questions/366124/inserting-a-tab-character-into-text-using-c-sharp
            outText = String.Format("{0}\t{1}\t{2}\t{3}\r\n",
                HttpContext.Current.Session.SessionID,
                HttpContext.Current.Request.UserHostAddress,
                DateTime.Now.ToString("F"),
                logText);

            try
            {
                StreamWriter fileWriter = new StreamWriter(logFilename, true);  // last param makes it append to the file..
                fileWriter.Write(outText);
                fileWriter.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }
        #endregion
        
    }
}