using System.Collections.Generic;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// Exposes properties and methods for precompiled view engines
    /// </summary>
    public interface ICompositePrecompiledMvcEngineWrapper
    {
        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        /// <value>
        /// The name of the package.
        /// </value>
        string PackageName { get; }

        /// <summary>
        /// Gets the precompiled assemblies.
        /// </summary>
        /// <value>
        /// The precompiled assemblies.
        /// </value>
        IEnumerable<PrecompiledViewAssemblyWrapper> PrecompiledAssemblies { get; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        ICompositePrecompiledMvcEngineWrapper Clone();
    }
}
