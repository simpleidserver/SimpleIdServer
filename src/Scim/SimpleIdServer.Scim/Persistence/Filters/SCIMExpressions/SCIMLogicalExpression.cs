namespace SimpleIdServer.Persistence.Filters.SCIMExpressions
{
    public class SCIMLogicalExpression : SCIMExpression
    {
        public SCIMLogicalExpression(SCIMLogicalOperators logicalOperator, SCIMExpression leftExpression, SCIMExpression rightExpression)
        {
            LogicalOperator = logicalOperator;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        public SCIMLogicalOperators LogicalOperator { get; private set; }
        public SCIMExpression LeftExpression { get; private set; }
        public SCIMExpression RightExpression{ get; private set; }

        public override object Clone()
        {
            return new SCIMLogicalExpression(LogicalOperator, (SCIMExpression)LeftExpression.Clone(), (SCIMExpression)RightExpression.Clone());
        }
    }
}
