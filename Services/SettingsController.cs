using Dnn.Modules.PowerBI.Components;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Net;
using System.Net.Http;

namespace Dnn.Modules.PowerBI.Services
{
    [SupportedModules("PowerBI")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class SettingsController : DnnApiController
    {

        public HttpResponseMessage GetSettings()
        {
            var moduleSettings = new BIModuleSettingsRepository().GetSettings(this.ActiveModule);
            return Request.CreateResponse(HttpStatusCode.OK, moduleSettings);
        }

        public HttpResponseMessage PostSettings(BIModuleSettings settings)
        {
            var repo = new BIModuleSettingsRepository();
            repo.SaveSettings(this.ActiveModule, settings);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}