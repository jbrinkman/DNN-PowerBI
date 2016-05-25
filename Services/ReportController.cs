using System;
using System.Linq;
using System.Net.Http;
using Dnn.Modules.PowerBI.Components;
using Dnn.Modules.PowerBI.Services.ViewModels;
using DotNetNuke.Web.Api;
using DotNetNuke.Security;
using Newtonsoft.Json;

namespace Dnn.Modules.PowerBI.Services
{
    [SupportedModules("PowerBI")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class ReportController : DnnApiController
    {

        const string workspaceName = "DNN-Connect";
        const string workspaceId = "68c5a8c6-f867-4233-80b4-ab1d9422947b";
        const string accessKey = "CuWhTGCOSauHI3He3MIc9mSv+nSG8ju+Gc2Cw5ul71VTHlBVsp2zMevm11pizYpIIT0rjstjIwSGp2wdC/APnw==";

        public HttpResponseMessage GetList()
        {
            Console.Write(Settings.WorkspaceName);

            if (string.IsNullOrEmpty(Settings.WorkspaceName) ||
                string.IsNullOrEmpty(Settings.WorkspaceId) ||
                string.IsNullOrEmpty(Settings.AccessKey))
            {
                return Request.CreateResponse(string.Empty);
            }

            PBIReports reports;

            //Get an app token to generate a JSON Web Token (JWT). An app token flow is a claims-based design pattern.
            //To learn how you can code an app token flow to generate a JWT, see the PowerBIToken class.
            var appToken = PowerBIToken.CreateDevToken(Settings.WorkspaceName, Settings.WorkspaceId);
            //var appToken = PowerBIToken.CreateDevToken(workspaceName, workspaceId);

            //After you get a PowerBIToken which has Claims including your WorkspaceName and WorkspaceID,
            //you generate JSON Web Token (JWT) . The Generate() method uses classes from System.IdentityModel.Tokens: SigningCredentials, 
            //JwtSecurityToken, and JwtSecurityTokenHandler. 
            string jwt = appToken.Generate(Settings.AccessKey);
            //string jwt = appToken.Generate(accessKey);

            //Construct reports uri resource string
            var uri = String.Format("https://api.powerbi.com/beta/collections/{0}/workspaces/{1}/reports", Settings.WorkspaceName, Settings.WorkspaceId);
            //var uri = String.Format("https://api.powerbi.com/beta/collections/{0}/workspaces/{1}/reports", workspaceName, workspaceId);

            //Configure reports request
            System.Net.WebRequest request = System.Net.WebRequest.Create(uri) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;

            //Set the WebRequest header to AppToken, and jwt
            //Note the use of AppToken instead of Bearer
            request.Headers.Add("Authorization", String.Format("AppToken {0}", jwt));

            //Get reports response from request.GetResponse()
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get reader from response stream
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();
                    //Deserialize JSON string into PBIReports
                    reports = JsonConvert.DeserializeObject<PBIReports>(json);

                }
            }

            var reportList = from rpt in reports.value
                             select new
                             {
                                 id = rpt.id,
                                 name = rpt.name,
                                 embedUrl = rpt.embedUrl,
                                 token = getReportToken(rpt.id)
                             };

            return Request.CreateResponse(reportList);
        }

        private string getReportToken(string reportId)
        {
            //Get an embed token for a report. 
            var token = PowerBIToken.CreateReportEmbedToken(Settings.WorkspaceName, Settings.WorkspaceId, reportId);
            //var token = PowerBIToken.CreateReportEmbedToken(workspaceName, workspaceId, reportId);

            //After you get a PowerBIToken which has Claims including your WorkspaceName and WorkspaceID,
            //you generate JSON Web Token (JWT) . The Generate() method uses classes from System.IdentityModel.Tokens: SigningCredentials, 
            //JwtSecurityToken, and JwtSecurityTokenHandler. 
            string jwt = token.Generate(Settings.AccessKey);
            //string jwt = token.Generate(accessKey);

            //The JavaScript embed code uses the embed jwt for a report. 
            return jwt;
        }

        private BIModuleSettings _settings;
        private BIModuleSettings Settings
        {
            get
            {
                return _settings ?? (_settings = new BIModuleSettingsRepository().GetSettings(this.ActiveModule));
            }
        }
    }
}
