using System;

namespace SimpleIdServer.Scim.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SCIMSchemaPropertyAttribute : Attribute
    {
        public SCIMSchemaPropertyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
