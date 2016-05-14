using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.TestUI.Framework.Framework.Wrappers.Backend.PageTemplates;
using Telerik.Sitefinity.TestUI.Framework.Utilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.ResourcePackages
{
    /// <summary>
    /// This is test class for Default resource packages tests.
    /// </summary>
    [TestClass]
    public class DefaultResourcePackages : FeatherTestCase
    {
        /// <summary>
        /// UI test MVCWidgetDefaultFeatherDesigner.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void VerifyGeneratedTemplatesDefaultResourcePackages()
        {
            RuntimeSettingsModificator.ExecuteWithClientTimeout(1600000, () => BAT.Macros().NavigateTo().CustomPage("~/sitefinity/design/pagetemplates", false, null, new HtmlFindExpression("class=~sfMain")));
            RuntimeSettingsModificator.ExecuteWithClientTimeout(1600000, () => BAT.Macros().User().EnsureAdminLoggedIn());
            BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().OpenTemplateEditor(BootstrapTemplate);
            Assert.IsTrue(BATFrontend.Wrappers().Backend().PageTemplates().PageTemplateEditor().IsPlaceHolderPresent(PlaceHolderId), "Placeholder not found in template editor");
            BAT.Wrappers().Backend().PageTemplates().PageTemplateModifyScreen().ClickBackToTemplatesButton();
        }

        /// <summary>
        /// Verifies the resource package layouts are displayed in "Based On" column.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void VerifyResourcePackageLayoutsAreDisplayedInBasedOnColumn()
        {
            RuntimeSettingsModificator.ExecuteWithClientTimeout(1600000, () => BAT.Macros().NavigateTo().CustomPage("~/sitefinity/design/pagetemplates", false, null, new HtmlFindExpression("class=~sfMain")));
            RuntimeSettingsModificator.ExecuteWithClientTimeout(1600000, () => BAT.Macros().User().EnsureAdminLoggedIn());
            var text = BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().GetBasedOnTemplateText(BootstrapTemplate);

            Assert.AreEqual("Bootstrap/.../Layouts/Bootstrap.default.cshtml", text, "Based On column does not indicate where which layout the template is based on.");
        }

        private const string BootstrapTemplate = "Bootstrap.default";
        private const string PlaceHolderId = "Contentplaceholder1";
    }
}
