# MongoDB Performance Optimization for SCIM

## Problem Statement

The original issue reported that SCIM filtering on MongoDB was generating inefficient queries like:

```json
{
  "Attribute.SchemaAttributeId": "26d51050-4962-4348-a6cb-310c198eeee3",
  "$expr": {
    "$eq": [
      { "$toLower": { "$ifNull": ["$Attribute.ValueString", ""] } },
      "150017355"
    ]
  }
}
```

This query pattern forced MongoDB into full collection scans because:
1. `$expr` with `$toLower` and `$ifNull` functions prevent index usage
2. MongoDB cannot use indexes when expressions involve function calls in `$expr`
3. Large collections (859k documents) result in poor performance (~2.11 minutes)

## Solution Overview

The solution implements MongoDB-specific optimizations for case-insensitive string comparisons in the SCIM persistence layer.

### Key Changes

1. **Optimized Expression Engine**: New `MongoDbOptimizedExpressionExtensions` class that generates regex-based queries instead of `$expr` queries
2. **Enhanced Indexes**: Additional compound and case-insensitive collation indexes for better query performance
3. **Selective Optimization**: Only case-insensitive string operations are optimized, preserving existing behavior for other data types

### Technical Details

#### Before (Problematic Pattern)
```csharp
// Generated inefficient $expr query with $toLower/$ifNull
e1 = Expression.Coalesce(e1, Expression.Constant(string.Empty));
e1 = Expression.Call(e1, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
e2 = Expression.Call(e2, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
return Expression.Equal(e1, e2);
```

#### After (Optimized Pattern)
```csharp
// Uses regex patterns that can leverage MongoDB indexes
var regexPattern = $"^{Regex.Escape(value)}$";
var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
// MongoDB can use indexes for anchored regex patterns
```

#### New Index Strategy
```csharp
// Additional indexes for better performance
var schemaAttributeValueIndex = Builders<SCIMRepresentationAttribute>.IndexKeys
    .Ascending(a => a.SchemaAttributeId)
    .Ascending(a => a.ValueString);

var caseInsensitiveCollation = new Collation("en", strength: CollationStrength.Secondary);
var valueStringIndex = Builders<SCIMRepresentationAttribute>.IndexKeys
    .Ascending(a => a.ValueString);
```

## Performance Impact

The optimization specifically targets the query pattern that was causing performance issues:

- **Query Type**: `eventid eq "150017355"` (case-insensitive equality)
- **Before**: Full collection scan with `$expr` functions
- **After**: Index-supported regex pattern matching
- **Index Usage**: Leverages `SchemaAttributeId_1_ValueString_1` compound index

## Backward Compatibility

- ✅ Existing case-sensitive queries unchanged
- ✅ Non-string attribute queries unchanged  
- ✅ Complex attribute expressions unchanged
- ✅ All existing tests continue to pass
- ✅ Only optimizes problematic case-insensitive string operations

## Usage

The optimization is automatically applied when:
1. Using MongoDB persistence layer
2. Filtering on string attributes with `CaseExact = false`
3. Using equality (`eq`), starts-with (`sw`), ends-with (`ew`), or contains (`co`) operations

No changes required in application code - the optimization is transparent to SCIM API consumers.