// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Exceptions;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Parser.Operators;
using SimpleIdServer.Scim.Parser.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleIdServer.Scim.Parser
{
    public class SCIMFilterParser
    {
        private SCIMFilterParser() { }

        public SCIMExpression Expression { get; private set; }

        public static bool DontContainsFilter(string filterString)
        {
            var regex = new Regex(@"(\w|\.)*");
            return regex.IsMatch(filterString);
        }

        public static SCIMExpression Parse(string filterString, ICollection<SCIMSchema> scimSchemas)
        {
            var scimExpression = Parse(filterString);
            Parse(scimExpression, scimSchemas.SelectMany(s => s.HierarchicalAttributes.Select(h => h.Leaf)).ToList(), scimSchemas);
            return scimExpression;
        }

        private static void Parse(SCIMExpression expression, ICollection<SCIMSchemaAttribute> scimSchemaAttributes, ICollection<SCIMSchema> schemas)
        {
            var compAttrExpression = expression as SCIMComparisonExpression;
            var attrExpression = expression as SCIMAttributeExpression;
            var logicalExpression = expression as SCIMLogicalExpression;
            var notExpression = expression as SCIMNotExpression;
            var presentExpression = expression as SCIMPresentExpression;
            if (compAttrExpression != null)
            {
                var schemaAttr = GetSCIMSchemaAttribute(compAttrExpression.LeftExpression.Name, scimSchemaAttributes, schemas);
                if (schemaAttr != null && schemaAttr.MultiValued && schemaAttr.Type == SCIMSchemaAttributeTypes.COMPLEX && compAttrExpression.LeftExpression.Child == null)
                {
                    var schema = schemas.FirstOrDefault(s => s.HasAttribute(schemaAttr));
                    var children = schema.GetChildren(schemaAttr);
                    compAttrExpression.LeftExpression.Child = new SCIMAttributeExpression("value")
                    {
                        SchemaAttribute = children.First(c => c.Name == "value")
                    };
                }

                Parse(compAttrExpression.LeftExpression, scimSchemaAttributes, schemas);
            }
            else if (logicalExpression != null)
            {
                Parse(logicalExpression.LeftExpression, scimSchemaAttributes, schemas);
                Parse(logicalExpression.RightExpression, scimSchemaAttributes, schemas);
            }
            else if (attrExpression != null)
            {
                var schemaAttr = GetSCIMSchemaAttribute(attrExpression.Name, scimSchemaAttributes, schemas);
                if (schemaAttr == null)
                {
                    return;
                }

                attrExpression.SchemaAttribute = schemaAttr;
                var complex = attrExpression as SCIMComplexAttributeExpression;
                if (attrExpression.Child != null || complex != null)
                {
                    var schema = schemas.FirstOrDefault(s => s.HasAttribute(attrExpression.SchemaAttribute));
                    var subAttributes = schema.GetChildren(attrExpression.SchemaAttribute).ToList();
                    if (attrExpression.Child != null)
                    {
                        Parse(attrExpression.Child, subAttributes, schemas);
                    }

                    if (complex != null && complex.GroupingFilter != null)
                    {
                        Parse(complex.GroupingFilter, subAttributes, schemas);
                    }
                }
            }
            else if (presentExpression != null)
            {
                var schemaAttr = GetSCIMSchemaAttribute(presentExpression.Content.Name, scimSchemaAttributes, schemas);
                if (schemaAttr == null) return;
                if (presentExpression.Content.Child != null)
                {
                    var schema = schemas.FirstOrDefault(s => s.HasAttribute(schemaAttr));
                    var subAttributes = schema.GetChildren(schemaAttr).ToList();
                    Parse(presentExpression.Content.Child, subAttributes, schemas);
                }

                presentExpression.Content.SchemaAttribute = schemaAttr;
            }
            else if (notExpression != null)
            {
                Parse(notExpression.Content, scimSchemaAttributes, schemas);
            }
        }

        public static SCIMExpression Parse(string filterString)
        {
            if (string.IsNullOrWhiteSpace(filterString))
            {
                return null;
            }

            filterString = filterString.Trim();
            var filters = SplitStringIntoFilters(filterString);
            return TransformStringFiltersIntoSCIMExpression(filters);
        }

        private static SCIMExpression TransformStringFiltersIntoSCIMExpression(IEnumerable<string> filters)
        {
            if(ContainsCombinedLogicalExpression(filters))
            {
                return TransformStringFiltersIntoSCIMLogicalExpression(filters);
            }

            if (ContainsNotLogicalExpression(filters))
            {
                return TransformStringFiltersIntoSCIMNotLogicalExpression(filters);
            }

            if (ContainsComparisonExpression(filters))
            {
                return TransformStringFiltersIntoSCIMComparisonExpression(filters);
            }

            if (ContainsNavigationAttributeExpression(filters))
            {
                return TransformStringFiltersIntoSCIMNavigationExpression(filters);
            }

            if (ContainsPresentExpression(filters))
            {
                return TransformStringFiltersIntoSCIMPresentExpression(filters);
            }

            throw new SCIMFilterException(string.Format(Global.InvalidFilters, string.Join(" ", filters)));
        }

        private static SCIMExpression TransformStringFiltersIntoSCIMPresentExpression(IEnumerable<string> filters)
        {
            var attributeExpression = Parse(filters.First()) as SCIMAttributeExpression;
            if (attributeExpression == null)
            {
                throw new SCIMFilterException(Global.BadPresentExpressionUsage);
            }

            return new SCIMPresentExpression(attributeExpression);
        }

        private static SCIMExpression TransformStringFiltersIntoSCIMLogicalExpression(IEnumerable<string> filters)
        {
            var logicalOperatorIndexes = new List<int>();
            for(var i = 0; i < filters.Count(); i++)
            {
                SCIMLogicalOperators op;
                if (Enum.GetNames(typeof(SCIMLogicalOperators)).Any(s => s.Equals(filters.ElementAt(i), StringComparison.InvariantCultureIgnoreCase)) && Enum.TryParse(filters.ElementAt(i), true, out op))
                {
                    logicalOperatorIndexes.Add(i);
                }
            }

            SCIMLogicalExpression rootExpression = null;
            foreach(var logicalOperatorIndex in logicalOperatorIndexes)
            {
                if(logicalOperatorIndex - 1 < 0 || logicalOperatorIndex + 1 >= filters.Count())
                {
                    throw new SCIMFilterException(Global.BadLogicalExpression);
                }

                var rightExpression = filters.ElementAt(logicalOperatorIndex + 1);
                var logicalOperator = filters.ElementAt(logicalOperatorIndex);
                if (rootExpression == null)
                {
                    var leftExpression = filters.ElementAt(logicalOperatorIndex - 1);
                    rootExpression = new SCIMLogicalExpression((SCIMLogicalOperators)Enum.Parse(typeof(SCIMLogicalOperators),logicalOperator, true), Parse(leftExpression), Parse(rightExpression));
                    continue;
                }

                rootExpression = new SCIMLogicalExpression(SCIMLogicalOperators.AND, (SCIMLogicalExpression)rootExpression.Clone(), Parse(rightExpression));
            }

            return rootExpression;
        }

        private static SCIMExpression TransformStringFiltersIntoSCIMNotLogicalExpression(IEnumerable<string> filters)
        {
            var notFilter = filters.First();
            var regex = new Regex(@"^(\s)*not(\s)*\(.*\)$");
            if (!regex.IsMatch(notFilter))
            {
                throw new SCIMFilterException(Global.BadNotFilterExpression);
            }

            var subFilter = (new Regex(@"\(.*\)")).Match(notFilter).Value.TrimStart('(').TrimEnd(')');
            return new SCIMNotExpression(Parse(subFilter));
        }

        private static SCIMExpression TransformStringFiltersIntoSCIMComparisonExpression(IEnumerable<string> filters)
        {
            var leftExpression = (SCIMAttributeExpression)Parse(filters.First());
            var scimOperator = filters.ElementAt(1);
            SCIMComparisonOperators op;
            if (!Enum.TryParse(scimOperator, true, out op))
            {
                throw new SCIMFilterException(string.Format(Global.ComparisonOperatorNotSupported, scimOperator));
            }

            return new SCIMComparisonExpression(op, leftExpression, filters.Last().Trim('"'));
        }

        private static SCIMExpression TransformStringFiltersIntoSCIMNavigationExpression(IEnumerable<string> filters)
        {
            var firstFilter = filters.First();
            var fullPath = firstFilter;
            var namespaceStr = SCIMAttributeExpression.ExtractNamespace(fullPath);
            if (!string.IsNullOrWhiteSpace(namespaceStr))
            {
                fullPath = fullPath.Replace(namespaceStr, string.Empty);
            }

            var regex = new Regex(@"(\[.*\])|((\w|\s))*");
            var matches = regex.Matches(fullPath);
            SCIMAttributeExpression result = null;
            SCIMAttributeExpression parentAttributeExpression = null;
            for (var i = 0; i < matches.Count; i++)
            {
                var currentMatch = matches[i];
                if (string.IsNullOrWhiteSpace(currentMatch.Value))
                {
                    continue;
                }

                var currentMatchValue = currentMatch.Value;
                if (!string.IsNullOrWhiteSpace(namespaceStr) && parentAttributeExpression == null)
                {
                    currentMatchValue = $"{namespaceStr}:{currentMatchValue}";
                }

                var complexAttributeFiltering = string.Empty;
                if ((i + 1) < matches.Count && matches[i + 1].Value.StartsWith("[") && matches[i + 1].Value.EndsWith("]"))
                {
                    complexAttributeFiltering = matches[i + 1].Value.TrimStart('[').TrimEnd(']');
                    i++;
                }

                if (parentAttributeExpression == null)
                {
                    if (string.IsNullOrWhiteSpace(complexAttributeFiltering))
                    {
                        parentAttributeExpression = new SCIMAttributeExpression(currentMatchValue);
                    }
                    else
                    {
                        parentAttributeExpression = new SCIMComplexAttributeExpression(currentMatchValue, Parse(complexAttributeFiltering));
                    }

                    result = parentAttributeExpression;
                    continue;
                }

                SCIMAttributeExpression subAttributeExpression;
                if (string.IsNullOrWhiteSpace(complexAttributeFiltering))
                {
                    subAttributeExpression = new SCIMAttributeExpression(currentMatchValue);
                }
                else
                {
                    subAttributeExpression = new SCIMComplexAttributeExpression(currentMatchValue, Parse(complexAttributeFiltering));
                }

                parentAttributeExpression.SetChild(subAttributeExpression);
                parentAttributeExpression = subAttributeExpression;
            }

            return result;
        }

        private static bool ContainsCombinedLogicalExpression(IEnumerable<string> filters)
        {
            return Enum.GetNames(typeof(SCIMLogicalOperators)).Any(o => filters.Any(f => o.Equals(f, StringComparison.InvariantCultureIgnoreCase)));
        }

        private static bool ContainsComparisonExpression(IEnumerable<string> filters)
        {
            if (filters.Count() != 3)
            {
                return false;
            }

            return Enum.GetNames(typeof(SCIMComparisonOperators)).Any(o => filters.Any(f => o.Equals(f, StringComparison.InvariantCultureIgnoreCase)));
        }

        private static bool ContainsNavigationAttributeExpression(IEnumerable<string> filters)
        {
            if (filters.Count() != 1)
            {
                return false;
            }

            return true;
        }

        private static bool ContainsPresentExpression(IEnumerable<string> filters)
        {
            if (filters.Count() != 2 || !filters.Last().Equals("pr", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private static bool ContainsNotLogicalExpression(IEnumerable<string> filters)
        {
            return new List<string>
            {
                "not"
            }.Any(o => filters.Any(f => f.StartsWith(o, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static IEnumerable<string> SplitStringIntoFilters(string filterString)
        {
            var level = 0;
            var nbGroupingRoot = 0;
            var result = new List<string>();
            var filterBuilder = new StringBuilder();
            var groupingIsClosed = false;
            var groupingIsOpened = false;
            var isTokenSeparatorDetected = false;
            var betweenQuotes = false;
            for (var i = 0; i < filterString.Count(); i++)
            {
                var character = filterString[i];
                if (character == '"')
                {
                    betweenQuotes = !betweenQuotes;
                }

                if ('[' == character)
                {
                    level++;
                }

                if ('(' == character && (string.IsNullOrWhiteSpace(filterBuilder.ToString()) || filterBuilder.ToString() == "not"))
                {
                    if ('(' == character && level == 0 && filterBuilder.Length == 0) nbGroupingRoot++;
                    level++;
                    groupingIsOpened = true;
                }

                if (']' == character)
                {
                    level--;
                }

                if (')' == character && groupingIsOpened)
                {
                    level--;
                    groupingIsOpened = false;
                    groupingIsClosed = true;
                }

                if (character != ' ' || level != 0 || betweenQuotes)
                {
                    filterBuilder.Append(character);
                }
                else if (filterBuilder.Length > 0 && character == ' ' && !filterBuilder.ToString().Equals("not", StringComparison.InvariantCultureIgnoreCase))
                {
                    isTokenSeparatorDetected = true;
                }

                if ((level == 0 && (groupingIsClosed || isTokenSeparatorDetected))
                    || (IsStandardOperand(filterBuilder.ToString()) && result.Any() && !IsStandardOperand(result.Last()))
                    || i == filterString.Count() - 1)
                {
                    var record = filterBuilder.ToString();
                    result.Add(CleanFilter(filterBuilder.ToString()));
                    filterBuilder.Clear();
                    groupingIsClosed = false;
                    isTokenSeparatorDetected = false;
                    continue;
                }
            }

            if (!result.Any(r => IsLogicalOperator(r)))
            {
                if (nbGroupingRoot == 1) return SplitStringIntoFilters(result.First());
                return result;
            }

            var groupedResult = new List<string>();
            for(var i = 0; i < result.Count(); i++)
            {
                var record = result[i];
                if (IsLogicalOperator(record))
                {
                    var leftValues = new List<string>();
                    for(var y = i - 1; y >= 0; y--)
                    {
                        if (IsLogicalOperator(result[y]))
                        {
                            break;
                        }

                        leftValues.Add(result[y]);
                    }

                    var rightValues = result.Skip(i + 1).TakeWhile(s => !IsLogicalOperator(s));
                    leftValues.Reverse();
                    groupedResult.Add(string.Join(" ", leftValues));
                    groupedResult.Add(record);
                    if (rightValues.Count() == result.Count() - 1 - i)
                    {
                        groupedResult.Add(string.Join(" ", rightValues));
                    }
                }
            }

            return groupedResult;
        }

        private static string CleanFilter(string filter)
        {
            if (filter.StartsWith("(") && filter.EndsWith(")"))
            {
                return filter.TrimStart('(').TrimEnd(')');
            }

            return filter;
        }

        private static bool IsStandardOperand(string str)
        {
            var lst = Enum.GetNames(typeof(SCIMComparisonOperators)).ToList();
            lst.AddRange(Enum.GetNames(typeof(SCIMLogicalOperators)));
            return lst.Any(s => str.Equals(s, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool IsLogicalOperator(string str)
        {
            var lst = Enum.GetNames(typeof(SCIMLogicalOperators)).ToList();
            return lst.Any(s => str.Equals(s, StringComparison.InvariantCultureIgnoreCase));
        }

        private static SCIMSchemaAttribute GetSCIMSchemaAttribute(string name,
            ICollection<SCIMSchemaAttribute> scimSchemaAttributes, ICollection<SCIMSchema> schemas)
        {
            foreach (var scimSchemaAttribute in scimSchemaAttributes)
            {
                var schema = schemas.First(s => s.HasAttribute(scimSchemaAttribute));
                var fullPath = $"{schema.Id}:{scimSchemaAttribute.Name}";
                if ((SCIMAttributeExpression.HasNamespace(name) && name == fullPath) || (!SCIMAttributeExpression.HasNamespace(name) && string.Equals(name, scimSchemaAttribute.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return scimSchemaAttribute;
                }
            }

            return null;
        }

        public LambdaExpression Evaluate(IQueryable<SCIMRepresentation> representations)
        {
            // return Expression.Evaluate(representations);
            return null;
        }
    }
}
