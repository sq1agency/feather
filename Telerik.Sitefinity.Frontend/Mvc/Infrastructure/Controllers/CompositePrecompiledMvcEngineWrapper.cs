using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    public class CompositePrecompiledMvcEngineWrapper : RazorGenerator.Mvc.CompositePrecompiledMvcEngine
    {
        private readonly RazorGenerator.Mvc.PrecompiledViewAssembly[] precompiledAssemblies;

        public string Package { get; set; }

        public CompositePrecompiledMvcEngineWrapper(params RazorGenerator.Mvc.PrecompiledViewAssembly[] viewAssemblies)
            : this(viewAssemblies, null)
        {
        }

        public CompositePrecompiledMvcEngineWrapper(IEnumerable<RazorGenerator.Mvc.PrecompiledViewAssembly> viewAssemblies, System.Web.Mvc.IViewPageActivator viewPageActivator)
            : base(viewAssemblies, viewPageActivator)
        {
            this.precompiledAssemblies = viewAssemblies.ToArray();
        }

        public CompositePrecompiledMvcEngineWrapper Clone()
        {
            var clone = new CompositePrecompiledMvcEngineWrapper(this.precompiledAssemblies);
            clone.Package = this.Package;

            return clone;
        }

        protected override bool FileExists(System.Web.Mvc.ControllerContext controllerContext, string virtualPath)
        {
            var result = base.FileExists(controllerContext, virtualPath);
            return result;
            //return Telerik.Sitefinity.Abstractions.VirtualPath.VirtualPathManager.FileExists(virtualPath) &&
            //    Telerik.Sitefinity.Abstractions.VirtualPath.VirtualPathManager.GetCacheDependency(virtualPath, new string[0], DateTime.UtcNow) == null &&
            //    base.FileExists(controllerContext, virtualPath);
        }
    }
}
