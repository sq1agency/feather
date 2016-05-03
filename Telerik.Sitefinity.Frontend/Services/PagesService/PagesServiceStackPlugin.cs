using System;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.PagesService.DTO;

namespace Telerik.Sitefinity.Frontend.Services.PagesService
{
    /// <summary>
    /// Represents a ServiceStack plug-in for the pages web service.
    /// </summary>
    /// <seealso cref="ServiceStack.IPlugin" />
    internal class PagesServiceStackPlugin : IPlugin
    {
        /// <summary>
        /// Adding the pages routes
        /// </summary>
        /// <param name="appHost">The service stack appHost.</param>
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
