using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.UI;
using System.Web.UI.WebControls;    // so we can access the repeater controls

namespace SurveyApp
{
    public class cRepeater
    {

        #region Repeater Controls - Getters

        //  format an item of data in a repeater control a variety of ways
        //      eg: yn, date
        public static void formatControlVal(RepeaterItemEventArgs e, string dataVarName, string ctrlName, string formatType)
        {
            // get me the data for the control first..
            object dataItemVal = getDataVal(e, dataVarName);

            switch (formatType)
            {
                case "date":
                    setLabelVal(e, ctrlName, cTools.cDateExt(dataItemVal));
                    break;

                case "yn":
                    setLabelVal(e, ctrlName, cTools.cBoolToYN(dataItemVal));
                    break;
            }
        }

        // get me the databound field value for a repeater and return the value in a specific datatype format using a conversion
        //  last param is optional
        public static object getDataVal(RepeaterItemEventArgs e, string varName, string dataTypeToReturn = "")
        {
            object dataItemVal = System.Web.UI.DataBinder.Eval(e.Item.DataItem, varName);

            // we can use this function to get the data value and return it in it's native format (the "default") 
            // or we can convert it to another format if specified
            switch (dataTypeToReturn)
            {
                case "int":
                    return (cTools.cIntExt(dataItemVal));

                case "string":
                    return (cTools.cStrExt(dataItemVal));

                default:
                    return (dataItemVal);
            }
        }

        public static string getTextboxVal(RepeaterItem item, string ctrlName)
        {
            return (cTools.cStrExt(((TextBox)item.FindControl(ctrlName)).Text));
        }

        public static string getHiddenField(RepeaterItem item, string ctrlName)
        {
            return (cTools.cStrExt(((HiddenField)item.FindControl(ctrlName)).Value));
        }

        public static Control getControl(RepeaterItem item, string ctrlName)
        {
            Control ctrl = (Control)item.FindControl(ctrlName);
            return (ctrl);
        }

        public static LinkButton getLinkButton(RepeaterItem item, string ctrlName)
        {
            LinkButton lnkTarget = (LinkButton)item.FindControl(ctrlName);
            return (lnkTarget);
        }

        public static string getDropdownVal(RepeaterItem item, string ctrlName)
        {
            return (cTools.cStrExt(((DropDownList)item.FindControl(ctrlName)).SelectedValue));
        }

        public static string getRadioButtonListVal(RepeaterItem item, string ctrlName)
        {
            return (cTools.cStrExt(((RadioButtonList)item.FindControl(ctrlName)).SelectedValue));
        }

        public static string getCheckboxYNVal(RepeaterItem item, string yesCtrlName, string noCtrlName)
        {
            string sourceVal = "";

            CheckBox sourceValYes = (CheckBox)(item.FindControl(yesCtrlName));
            CheckBox sourceValNo = (CheckBox)(item.FindControl(noCtrlName));

            //check that something has actually been selected..if not, returns blank
            if (sourceValYes.Checked)
            {
                sourceVal = "Y";
            }
            else if (sourceValNo.Checked)
            {
                sourceVal = "N";
            }

            return (sourceVal);
        }

        #endregion

        #region Repeater Controls - Setters
        public static void setItemLabelVal(RepeaterItem item, string ctrlName, string dataVal)
        {
            Label lblTarget = (Label)(item.FindControl(ctrlName));
            lblTarget.Text = dataVal;
        }
        
        public static void setItemLabelStyle(RepeaterItem item, string ctrlName, string cssClass)
        {
            Label lblTarget = (Label)(item.FindControl(ctrlName));
            lblTarget.CssClass = cssClass;
        }

        public static void setLabel(RepeaterItemEventArgs e, string ctrlName, string fieldName)
        {
            Label lblTarget = (Label)(e.Item.FindControl(ctrlName));
            lblTarget.Text = cTools.cStrExt(System.Web.UI.DataBinder.Eval(e.Item.DataItem, fieldName));
        }

        public static void setLabelBr(RepeaterItemEventArgs e, string ctrlName, string fieldName)
        {
            // replaces cr with br
            Label lblTarget = (Label)(e.Item.FindControl(ctrlName));
            lblTarget.Text = cTools.cStrExt(System.Web.UI.DataBinder.Eval(e.Item.DataItem, fieldName)).Replace("\n", "<br />");
        }

        public static void setLabelVal(RepeaterItemEventArgs e, string ctrlName, string dataVal)
        {
            Label lblTarget = (Label)(e.Item.FindControl(ctrlName));
            lblTarget.Text = dataVal;
        }

        public static void setTextbox(RepeaterItemEventArgs e, string ctrlName, string fieldName)
        {
            TextBox txtTarget = (TextBox)(e.Item.FindControl(ctrlName));
            txtTarget.Text = cTools.cStrExt(System.Web.UI.DataBinder.Eval(e.Item.DataItem, fieldName));
        }

