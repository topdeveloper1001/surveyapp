using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SurveyApp
{
    public partial class AdminTool : System.Web.UI.Page
    {

        #region Member Variables
        // our database class
        private cData m_data;

        // our variables that get set on the cmdline
        private string m_actionref = "";
        private int m_idref = 0;
        private int m_fkey = 0; // where I need to reference a foreign key (eg: new product for client 101)
        
        private string cssTicked = "glyphicon glyphicon-ok";
        private string m_prevclientid = ""; // for client data grid handling
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
            // load our command-line every time..
            m_actionref = cTools.cStrExt(Page.RouteData.Values["actionref"]);
            m_idref = cTools.cIntExt(Page.RouteData.Values["id"]);
            m_fkey = cTools.cIntExt(Page.Request.QueryString["fkey"]);

            // check access ...
            WebUser webUser = cSecurity.getUser();
            if (!webUser.isadmin && !webUser.issuperadmin)
            {
                Response.Clear();
                Response.Write("You do not have access to this module");
                Response.End();
                return;
            }
        }

        private void initApp()
        {
            // run the module now..
            runModule();
        }

        private void updateApp()
        {  
            
        }

        #endregion

        #region Main Page Processing Block
        private void runModule()
        {   
            // do it!
            switch (m_actionref)
            {
                case "clientedit":
                    loadClient(m_idref);
                    break;
                    
                case "locationedit":
                    loadLocation(m_idref);
                    break;

                case "users":
                    loadUserList();
                    break;

                case "useredit":
                    loadUserEditor(m_idref);
                    break;

                case "userlocations":
                    loadUserLocationsList(m_idref);
                    break;

                case "userlocations-add":
                    addUserLocationAccess(m_fkey, m_idref);
                    break;

                case "userlocations-remove":
                    deleteUserLocationAccess(m_fkey, m_idref);
                    break;
            }
        }
        
        #endregion

        #region Client Administration
        private void loadClient(int clientID)
        {
            lblPageTitle.Text = "Client Editor";

            ClientData client;
            if (clientID != 0)
            {
                client = m_data.loadClient(clientID);
            }
            else
            {
                client = new ClientData();
                client.clientid = clientID;
                cmdDeleteClient.Enabled = false;
            }

            // put the details of this document up on the screen for editing..
            txtClientRef.Text = client.clientref;
            txtClientName.Text = client.clientname;
            
            pnlClientEdit.Visible = true;
            phClientEditValidation.Visible = true;
            txtClientRef.Focus();
        }

        private void saveClient()
        {
            ClientData client = new ClientData();

            client.clientid = m_idref;
            client.clientref = txtClientRef.Text;
            client.clientname = txtClientName.Text;

            int newClientID = m_data.saveClient(client);

            // audit trail
            if (m_idref == 0)
                m_data.auditEvent(cConstants.eventClientNew, newClientID);
            else
                m_data.auditEvent(cConstants.eventClientEdit, newClientID);


            if (newClientID != 0)
                Response.Redirect("~/audit/clients");
        }

        private void deleteClient()
        {
            m_data.deleteClient(m_idref);

            //audit trail
            m_data.auditEvent(cConstants.eventClientDelete, m_idref);

            Response.Redirect("~/audit/clients");
        }
        
        protected void cmdSaveClient_Command(object sender, CommandEventArgs e)
        {
            saveClient();
        }

        protected void cmdCancelEditClient_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/audit/clients/");
        }

        protected void cmdDeleteClient_Command(object sender, CommandEventArgs e)
        {
            deleteClient();
        }
        #endregion

        #region Location Administration
        private void loadLocation(int locationId)
        {
            LocationData location;

            if (locationId != 0)
            {
                lblPageTitle.Text = "Edit Site";

                location = m_data.loadLocation(locationId);
            }
            else
            {
                lblPageTitle.Text = "Add New Site";

                location = new LocationData();
                location.clientid = m_fkey;
                cmdDeleteLocation.Enabled = false;
            }

            // load up the client details to show on screen
            ClientData client = m_data.loadClient(location.clientid);
            lblLocationClientName.Text = client.clientname;

            // put the details of this document up on the screen for editing..
            hidLocationClientId.Value = location.clientid.ToString();
            txtLocationBusinessUnit.Text = location.businessunit;
            txtLocationName.Text = location.locationname;
            txtLocationAddress.Text = location.locationaddress;

            pnlLocationEdit.Visible = true;
            txtLocationBusinessUnit.Focus();
            
        }

        private void saveLocation()
        {

            LocationData location = new LocationData();

            location.locationid = m_idref;
            location.clientid = cTools.cIntExt(hidLocationClientId.Value);
            location.businessunit = txtLocationBusinessUnit.Text;
            location.locationname = txtLocationName.Text;
            location.locationaddress = txtLocationAddress.Text;

            int newId = m_data.saveLocation (location);

            //audit trail
            if (m_idref == 0)
                m_data.auditEvent(cConstants.eventLocationNew, newId);
            else
                m_data.auditEvent(cConstants.eventLocationEdit, newId);

            if (newId != 0)
                Response.Redirect("~/audit/clients/");
        }
        private void deleteLocation()
        {
            m_data.deleteLocation(m_idref);

            //audit trail
            m_data.auditEvent(cConstants.eventLocationDelete, m_idref);
            
            Response.Redirect("~/admin/productedit/");
        }
        protected void cmdSaveLocation_Command(object sender, CommandEventArgs e)
        {
            saveLocation();
        }
        protected void cmdCancelLocation_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/audit/clients");
        }
        protected void cmdDeleteLocation_Command(object sender, CommandEventArgs e)
        {
            deleteLocation();
        }
        #endregion
        
        #region User Administration
        private void loadUserList()
        {
            lblPageTitle.Text = "User Administration";

            GridView1.RowDataBound += new GridViewRowEventHandler(handleDataBind_UserListing);
            string sqlCmd = "spAdminUserList";
            m_data.bindToGrid(sqlCmd, GridView1);

            pnlGrid.Visible = true;
            cmdAddNewUser.Visible = true;
        }
        
        private void loadUserEditor(int userId)
        {
            // this is the currently-logged-in user (not the user being edited)
            WebUser webUser = cSecurity.getUser();

            /*
             * get me the user details and load them into a user class 
             */
            WebUser editUser;
            if (userId != 0)
            {
                editUser = m_data.loadUser(userId);
                lblPageTitle.Text = "Edit User - " + editUser.firstname + " " + editUser.lastname;
            }
            else
            {
                // new user
                lblPageTitle.Text = "Add New User";
                editUser = new WebUser();
                editUser.userid = 0;
                
            }
            
            // load up the other edit textboxes
            txtUserFirstname.Text = editUser.firstname;
            txtUserLastname.Text = editUser.lastname;
            txtUserEMail.Text = editUser.emailaddress;

            // network details
            txtUserLoginName.Text = editUser.apploginname;
            txtUserPassword.Text = editUser.apppassword;
            txtNetworkLogin.Text = editUser.networkloginname;

            // user roles
            chkIsNewHire.Checked = editUser.isnewhire;
            chkIsLeader.Checked = editUser.isleader;
            chkIsBuddy.Checked = editUser.isbuddy;
            chkIsAdmin.Checked = editUser.isadmin;
            chkIsSuperAdmin.Checked = editUser.issuperadmin;
            chkIsRN.Checked = editUser.isrn;
            
            // Only a Super-Admin can create admins / super-admins or delete users
            chkIsAdmin.Enabled = webUser.issuperadmin;
            chkIsSuperAdmin.Enabled = webUser.issuperadmin;
            cmdDeleteUser.Enabled = (webUser.issuperadmin && userId != 0);    // can't delete a new entry..

            pnlUserEdit.Visible = true;
            phUserEditValidation.Visible = true;        // enable data validation
            txtUserFirstname.Focus();
        }
        protected void cmdEditUser_Command(object sender, CommandEventArgs e)
        {
            int userId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect("~/admin/useredit/" + userId);
        }
        
        private void saveUser()
        {
            // save changes to the user record..
            WebUser webUser = new WebUser();
            
            webUser.userid = m_idref;
            webUser.firstname = txtUserFirstname.Text;
            webUser.lastname = txtUserLastname.Text;
            webUser.emailaddress = txtUserEMail.Text;
            
            // network..
            webUser.apploginname = txtUserLoginName.Text;
            webUser.apppassword = txtUserPassword.Text;

            // roles ..
            webUser.isnewhire = chkIsNewHire.Checked;
            webUser.isleader= chkIsLeader.Checked;
            webUser.isadmin = chkIsAdmin.Checked;
            webUser.issuperadmin = chkIsSuperAdmin.Checked;
            
            // submit changes to db now..
            int newId = m_data.saveUser(webUser);

            //audit trail
            if (m_idref == 0)
                m_data.auditEvent(cConstants.eventUserNew, newId);
            else
                m_data.auditEvent(cConstants.eventUserEdit, newId);

            if (newId != 0)
                Response.Redirect("~/admin/users");
        }

        private void deleteUser()
        {
            m_data.deleteUser(m_idref);

            //audit trail
            m_data.auditEvent(cConstants.eventUserDelete, m_idref);


            Response.Redirect("~/admin/users");
        }

        private void loadUserLocationsList(int userId)
        {
            WebUser editUser = m_data.loadUser(userId);
            lblPageTitle.Text = "Edit User Site Access - " + editUser.firstname + " " + editUser.lastname;
            
            GridView1.RowDataBound += new GridViewRowEventHandler(handleDataBind_UserLocationsListing);
            string sqlCmd = "spAdminGetUserLocationList " + userId;
            m_data.bindToGrid(sqlCmd, GridView1);

            pnlGrid.Visible = true;
            cmdAddAll.Visible = true;
            cmdRemoveAll.Visible = true;
            cmdCloseEditUserAccess.Visible = true;

        }
        
        private void addUserLocationAccess(int userId, int locationId)
        {
            m_data.addUserLocationAccess(userId, locationId);
            Response.Redirect("~/admin/userlocations/" + userId);
        }

        private void deleteUserLocationAccess(int userId, int locationId)
        {
            m_data.deleteUserLocationAccess(userId, locationId);
            Response.Redirect("~/admin/userlocations/" + userId);
        }
        private void handleDataBind_UserListing(object sender, GridViewRowEventArgs e)
        {
            int colId = 0;
            int colButtonsPlaceholder = 7;

            // hide id's in the grid 
            e.Row.Cells[colId].Visible = false;
            
            // I want the buttons stop going to a second line
            e.Row.Cells[colButtonsPlaceholder].Width = 320;

            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    break;

                case DataControlRowType.DataRow:
            
                    // create buttons
                    e.Row.Cells[colButtonsPlaceholder].Text = String.Format(
                        "<a class='btn btn-primary btn-xs' href='{0}'>Edit User</a>" +
                        "&nbsp;&nbsp;&nbsp;" +
                        "<a class='btn btn-warning btn-xs' href='{1}'>Edit Site Access</a>",
                        Page.ResolveUrl("~/admin/useredit/") + e.Row.Cells[colId].Text,
                        Page.ResolveUrl("~/admin/userlocations/") + e.Row.Cells[colId].Text);
                    break;
            }
        }
        protected void cmdAddNewUser_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/admin/useredit/");
        }

        protected void cmdSaveUser_Command(object sender, CommandEventArgs e)
        {
            saveUser();
        }

        protected void cmdCancelEditUser_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/admin/users/");
        }

        protected void cmdDeleteUser_Command(object sender, CommandEventArgs e)
        {
            deleteUser();
        }

        private void handleDataBind_UserLocationsListing(object sender, GridViewRowEventArgs e)
        {
            int colLocationId = 0;
            int colUsersLocationId = 4;
            int colButtonsPlaceholder = 5;
            int colButtonsPlaceholder2 = 6;

            // hide id's in the grid 
            e.Row.Cells[colLocationId].Visible = false;

            // align the tick boxes
            e.Row.Cells[colUsersLocationId].HorizontalAlign = HorizontalAlign.Center;
            
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    break;

                case DataControlRowType.DataRow:
                    
                    // create buttons
                    e.Row.Cells[colButtonsPlaceholder].Text = String.Format(
                        "<a class='btn btn-primary btn-xs {1}' href='{0}'>Add Site</a>",
                        Page.ResolveUrl("~/admin/userlocations-add/") + e.Row.Cells[colLocationId].Text + "?fkey=" + m_idref,
                        cTools.iif(e.Row.Cells[colUsersLocationId].Text != "0" || e.Row.Cells[colLocationId].Text == "&nbsp;", "hidden"));
                        
                    e.Row.Cells[colButtonsPlaceholder2].Text = String.Format(
                        "<a class='btn btn-danger btn-xs {1}' href='{0}'>Remove</a>",
                        Page.ResolveUrl("~/admin/userlocations-remove/") + e.Row.Cells[colLocationId].Text + "?fkey=" + m_idref,
                        cTools.iif(e.Row.Cells[colUsersLocationId].Text == "0", "hidden"));

                    // show ticks
                    if (e.Row.Cells[colUsersLocationId].Text != "0")
                    {
                        e.Row.Cells[colUsersLocationId].Text = "<span class='" + cssTicked + "'></span>";
                    }
                    else
                    {
                        e.Row.Cells[colUsersLocationId].Text = "";  // don't show the zero when not ticked
                    }
                    break;
            }
        }
        protected void cmdAddAll_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/admin/userlocations-add/-1?fkey=" + m_idref);
        }

        protected void cmdRemoveAll_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/admin/userlocations-remove/-1?fkey=" + m_idref);
        }

        protected void cmdCloseEditUserAccess_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/admin/users");
        }
        #endregion


    }
}