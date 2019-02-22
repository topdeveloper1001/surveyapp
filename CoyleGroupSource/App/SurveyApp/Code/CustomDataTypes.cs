using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

namespace SurveyApp
{
    public class ClientData
    {
        public int clientid { get; set; }
        public string clientref { get; set; }
        public string clientname { get; set; }
        public string clientdatatable { get; set; }
        public string clientdatafolder { get; set; }
        public string clientdatasheetname { get; set; }
        public int clientdatanumfields { get; set; }
    }
    public class LocationData
    {
        public int locationid { get; set; }
        public int clientid { get; set; }
        public string businessunit { get; set; }
        public string locationname { get; set; }
        public string locationaddress { get; set; }
    }

    public class SurveySummaryData
    {
        public int surveyid { get; set; }
        public int templateid { get; set; }
        public int locationid { get; set; }
        public int auditorid { get; set; }
        public string dateofaudit { get; set; }
        public string clientcontact { get; set; }
        public string sitedesc { get; set; }
        public string scopeofwork { get; set; }
        public string weatherconditions { get; set; }
        public int statusid { get; set; }
        public string summary { get; set; }
        public int client_mgruserid { get; set; }
        public int client_adminuserid { get; set; }
        public int client_approvalstatusid { get; set; }
        public string client_approvalcomments { get; set; }
    }

    public class WebFacility
    {
        public int facilityid { get; set; }
        public string facilityname { get; set; }
    }

    public class WebUser
    {
        public int userid { get; set; }
        public int facilityid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string apploginname { get; set; }
        public string apppassword { get; set; }
        public string networkloginname { get; set; }
        public string startdate { get; set; }
        public int leaderuserid { get; set; }
        public int buddyuserid { get; set; }
        public bool isnewhire { get; set; }
        public bool isleader { get; set; }
        public bool isbuddy { get; set; }
        public bool isadmin { get; set; }
        public bool issuperadmin { get; set; }
        public string photo { get; set; }
        public string buddyfile { get; set; }
        public string inactivedate { get; set; }
        public bool isrn { get; set; }
        public string emailaddress { get; set; }
    }

    
   
}
