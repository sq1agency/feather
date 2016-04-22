using System;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.PagesService.DTO;

namespace Telerik.Sitefinity.Frontend.Services.PagesService
{
    internal class PagesServiceStackPlugin : IPlugin
    {
        public void Register(IAppHost appHost)
        {
            if (appHost == null)
                throw new ArgumentNullException("appHost");

            appHost.RegisterService<PagesWebService>();
            appHost.Routes.Add<PagesGetRequest>(PagesServiceStackPlugin.PagesServiceUrl, ApplyTo.Get);
        }

        private const string PagesServiceUrl = "/pages-api"; 
    }
}
