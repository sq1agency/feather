using Ninject.Modules;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Infrastructure
{
    public class CustomModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ITestType>().To<TestType>();
        }
    }
}
