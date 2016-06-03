using Ninject.Modules;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Infrastructure
{
    /// <summary>
    /// Custom Ninject Module
    /// </summary>
    /// <seealso cref="Ninject.Modules.NinjectModule" />
    public class CustomModule : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            this.Bind<IDummyPOCO>().To<DummyPOCO>();
        }
    }
}
