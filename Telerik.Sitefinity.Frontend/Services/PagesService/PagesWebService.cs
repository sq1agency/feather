using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ServiceStack;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Frontend.Services.PagesService.DTO;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Services.PagesService
{
    /// <summary>
    /// This class provides methods for Sitefinity pages.
    /// </summary>
    /// <seealso cref="ServiceStack.Service" />
    public class PagesWebService : Service
    {
        /// <summary>
        /// Gets all pages.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [AddHeader(ContentType = MimeTypes.Json)]
        public PagesViewModel Get(PagesGetRequest request)
        {
            var pageManager = PageManager.GetManager();

            IEnumerable<PageData> pages = pageManager
                .GetPageDataList()
                .Where(pageData =>
                    pageData.Status == ContentLifecycleStatus.Live &&
                    pageData.Status != ContentLifecycleStatus.Deleted &&
                    pageData.Controls.Where(objectData => objectData.ObjectType == WidgetWrapperTypeName).Any())
                .ToList();

            var pageUrls = new HashSet<string>();

            var controllerFactory = ControllerBuilder.Current.GetControllerFactory() as FrontendControllerFactory;
            if (controllerFactory == null)
            {
                return null;
            }

            foreach (PageData page in pages)
            {
                PageNode pageNode = page.NavigationNode;

                var pageCultures = pageNode.AvailableCultures;

                string defaultPageUrl = UrlPath.ResolveAbsoluteUrl(pageNode.GetFullUrl());
                pageUrls.Add(defaultPageUrl);

                foreach (CultureInfo cultureInfo in pageCultures)
                {
                    string cultureDefinedPageUrl = UrlPath.ResolveAbsoluteUrl(pageNode.GetFullUrl(cultureInfo, fallbackToAnyLanguage: false));
                    pageUrls.Add(cultureDefinedPageUrl);
                }

                foreach (PageControl control in page.Controls)
                {
                    var controlUrl = this.GetPageControlUrl(pageNode.Id, control, controllerFactory);
                    if (!string.IsNullOrEmpty(controlUrl))
                    {
                        pageUrls.Add(controlUrl);
                    }
                }
            }

            return new PagesViewModel() { PageUrls = pageUrls };
        }

        private string GetPageControlUrl(Guid navigationNodeId, PageControl control, FrontendControllerFactory controllerFactory)
        {
            string controllerName = control.Properties.FirstOrDefault(p => p.Name == "ControllerName").Value;
            var controllerInfo = ControllerStore.Controllers().FirstOrDefault(c => c.ControllerType.ToString() == controllerName);
            var controllerType = controllerInfo.ControllerType;

            var controller = controllerFactory.CreateController(HttpContext.Current.Request.RequestContext, controllerType.FullName);

            var modelPropertyInfo = controllerType
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name == "Model");

            if (modelPropertyInfo == null)
            {
                return null;
            }

            var firstItem = this.GetControlItem(modelPropertyInfo, controller);
            if (firstItem == null)
            {
                return null;
            }

            return HyperLinkHelpers.GetDetailPageUrl(firstItem, navigationNodeId);
        }

        private IDataItem GetControlItem(PropertyInfo modelPropertyInfo, object convertedController)
        {
            if (modelPropertyInfo == null)
            {
                return null;
            }

            var model = modelPropertyInfo.GetValue(convertedController, null) as ContentModelBase;
            if (model == null)
            {
                return null;
            }

            var modelContentType = model.ContentType;
            var manager = ManagerBase.GetMappedManager(modelContentType, null);

            var firstItem = manager.GetItems(modelContentType, null, null, 0, 1).OfType<IDataItem>().FirstOrDefault();

            return firstItem as ILocatable;
        }

        private const string WidgetWrapperTypeName = "Telerik.Sitefinity.Mvc.Proxy.MvcControllerProxy";
    }
}
