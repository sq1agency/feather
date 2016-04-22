using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.PagesService.DTO;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Services.PagesService
{
    internal class PagesWebService : Service
    {
        [AddHeader(ContentType = MimeTypes.Json)]
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

            ICollection<string> pageUrls = new List<string>();
            foreach (PageData pageData in info)
            {
                PageNode pageNode = pageData.NavigationNode;

                CultureInfo[] pageCultures = pageNode.AvailableCultures;

                string defaultPageUrl = UrlPath.ResolveAbsoluteUrl(pageNode.GetFullUrl());
                pageUrls.Add(defaultPageUrl);

                foreach (CultureInfo cultureInfo in pageCultures)
                {
                    string cultureDefinedPageUrl = UrlPath.ResolveAbsoluteUrl(pageNode.GetFullUrl(cultureInfo, fallbackToAnyLanguage: false));
                    pageUrls.Add(cultureDefinedPageUrl);
                }
            }

            return new PagesViewModel() { PageUrls = pageUrls };
        }

        private const string WidgetWrapperTypeName = "Telerik.Sitefinity.Mvc.Proxy.MvcControllerProxy";
    }
}