        public static void setHiddenField(RepeaterItemEventArgs e, string ctrlName, string fieldName)
        {
            HiddenField txtTarget = (HiddenField)(e.Item.FindControl(ctrlName));
            txtTarget.Value = cTools.cStrExt(System.Web.UI.DataBinder.Eval(e.Item.DataItem, fieldName));
        }

        public static void setItemHiddenField(RepeaterItem item, string ctrlName, string dataVal)
        {
            HiddenField txtTarget = (HiddenField)(item.FindControl(ctrlName));
            txtTarget.Value = dataVal;
        }

        public static void setLinkbuttonStyle(RepeaterItemEventArgs e, string ctrlName, string dataVal)
        {
            LinkButton lnkTarget = (LinkButton)(e.Item.FindControl(ctrlName));
            lnkTarget.CssClass = dataVal;
        }
        
        public static void setImageSource(RepeaterItemEventArgs e, string ctrlName, string dataVal)
        {
            Image imgTarget = (Image)(e.Item.FindControl(ctrlName));
            imgTarget.ImageUrl = dataVal;
        }

        public static void setPanelVisibility(RepeaterItemEventArgs e, string ctrlName, bool isVisible)
        {
            Panel pnlTarget = (Panel)(e.Item.FindControl(ctrlName));
            pnlTarget.Visible = isVisible;
        }

        public static void setPanelStyle(RepeaterItemEventArgs e, string ctrlName, string cssClass)
        {
            Panel pnlTarget = (Panel)(e.Item.FindControl(ctrlName));
            pnlTarget.CssClass = cssClass;
        }

        public static void setPlaceholderVisibility(RepeaterItemEventArgs e, string ctrlName, bool isVisible)
        {
            PlaceHolder phTarget = (PlaceHolder)(e.Item.FindControl(ctrlName));
            phTarget.Visible = isVisible;
        }
        // from an sql command, build up a dropdown combo
        public static void setDropdown(RepeaterItemEventArgs e, string ctrlName, string fieldName, string sqlCmd, cData oData, string sBlankOptionText = "", string sBlankOptionVal = "")
        {
            // get a handle on the dropdown
            DropDownList cboTarget = (DropDownList)e.Item.FindControl(ctrlName);

            // run the sql command and build up our dropdown list..
            oData.createDropdown(sqlCmd, cboTarget, sBlankOptionText, sBlankOptionVal);

            // now auto-select the value for this dropdown from the db
            string selectedVal = cTools.cStrExt(System.Web.UI.DataBinder.Eval(e.Item.DataItem, fieldName));
            cTools.autoSelectListItem(cboTarget, selectedVal);
        }

        public static void setRadioButtonList(RepeaterItemEventArgs e, string ctrlName, string fieldName)
        {
            // get a handle on the dropdown
            RadioButtonList oTarget = (RadioButtonList)e.Item.FindControl(ctrlName);

            // now auto-select the value for this dropdown from the db
            string selectedVal = cTools.cStrExt(System.Web.UI.DataBinder.Eval(e.Item.DataItem, fieldName));
            cTools.autoSelectRadioItem(oTarget, selectedVal);
        }

        public static void setRadioButtonListVal(RepeaterItemEventArgs e, string ctrlName, string varData)
        {
            // get a handle on the dropdown
            RadioButtonList oTarget = (RadioButtonList)e.Item.FindControl(ctrlName);

            // now auto-select the value for this dropdown from the db
            cTools.autoSelectRadioItem(oTarget, varData);
        }

        public static void setControlVal(RepeaterItemEventArgs e, string ctrlName, string varData)
        {
            Label lblTarget = (Label)(e.Item.FindControl(ctrlName));
            lblTarget.Text = varData;
        }
        public static void setHyperLinkTarget(RepeaterItemEventArgs e, string ctrlName, string targetURL, bool popupNewWindow = false)
        {
            HyperLink oTarget = (HyperLink)(e.Item.FindControl(ctrlName));
            oTarget.NavigateUrl = targetURL;
            
            if (popupNewWindow)
                oTarget.Target = "_blank";
        }

        public static void setHyperLinkVisibility(RepeaterItemEventArgs e, string ctrlName, bool visible)
        {
            HyperLink oTarget = (HyperLink)(e.Item.FindControl(ctrlName));
            oTarget.Visible = visible;
        }

        public static string getHyperLinkClientID(RepeaterItemEventArgs e, string ctrlName)
        {
            HyperLink oTarget = (HyperLink)(e.Item.FindControl(ctrlName));
            return (oTarget.ClientID);
        }
            #endregion

        }
    }