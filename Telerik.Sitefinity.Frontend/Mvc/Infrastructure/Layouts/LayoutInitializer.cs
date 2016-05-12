using System;
using System.IO;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Web.Services;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class contains logic for registration and initialization of the layouts.
    /// </summary>
    internal class LayoutInitializer : IInitializer
    {
        /// <summary>
        /// Registers the types and resolvers related to the layouts functionality.
        /// </summary>
        public virtual void Initialize()
        {
            ObjectFactory.Container.RegisterType<ILayoutResolver, LayoutResolver>(new ContainerControlledLifetimeManager());
            ObjectFactory.Container.RegisterType<IVirtualFileResolver, LayoutMvcPageResolver>("PureMvcPageResolver", new ContainerControlledLifetimeManager(), new InjectionConstructor());

            VirtualPathManager.AddVirtualFileResolver<LayoutVirtualFileResolver>(string.Format(System.Globalization.CultureInfo.InvariantCulture, "~/{0}*", LayoutVirtualFileResolver.ResolverPath), typeof(LayoutVirtualFileResolver).FullName);
            ObjectFactory.Container.RegisterType<PageRouteHandler, MvcPageRouteHandler>();
            ObjectFactory.Container.RegisterType<PageEditorRouteHandler, MvcPageEditorRouteHandler>();
            ObjectFactory.Container.RegisterType<TemplateEditorRouteHandler, MvcTemplateEditorRouteHandler>();

            this.mvcVersioningRoute = new System.Web.Routing.Route("Sitefinity/Versioning/{itemId}/{VersionNumber}", ObjectFactory.Resolve<MvcVersioningRouteHandler>());
            System.Web.Routing.RouteTable.Routes.Insert(1, this.mvcVersioningRoute);

            EventHub.Subscribe<IPageTemplateViewModelCreatedEvent>(this.AugmentPageTemplateViewModel);
        }

        /// <summary>
        /// Uninitializes the functionality related to the layouts.
        /// </summary>
        public virtual void Uninitialize()
        {
            EventHub.Unsubscribe<IPageTemplateViewModelCreatedEvent>(this.AugmentPageTemplateViewModel);
            System.Web.Routing.RouteTable.Routes.Remove(this.mvcVersioningRoute);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void AugmentPageTemplateViewModel(IPageTemplateViewModelCreatedEvent ev)
        {
            if (ev != null && ev.ViewModel != null && !string.IsNullOrEmpty(ev.ViewModel.Name))
            {
                var package = (new PackageManager()).GetPackageFromTemplateId(ev.ViewModel.Id.ToString());
                if (!string.IsNullOrEmpty(package))
                {
                    ev.ViewModel.MasterPage = package;
                    if (ev.PageTemplate != null)
                    {
                        var layoutName = new TemplateTitleParser().GetLayoutName(ev.PageTemplate.Name);
                        if (!string.IsNullOrEmpty(layoutName))
                        {
                            var displayPath = string.Format("{0}/MVC/Views/{1}/{2}.cshtml", package, LayoutRenderer.LayoutsFolderName, layoutName);
                            var relativePath = string.Format("~/{0}/{1}", PackageManager.PackagesFolder, displayPath);
                            try
                            {
                                var filePath = System.Web.Hosting.HostingEnvironment.MapPath(relativePath);
                                if (File.Exists(filePath))
                                {
                                    ev.ViewModel.MasterPage = displayPath;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Write(string.Format("Cannot map path: {0}, Exception: {1}", relativePath, ex.ToString()), System.Diagnostics.TraceEventType.Error);
                            }
                        }
                    }
                }
            }
        }

        private System.Web.Routing.Route mvcVersioningRoute;
    }
}
