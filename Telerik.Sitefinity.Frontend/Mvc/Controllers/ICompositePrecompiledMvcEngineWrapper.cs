using System.Collections.Generic;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    public interface ICompositePrecompiledMvcEngineWrapper
    {
        string PackageName { get; }

        IEnumerable<PrecompiledViewAssemblyWrapper> PrecompiledAssemblies { get; }

        ICompositePrecompiledMvcEngineWrapper Clone();
    }
}
