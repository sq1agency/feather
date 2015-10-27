using System;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// This attribute is used to mark assemblies that may contain <see cref="IController"/> types.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ControllerContainerAttribute : Attribute
    {
        public ControllerContainerAttribute()
            : this(null, null)
        {
        }

        public ControllerContainerAttribute(Type initializationType, string initializationMethodName)
            : this(initializationType, initializationMethodName, null)
        {
        }

        public ControllerContainerAttribute(Type initializationType, string initializationMethodName, string package)
        {
            this.InitializationType = initializationType;
            this.InitializationMethod = initializationMethodName;
            this.Package = package;
        }

        public Type InitializationType { get; private set; }

        public string InitializationMethod { get; private set; }

        public string Package { get; private set; }
    }
}
