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
    public class PagesWebService : Service
    {
        public PagesViewModel GetUrls()
        {
            var pageManager = PageManager.GetManager();

            IEnumerable<PageData> info = pageManager
                .GetPageDataList()
                .Where(pageData =>
                    pageData.Status == ContentLifecycleStatus.Live &&
                    pageData.Status != ContentLifecycleStatus.Deleted &&
                    pageData.Controls.Where(objectData => objectData.ObjectType == WidgetWrapperTypeName).Any())
                .ToList();

            var pageUrls = new SortedSet<string>();
            foreach (PageData pageData in info)
            {
                PageNode pageNode = pageData.NavigationNode;

                var pageCultures = pageNode.AvailableCultures;

                string defaultPageUrl = UrlPath.ResolveAbsoluteUrl(pageNode.GetFullUrl());
                pageUrls.Add(defaultPageUrl);

                foreach (CultureInfo cultureInfo in pageCultures)
                {
                    string cultureDefinedPageUrl = UrlPath.ResolveAbsoluteUrl(pageNode.GetFullUrl(cultureInfo, fallbackToAnyLanguage: false));
                    pageUrls.Add(cultureDefinedPageUrl);
                }

                var controllerFactory = ControllerBuilder.Current.GetControllerFactory() as FrontendControllerFactory;
                if (controllerFactory == null)
                {
                    continue;
                }

                foreach (PageControl control in pageData.Controls)
                {
                    var controlUrl = this.GetPageControlUrls(pageNode.Id, control, controllerFactory);
                    pageUrls.Add(controlUrl);
                }
            }

            return new PagesViewModel() { PageUrls = pageUrls };
        }

        private string GetPageControlUrls(Guid navigationNodeId, PageControl control, FrontendControllerFactory controllerFactory)
        {
            string controllerName = control.Properties.FirstOrDefault(p => p.Name == "ControllerName").Value;
            var controllerInfo = ControllerStore.Controllers().FirstOrDefault(c => c.ControllerType.ToString() == controllerName);
            var controllerType = controllerInfo.ControllerType;

            dynamic convertedController = this.GetConvertedController(controllerType, controllerFactory);

            var modelPropertyInfo = controllerType
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name == "Model");

            if (modelPropertyInfo == null)
            {
                return null;
            }

            var firstItem = this.GetControlItem(modelPropertyInfo, convertedController);
            if (!(firstItem is ILocatable))
            {
                return null;
            }

            return HyperLinkHelpers.GetDetailPageUrl(firstItem, navigationNodeId);
        }

        private object GetConvertedController(Type controllerType, FrontendControllerFactory controllerFactory)
        {
            var controller = controllerFactory.CreateController(HttpContext.Current.Request.RequestContext, controllerType.FullName) as Controller;
            return Convert.ChangeType(controller, controllerType);
        }

        private IDataItem GetControlItem(PropertyInfo modelPropertyInfo, dynamic convertedController)
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
