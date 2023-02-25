// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Operators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Parser.Expressions
{
    public class SCIMComparisonExpression : SCIMExpression
    {
        public SCIMComparisonExpression(SCIMComparisonOperators comparisonOperator, SCIMAttributeExpression leftExpression, string value)
        {
            ComparisonOperator = comparisonOperator;
            LeftExpression = leftExpression;
            Value = value;
        }

        public SCIMComparisonOperators ComparisonOperator { get; private set; }
        public SCIMAttributeExpression LeftExpression { get; private set; }
        public string Value { get; private set; }

        public override ICollection<SCIMRepresentationAttribute> BuildEmptyAttributes()
        {
            var lastAttrId = LeftExpression.GetLastChild().SchemaAttribute.Id;
            var result = LeftExpression.BuildEmptyAttributes();
            var firstAttr = result.First();
            var lastChild = firstAttr.ToFlat().First(a => a.SchemaAttribute.Id == lastAttrId);
            switch(lastChild.SchemaAttribute.Type)
            {
                case SCIMSchemaAttributeTypes.STRING:
                    lastChild.ValueString = Value;
                    break;
                case SCIMSchemaAttributeTypes.BOOLEAN:
                    lastChild.ValueBoolean = bool.Parse(Value);
                    break;
                case SCIMSchemaAttributeTypes.DECIMAL:
                    lastChild.ValueDecimal = decimal.Parse(Value);
                    break;
                case SCIMSchemaAttributeTypes.DATETIME:
                    lastChild.ValueDateTime = DateTime.Parse(Value);
                    break;
                case SCIMSchemaAttributeTypes.INTEGER:
                    lastChild.ValueInteger = int.Parse(Value);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        public override object Clone()
        {
            return new SCIMComparisonExpression(ComparisonOperator, (SCIMAttributeExpression)LeftExpression.Clone(), Value);
        }
    }
}
