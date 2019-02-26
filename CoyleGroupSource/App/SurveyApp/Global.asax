<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        RegisterRoutes(RouteTable.Routes);

    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

    void RegisterRoutes(RouteCollection routes)
    {
        // User Login
        routes.MapPageRoute("UserLogin",
             "login", "~/UserLogin.aspx");

        routes.MapPageRoute("Home",
            "home", "~/Home.aspx");

        // we define two routes for our admin page..one that takes an ID and the other that does not..
        routes.MapPageRoute("AdminAction",
            "admin/{actionref}", "~/AdminTool.aspx");

        routes.MapPageRoute("AdminActionID",
            "admin/{actionref}/{id}", "~/AdminTool.aspx");

        // we define two routes for our survey page..one that takes an ID and the other that does not..
        routes.MapPageRoute("SurveyAction",
            "audit/{actionref}", "~/Survey.aspx");

        routes.MapPageRoute("SurveyActionID",
            "audit/{actionref}/{id}", "~/Survey.aspx");
        
        // we define two routes for our page..one that takes an ID and the other that does not..
        routes.MapPageRoute("Reports",
            "reports", "~/Reports.aspx");

         routes.MapPageRoute("ReportsAction",
            "reports/{reportref}", "~/Reports.aspx");
    }

</script>
