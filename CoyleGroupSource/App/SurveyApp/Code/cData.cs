using System;
using System.Collections.Generic;
using System.Web;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

using System.Configuration; // so I can access web.config
using Newtonsoft.Json;

namespace SurveyApp
{

    public class cData
    {
        SqlConnection m_conn = null;
        public string lasterrmsg = "";

        #region Set up, Open & Close Database Handlers
        public bool openDB()
        {
            bool dbOpen = false;
            try
            {
                string dataSource = ConfigurationManager.ConnectionStrings["WebAppDB"].ConnectionString;

                if (dataSource == "")
                    return false;

                // open up our connection now..
                m_conn = new SqlConnection(dataSource);
                m_conn.Open();
                dbOpen = true;
            }
            catch (Exception e)
            {
				cTools.log("Error opening database." + e.Message);
                lasterrmsg = e.Message;
            }
            return (dbOpen);

        }

        public void closeDB()
        {
            m_conn.Close();
            m_conn.Dispose();
        }
        #endregion

        #region Generic Sql Run/Exec Commands
        // run a sql command and return a single result
        public string runSql(string sqlCmd)
        {
            string rtnVal;

            SqlCommand cmd;
            cmd = new SqlCommand(sqlCmd, m_conn);
            rtnVal = Convert.ToString(cmd.ExecuteScalar());
            cmd.Dispose();

            return (rtnVal);
        }

        // execute a command on the db
        public int execSql(string sqlCmd)
        {
            int numRecsAffected = 0;
            try
            {
                SqlCommand cmd;
                cmd = new SqlCommand(sqlCmd, m_conn);
                numRecsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                cTools.log("*** ERROR *** Module: execSql [" + sqlCmd + "]\n" + e.Message);
            }

            return (numRecsAffected);
        }

        public SqlDataReader runSqlReader(string sqlCmd)
        {
            SqlDataReader rsReader;
            SqlCommand cmd;

            cmd = new SqlCommand(sqlCmd, m_conn);
            rsReader = cmd.ExecuteReader();
            cmd.Dispose();

            return (rsReader);
        }

        #endregion

        #region Run Sql Commands & Bind to Objects - Dropdowns/Grids etc

        /* 
         * Run a SQL command and bind to a DROPDOWN
         */
        public void createDropdown(string sqlCmd, DropDownList oDDN, string sBlankOptionText = "", string sBlankOptionVal = "")
        {
            SqlDataReader rsReader;
            SqlCommand cmd;

            //clear whatever's already in the list!!
            oDDN.Items.Clear();

            //add "blank" or top-level entry first..
            if (sBlankOptionText != "" || sBlankOptionVal != "")
            {
                oDDN.Items.Add(new ListItem(sBlankOptionText, sBlankOptionVal));
            }

            cmd = new SqlCommand(sqlCmd, m_conn);
            rsReader = cmd.ExecuteReader();
            while (rsReader.Read())
            {
                oDDN.Items.Add(new ListItem(rsReader[0].ToString(), rsReader[1].ToString()));

            }
            rsReader.Close();
            cmd.Dispose();

        }

        /* 
         * Run a SQL command and bind to a GRIDVIEW
         */
        public void bindToGrid(string sqlCmd, GridView oGrid)
        {
            SqlDataReader rsReader;
            SqlCommand cmd;

            cmd = new SqlCommand(sqlCmd, m_conn);
            rsReader = cmd.ExecuteReader();

            oGrid.DataSource = rsReader;
            oGrid.DataBind();

            rsReader.Close();
            cmd.Dispose();

        }

        private IEnumerable<Dictionary<string, object>> Serialize(SqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                        SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }
        public string bindToJsonString(string sqlCmd)
        {
            SqlDataReader rsReader;
            SqlCommand cmd;
            
            cmd = new SqlCommand(sqlCmd, m_conn);
            rsReader = cmd.ExecuteReader();

            var r = Serialize(rsReader);
            string json = JsonConvert.SerializeObject(r, Formatting.Indented);

            rsReader.Close();
            cmd.Dispose();

            return json;
        }
        /* 
         * Run a SQL command and bind to a REPEATER
         */
        public void runSqlRepeater(string sqlCmd, Repeater rptTarget)
        {
            SqlDataAdapter cmd;
            DataSet ds = new DataSet();

            // get my data..
            cmd = new SqlDataAdapter(sqlCmd, m_conn);
            cmd.Fill(ds);

            //bind the repeater to the dataset
            rptTarget.DataSource = ds;
            rptTarget.DataBind();

            cmd.Dispose();
        }

