namespace SimpleIdServer.Persistence.Filters.SCIMExpressions
{
    public class SCIMAttributeExpression : SCIMExpression
    {
        public SCIMAttributeExpression(string name)
        {
            Name = name;
        }

        public SCIMAttributeExpression(string name, SCIMAttributeExpression child) : this(name)
        {
            Child = child;
        }

        public string Name { get; private set; }
        public SCIMAttributeExpression Child { get; private set;}

        public void SetChild(SCIMAttributeExpression child)
        {
            Child = child;
        }

        public override object Clone()
        {
            return ProtectedClone();
        }

        protected virtual object ProtectedClone()
        {
            return new SCIMAttributeExpression(Name, (SCIMAttributeExpression)Child.Clone());
        }
    }
}
