using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Xml;
using System.Data.SqlClient;

using System.Configuration; // so I can access web.config

namespace ArdentPortalWebApp
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreLoad()
        {
            Response.Redirect("~/home");
        }

    }
}