        /* 
        * Run a SQL command and bind to a LIST
        */
        public void runSqlList(string sqlCmd, List<ListItem> lst)
        {
            SqlDataReader rsReader;
            SqlCommand cmd;

            cmd = new SqlCommand(sqlCmd, m_conn);
            rsReader = cmd.ExecuteReader();
            while (rsReader.Read())
            {
                lst.Add(new ListItem(rsReader[0].ToString(), rsReader[1].ToString()));

            }
            rsReader.Close();
            cmd.Dispose();
        }

        #endregion

        #region User Management
        public WebUser loadUser(int userID)
        {
            SqlDataReader rsReader;
            SqlCommand cmd;

            // create our blank class
            WebUser webUser = new WebUser();

            if (userID != 0)
            {
                // our sql command to get the document from the db
                string sqlCmd = "spGetUser";

                // go get me the iser and put the details into the empty class
                cmd = new SqlCommand(sqlCmd, m_conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("userid", userID);

                rsReader = cmd.ExecuteReader();
                if (rsReader.Read())
                {
                    // the db stuff we need for the CMS
                    webUser.userid = cIntExt(rsReader["userid"]);
                    webUser.facilityid = cIntExt(rsReader["facilityid"]);
                    webUser.firstname = cStrExt(rsReader["firstname"]);
                    webUser.lastname = cStrExt(rsReader["lastname"]);
                    webUser.apploginname = cStrExt(rsReader["apploginname"]);
                    webUser.apppassword = cStrExt(rsReader["apppassword"]);
                    webUser.networkloginname = cStrExt(rsReader["networkloginname"]);
                    webUser.startdate = cStrExt(rsReader["startdate"]);
                    webUser.isnewhire = cBoolExt(rsReader["isnewhire"]);
                    webUser.isleader = cBoolExt(rsReader["isleader"]);
                    webUser.isbuddy = cBoolExt(rsReader["isbuddy"]);
                    webUser.isadmin = cBoolExt(rsReader["isadmin"]);
                    webUser.issuperadmin = cBoolExt(rsReader["issuperadmin"]);
                    webUser.photo = cStrExt(rsReader["photo"]);
                    webUser.buddyfile = cStrExt(rsReader["buddyfile"]);
                    webUser.leaderuserid = cIntExt(rsReader["leaderuserid"]);
                    webUser.buddyuserid = cIntExt(rsReader["buddyuserid"]);

                    webUser.inactivedate = cStrExt(rsReader["inactivedate"]);
                    webUser.emailaddress = cStrExt(rsReader["emailaddress"]);
                    webUser.isrn = cBoolExt(rsReader["isrn"]);

                }
                rsReader.Close();
                cmd.Dispose();
            }

            return (webUser);
        }


        /*
         * Save the user to the database
         */
        public int saveUser(WebUser webUser)
        {
            // add the new user / save user changes..
            SqlCommand cmd;
            string sqlCmd = "spAdminSaveUser";

            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("userid", webUser.userid);
            cmd.Parameters.AddWithValue("firstname", webUser.firstname);
            cmd.Parameters.AddWithValue("lastname", webUser.lastname);
            cmd.Parameters.AddWithValue("emailaddress", webUser.emailaddress);

            // network 
            cmd.Parameters.AddWithValue("apploginname", webUser.apploginname);
            cmd.Parameters.AddWithValue("apppassword", webUser.apppassword);
            
            //roles..
            cmd.Parameters.AddWithValue("isnewhire", webUser.isnewhire);
            cmd.Parameters.AddWithValue("isleader", webUser.isleader);
            cmd.Parameters.AddWithValue("isadmin", webUser.isadmin);
            cmd.Parameters.AddWithValue("issuperadmin", webUser.issuperadmin);
            
            int newUserID = Convert.ToInt32(cmd.ExecuteScalar());

            return (newUserID);   //return val from SP
        }

        // mark a user as deleted
        public void deleteUser(int userID)
        {
            string sqlCmd = "UPDATE users SET deleted=1 WHERE userid=" + userID.ToString();
            execSql(sqlCmd);
        }


        public int addUserLocationAccess(int userId, int locationId)
        {
            SqlCommand cmd;
            string sqlCmd = "spAdminSaveUserLocation";

            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("userid", userId);
            cmd.Parameters.AddWithValue("locationid", locationId);

            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            return (newId);   //return val from SP
        }

        public void deleteUserLocationAccess(int userId, int locationId)
        {
            string sqlCmd = "DELETE FROM users_locations WHERE userid=" + userId.ToString();
            if (locationId != -1)
                sqlCmd += " AND locationid=" + locationId.ToString();

            execSql(sqlCmd);
        }

        #endregion

        #region Client Management

        /*
        * Load from the database into a specified class
        */
        public ClientData loadClient(int clientID)
        {
            SqlDataReader rsReader;
            SqlCommand cmd;
            ClientData client;

            // create our blank class
            client = new ClientData();

            // our sql command to get the document from the db
            string sqlCmd = "spGetClient " + clientID.ToString();

            // go get me the doc and put the details into the empty class
            cmd = new SqlCommand(sqlCmd, m_conn);

            rsReader = cmd.ExecuteReader();
            if (rsReader.Read())
            {
                client.clientid = cTools.cIntExt(rsReader["clientid"]);
                client.clientref = cTools.cStrExt(rsReader["clientref"]);
                client.clientname = cTools.cStrExt(rsReader["clientname"]);
                client.clientdatatable = cTools.cStrExt(rsReader["clientdatatable"]);
                client.clientdatafolder = cTools.cStrExt(rsReader["clientdatafolder"]);
                client.clientdatasheetname = cTools.cStrExt(rsReader["clientdatasheetname"]);
                client.clientdatanumfields = cTools.cIntExt(rsReader["clientdatanumfields"]);

            }
            rsReader.Close();
            cmd.Dispose();
            return (client);
        }

        /*
        * Save the specfied data to the database
        */
        public int saveClient(ClientData client)
        {
            SqlCommand cmd;
            int newClientID;
            string sqlCmd = "spAdminSaveClient";

            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("clientid", client.clientid);
            cmd.Parameters.AddWithValue("clientref", client.clientref);
            cmd.Parameters.AddWithValue("clientname", client.clientname);

            newClientID = Convert.ToInt16(cmd.ExecuteScalar());
            return (newClientID);   //return val from SP
        }

        public void deleteClient(int clientID)
        {
            string sqlCmd = "UPDATE clients SET deleted=1 WHERE clientid=" + clientID.ToString();
            execSql(sqlCmd);
        }
        #endregion

        #region Location Management


        /*
       * Load from the database into a specified class
       */
        public LocationData loadLocation(int locationId)
        {
            SqlDataReader rsReader;
            SqlCommand cmd;
            LocationData location = new LocationData();

            // our sql command to get the document from the db
            string sqlCmd = "spAdminGetLocation " + locationId.ToString();

            // go get me the doc and put the details into the empty class
            cmd = new SqlCommand(sqlCmd, m_conn);

            rsReader = cmd.ExecuteReader();
            if (rsReader.Read())
            {
                location.locationid = cTools.cIntExt(rsReader["locationid"]);
                location.clientid = cTools.cIntExt(rsReader["clientid"]);
                location.businessunit = cTools.cStrExt(rsReader["businessunit"]);
                location.locationname = cTools.cStrExt(rsReader["locationname"]);
                location.locationaddress = cTools.cStrExt(rsReader["locationaddress"]);

            }
            rsReader.Close();
            cmd.Dispose();
            return (location);
        }

        /*
        * Save the specfied data to the database
        */
        public int saveLocation(LocationData location)
        {
            SqlCommand cmd;
            string sqlCmd = "spAdminSaveLocation";

            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("locationid", location.locationid);
            cmd.Parameters.AddWithValue("clientid", location.clientid);
            cmd.Parameters.AddWithValue("businessunit", location.businessunit);
            cmd.Parameters.AddWithValue("locationname", location.locationname);
            cmd.Parameters.AddWithValue("locationaddress", location.locationaddress);

            int newId = Convert.ToInt16(cmd.ExecuteScalar());
            return (newId);   //return val from SP
        }
        
        public void deleteLocation(int locationId)
        {
            string sqlCmd = "UPDATE locations SET deleted=1 WHERE locationid=" + locationId.ToString();
            execSql(sqlCmd);
        }
        #endregion
        
        #region Survey Management
        public SurveySummaryData loadSurvey(int surveyId)
        {
            SqlDataReader rsReader;
            SqlCommand cmd;
            SurveySummaryData survey = new SurveySummaryData();

            // our sql command to get the document from the db
            string sqlCmd = "spGetSurvey " + surveyId.ToString();

            // go get me the doc and put the details into the empty class
            cmd = new SqlCommand(sqlCmd, m_conn);

            rsReader = cmd.ExecuteReader();
            if (rsReader.Read())
            {
                survey.surveyid = cTools.cIntExt(rsReader["surveyid"]);
                survey.templateid = cTools.cIntExt(rsReader["templateid"]);
                survey.locationid = cTools.cIntExt(rsReader["locationid"]);
                survey.auditorid = cTools.cIntExt(rsReader["auditorid"]);
                survey.dateofaudit = cTools.cStrExt(rsReader["dateofaudit"]);
                survey.clientcontact = cTools.cStrExt(rsReader["clientcontact"]);
                survey.sitedesc = cTools.cStrExt(rsReader["sitedesc"]);
                survey.scopeofwork = cTools.cStrExt(rsReader["scopeofwork"]);
                survey.weatherconditions = cTools.cStrExt(rsReader["weatherconditions"]);
                survey.statusid = cTools.cIntExt(rsReader["statusid"]);
                survey.summary = cTools.cStrExt(rsReader["summary"]);
                survey.client_mgruserid = cTools.cIntExt(rsReader["client_mgruserid"]);
                survey.client_adminuserid= cTools.cIntExt(rsReader["client_adminuserid"]);
                survey.client_approvalstatusid = cTools.cIntExt(rsReader["client_approvalstatusid"]);
                survey.client_approvalcomments = cTools.cStrExt(rsReader["client_approvalcomments"]);
            }
            rsReader.Close();
            cmd.Dispose();
            return (survey);
        }
        public int saveSurvey(SurveySummaryData survey)
        {
            SqlCommand cmd;
            string sqlCmd = "spSaveSurvey";

            int sourceUserId = cSecurity.userID();

            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("surveyid", survey.surveyid);
            cmd.Parameters.AddWithValue("userid", sourceUserId);
            cmd.Parameters.AddWithValue("templateid", survey.templateid);
            cmd.Parameters.AddWithValue("locationid", survey.locationid);
            cmd.Parameters.AddWithValue("auditorid", survey.auditorid);
            cmd.Parameters.AddWithValue("dateofaudit", survey.dateofaudit);
            cmd.Parameters.AddWithValue("clientcontact", survey.clientcontact);
            cmd.Parameters.AddWithValue("sitedesc", survey.sitedesc);
            cmd.Parameters.AddWithValue("scopeofwork", survey.scopeofwork);
            cmd.Parameters.AddWithValue("weatherconditions", survey.weatherconditions);
            cmd.Parameters.AddWithValue("statusid", survey.statusid);
            cmd.Parameters.AddWithValue("summary", survey.summary);
            cmd.Parameters.AddWithValue("client_mgruserid", survey.client_mgruserid);
            cmd.Parameters.AddWithValue("client_adminuserid", survey.client_adminuserid);

            int newId = Convert.ToInt16(cmd.ExecuteScalar());
            return (newId);   //return val from SP
        }
        public void deleteSurvey(int surveyId)
        {
            string sqlCmd = "UPDATE surveys SET deleted=1 WHERE surveyid=" + surveyId.ToString();
            execSql(sqlCmd);
        }
        
        public int saveSurveyAnswer(int surveyId, int userid, int questionid, int answerid, int classificationId = 0, string observationsText = "")
        {
            int newId = 0;

            try
            {
             
                SqlCommand cmd;
                string sqlCmd = "spSaveSurveyAnswer";

                cmd = new SqlCommand(sqlCmd, m_conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("surveyid", surveyId);
                cmd.Parameters.AddWithValue("userid", userid);
                cmd.Parameters.AddWithValue("questionid", questionid);

                // note - we want to be able to change these individually without affecting the other
                //   for example, we could be changing from a yes to a no but not updating the notes field..
                if (answerid != 0)              // I want to save Nulls in the answerid if it's zero - keep it consistent
                    cmd.Parameters.AddWithValue("answerid", answerid);
                if (classificationId != 0)              // I want to save Nulls in the answerid if it's zero - keep it consistent
                    cmd.Parameters.AddWithValue("classificationid", classificationId);
                if (observationsText != "null")       // I want to save Nulls in the observationsText is "null"
                    cmd.Parameters.AddWithValue("observations", observationsText);
                
                // save now...
                newId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                cTools.log("***ERROR*** Module cData.saveSurveyAnswer encountered error:" + e.Message);
            }

            return (newId);   //return val from SP
        }

        public string getQuestion(int questionid)
        {
            string sqlCmd = "SELECT " +
                "    qc.questioncatname + ': ' + q.questiontext " +
                " FROM questions AS q " +
                " LEFT JOIN questioncategories AS qc ON qc.questioncatid = q.questioncatid " +
                " WHERE q.questionid=" + questionid;
            return (runSql(sqlCmd));
        }

        public string getAnswer(int answerId)
        {
            string sqlCmd = "SELECT " +
                "     a.answertext " +
                " FROM answers AS a " +
                " WHERE a.answerid=" + answerId;
            return (runSql(sqlCmd));
        }
        public string getClassification(int classificationId)
        {
            string sqlCmd = "SELECT " +
                "     classificationtitle " +
                " FROM classifications AS c " +
                " WHERE c.classificationid=" + classificationId;
            return (runSql(sqlCmd));
        }

        public int getQuestionIdFromFileId(int fileId)
        {
            string sqlCmd = "SELECT questionid FROM fileuploads WHERE fileid=" + fileId; 
            return (cTools.cIntExt(runSql(sqlCmd)));
        }

        public int saveSurveyClientApproval(SurveySummaryData survey)
        {
            SqlCommand cmd;
            string sqlCmd = "spSaveSurveyClientApproval";

            int sourceUserId = cSecurity.userID();

            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("surveyid", survey.surveyid);
            cmd.Parameters.AddWithValue("client_approvalstatusid", survey.client_approvalstatusid);
            cmd.Parameters.AddWithValue("client_approvalcomments", survey.client_approvalcomments);
            cmd.Parameters.AddWithValue("client_approveruserid", sourceUserId);
            
            int newId = Convert.ToInt16(cmd.ExecuteScalar());
            return (newId);   //return val from SP
        }
        public int saveClientResponse(int surveyId, int userid, int questionid, string clientResponse, string targetDate, string closeItem)
        {
            int newId = 0;

            try
            {

                SqlCommand cmd;
                string sqlCmd = "spSaveSurveyClientResponse";

                cmd = new SqlCommand(sqlCmd, m_conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("surveyid", surveyId);
                cmd.Parameters.AddWithValue("userid", userid);
                cmd.Parameters.AddWithValue("questionid", questionid);
                if (clientResponse != "null")       // I want to save Nulls in the observationsText is "null"
                    cmd.Parameters.AddWithValue("clientresponse", clientResponse);
                if (targetDate != "null")       // I want to save Nulls in the observationsText is "null"
                    cmd.Parameters.AddWithValue("targetdate", targetDate);
                if (closeItem != "null")       // I want to save Nulls in the observationsText is "null"
                    cmd.Parameters.AddWithValue("closeitem", closeItem);

                // save now...
                newId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                cTools.log("***ERROR*** Module cData.saveClientResponse encountered error:" + e.Message);
            }

            return (newId);   //return val from SP
        }
       
        public int getNumQuestionsUnanswered(int surveyId)
        {
            SqlCommand cmd;
            string sqlCmd = "spGetSurveyDetailNumUnanswered";

            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("surveyid", surveyId);
            
            int numQuestionsUnanswered  = Convert.ToInt16(cmd.ExecuteScalar());
            return (numQuestionsUnanswered);   //return val from SP
        }
        public int saveFileUpload(int fileId, int surveyId, int questionId, int sourceId, string originalFilename, string uniqueFilename)
        {
            SqlCommand cmd;
            string sqlCmd = "spSaveFileUpload";

            int sourceUserId = cSecurity.userID();  // who's logged in

            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("fileid", fileId);
            cmd.Parameters.AddWithValue("surveyid", surveyId);
            cmd.Parameters.AddWithValue("questionid", questionId);
            cmd.Parameters.AddWithValue("sourceid", sourceId);
            cmd.Parameters.AddWithValue("userid", sourceUserId);
            cmd.Parameters.AddWithValue("sourcefilename", originalFilename);
            cmd.Parameters.AddWithValue("serverfilename", uniqueFilename);

            int newId = Convert.ToInt16(cmd.ExecuteScalar());
            return (newId);   //return val from SP
        }

        public void deleteFile(int fileId)
        {
            string sqlCmd = "UPDATE fileuploads SET deleted=1 WHERE fileid=" + fileId;
            execSql(sqlCmd);
        }
        #endregion

        #region Audit Trail
        public void auditEvent(int eventId, int sourceId = 0, string narrativeText = "")
        {
            // see if this event is being logged by the user or a leader/admin
            int userId = cSecurity.userID();
            
            SqlCommand cmd;
            string sqlCmd = "spSaveAuditEvent";

            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("userid", userId);
            cmd.Parameters.AddWithValue("eventtypeid", eventId);
            if (sourceId != 0)
                cmd.Parameters.AddWithValue("sourceid", sourceId);
            if (narrativeText != "")
                cmd.Parameters.AddWithValue("narrative", narrativeText);
            
            cmd.ExecuteNonQuery();
        }
        #endregion

        #region User Login
        public int getUserIDFromLogin(string loginName, string loginPassword, string applicationOrNetworkLogin)
        {
            SqlDataReader rsReader;
            SqlCommand cmd;
            int userID = 0;

            // determine if we're checking the application or the network login
            int isApplicationLogin = 1;
            if (applicationOrNetworkLogin == "network")
                isApplicationLogin = 0;

            // our sql command to get the userid from the db
            string sqlCmd = "spCheckUserLogin";

            // go get me the doc and put the details into the empty class
            cmd = new SqlCommand(sqlCmd, m_conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("apploginname", loginName);
            cmd.Parameters.AddWithValue("apppassword", loginPassword);
            cmd.Parameters.AddWithValue("isapplicationlogin", isApplicationLogin);

            rsReader = cmd.ExecuteReader();
            if (rsReader.Read())
                userID = cIntExt(rsReader["userid"]);
            rsReader.Close();
            cmd.Dispose();

            return (userID);
        }

        #endregion

        #region Handy Tools

        // if something is null, just set it to ""
        private string cStrExt(object obj)
        {
            string rtnVal = "";

            if (obj != null)
            {
                rtnVal = obj.ToString();
            }

            return (rtnVal);
        }

        private int cIntExt(object obj)
        {
            int rtnVal = 0;

            if (!DBNull.Value.Equals(obj))
            {
                rtnVal = Convert.ToInt32(obj);
            }

            return (rtnVal);
        }

        private bool cBoolExt(object obj)
        {
            bool rtnVal = false;

            if (!DBNull.Value.Equals(obj))
            {
                rtnVal = Convert.ToBoolean(obj);
            }

            return (rtnVal);
        }
        #endregion
        
    }
}

