using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleIdServer.Scim.Helpers
{
    public class SCIMFilterParser
    {
        private SCIMFilterParser() { }

        public SCIMExpression Expression { get; private set; }

        public static SCIMExpression Parse(string filterString)
        {
            if (string.IsNullOrWhiteSpace(filterString))
            {
                return null;
            }

            filterString = filterString.Trim();
            var result = new SCIMFilterParser();
            var strBuilder = new StringBuilder();
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

            throw new SCIMFilterException("bad_filter", $"{string.Join(" ", filters)} filters are not valid");
        }

        private static SCIMExpression TransformStringFiltersIntoSCIMPresentExpression(IEnumerable<string> filters)
        {
            var attributeExpression = Parse(filters.First()) as SCIMAttributeExpression;
            if (attributeExpression == null)
            {
                throw new SCIMFilterException("bad_filter", "present expression can only be used with attribute");
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
                    throw new SCIMFilterException("bad_filter", "logical expression is not well formatted");
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
                throw new SCIMFilterException("bad_filter", "not filter is not well formatted");
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
                throw new SCIMFilterException("bad_filter", $"the comparison operator {scimOperator} is not supported");
            }

            return new SCIMComparisonExpression(op, leftExpression, filters.Last());
        }

        private static SCIMExpression TransformStringFiltersIntoSCIMNavigationExpression(IEnumerable<string> filters)
        {
            var firstFilter = filters.First();
            var fullPath = firstFilter;
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
                        parentAttributeExpression = new SCIMAttributeExpression(currentMatch.Value);
                    }
                    else
                    {
                        parentAttributeExpression = new SCIMComplexAttributeExpression(currentMatch.Value, Parse(complexAttributeFiltering));
                    }

                    result = parentAttributeExpression;
                    continue;
                }

                SCIMAttributeExpression subAttributeExpression;
                if (string.IsNullOrWhiteSpace(complexAttributeFiltering))
                {
                    subAttributeExpression = new SCIMAttributeExpression(currentMatch.Value);
                }
                else
                {
                    subAttributeExpression = new SCIMComplexAttributeExpression(currentMatch.Value, Parse(complexAttributeFiltering));
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

        private static IEnumerable<string> SplitStringIntoFilters(string filterString)
        {
            var level = 0;
            var result = new List<string>();
            var filterBuilder = new StringBuilder();
            var groupingIsClosed = false;
            var isTokenSeparatorDetected = false;
            for (var i = 0; i < filterString.Count(); i++)
            {
                var character = filterString[i];
                if (new[] { '(', '[' }.Contains(character))
                {
                    level++;
                }

                if (new[] { ')', ']' }.Contains(character))
                {
                    level--;
                    if (character == ')')
                    {
                        groupingIsClosed = true;
                    }
                }

                if (!(character == ' ' && level == 0))
                {
                    filterBuilder.Append(character);
                }
                else if (filterBuilder.Length > 0 && character == ' ' && !filterBuilder.ToString().Equals("not", StringComparison.InvariantCultureIgnoreCase))
                {
                    isTokenSeparatorDetected = true;
                }

                if ((level == 0 && (groupingIsClosed || isTokenSeparatorDetected)) || IsStandardOperand(filterBuilder.ToString()) || i == filterString.Count() - 1)
                {
                    var record = filterBuilder.ToString();
                    result.Add(filterBuilder.ToString());
                    filterBuilder.Clear();
                    groupingIsClosed = false;
                    isTokenSeparatorDetected = false;
                    continue;
                }
            }

            if (!result.Any(r => IsLogicalOperator(r)))
            {
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

        public LambdaExpression Evaluate(IQueryable<SCIMRepresentation> representations)
        {
            // return Expression.Evaluate(representations);
            return null;
        }
    }
}
