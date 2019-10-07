namespace SimpleIdServer.Persistence.Filters.SCIMExpressions
{
    public class SCIMPresentExpression : SCIMExpression
    {
        public SCIMPresentExpression(SCIMAttributeExpression content)
        {
            Content = content;
        }

        public SCIMAttributeExpression Content { get; private set; }

        public override object Clone()
        {
            return new SCIMPresentExpression((SCIMAttributeExpression)Content.Clone());
        }
    }
}
