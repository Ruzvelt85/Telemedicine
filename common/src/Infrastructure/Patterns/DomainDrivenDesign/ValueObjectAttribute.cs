using System;

namespace Telemedicine.Common.Infrastructure.Patterns.DomainDrivenDesign
{
    /// <summary>
    /// It's a declarative attribute to mark Value objects, like a hint for developers. It's not used implicitly anywhere through reflection
    /// Value object should not have property Id, but it has because it is its database primary key.
    /// </summary>
    public class ValueObjectAttribute : Attribute
    {
    }
}
