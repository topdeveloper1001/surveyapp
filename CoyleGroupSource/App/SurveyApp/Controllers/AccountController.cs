using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SurveyApp.Models;
namespace SurveyApp.Controllers
{
    public class AccountController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Login(string UserName, string Password) {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Forbidden, "Invalid");
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                return response;

            int userId = cSecurity.checkMobileLogin(UserName, Password);
            if (userId > 0)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, userId.ToString());
            }
            
            return response;
        }
        

    }
}
