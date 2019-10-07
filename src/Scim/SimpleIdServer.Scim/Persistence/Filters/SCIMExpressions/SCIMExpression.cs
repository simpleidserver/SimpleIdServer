using System;

namespace SimpleIdServer.Persistence.Filters.SCIMExpressions
{
    public abstract class SCIMExpression : ICloneable
    {
        public abstract object Clone();
    }
}