using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TripRadar.Controllers;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v2;
using Google.Apis.Util.Store;
using System.Web.Mvc;

namespace TripRadar.Class
{
    public class AppFlowMetadata : FlowMetadata
    {

        private static readonly IAuthorizationCodeFlow flow =
           new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
           {
               ClientSecrets = new ClientSecrets
               {
                   ClientId = "866525550916-o4k3tn8uk6t835sav6iud8pjogo2ptv0.apps.googleusercontent.com",
                   ClientSecret = "yY9iaAfrHJtme5Lg7fM82mOB"
               },
               Scopes = new[] { DriveService.Scope.Drive },
               DataStore = new FileDataStore("Drive.Api.Auth.Store")
           });

        public override string GetUserId(Controller controller)
        {
            
            var user = controller.Session["user"];
            if (user == null)
            {
                user = Guid.NewGuid();
                controller.Session["user"] = user;
            }
            return user.ToString();

        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}
