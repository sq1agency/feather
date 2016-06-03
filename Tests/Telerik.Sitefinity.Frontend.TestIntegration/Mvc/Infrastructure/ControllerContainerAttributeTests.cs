using System;
using System.Linq;
using System.Reflection;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.TestIntegration.Helpers;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Infrastructure
{
    /// <summary>
    /// This class contains tests for ControllerContainerAttribute and ResourcePackageAttribute.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This class contains tests for ControllerContainerAttribute and ResourcePackageAttribute.")]
    public class ControllerContainerAttributeTests
    {
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether assembly is added only once to ControllerContainerInitializer.ControllerContainerAssemblies if it has both ControllerContainerAttribute and ResourcePackageAttribute set")]
        public void ControllerContainerAssemblies_CheckAssemblyAddedOnlyOnce()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Assert.IsTrue(assembly.CustomAttributes.Any(x => x.AttributeType == typeof(ControllerContainerAttribute)));
            Assert.IsTrue(assembly.CustomAttributes.Any(x => x.AttributeType == typeof(ResourcePackageAttribute)));

            var controllerContainerInitializer = new ControllerContainerInitializer();
            Assert.IsTrue(controllerContainerInitializer.ControllerContainerAssemblies.Count(x => x.FullName == assembly.FullName) == 1);

            Assert.DoesNotThrow(() => WebRequestHelper.GetPageWebContent(WebRequestHelper.GetSiteLocation()), "Sitefinity did not start properly");
        }
    }
}
