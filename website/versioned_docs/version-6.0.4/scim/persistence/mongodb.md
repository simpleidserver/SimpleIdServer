# Configuring MongoDB for SCIM Server Persistence

Instead of relying on traditional relational databases, [MongoDB](https://www.mongodb.com/) offers a document-oriented approach that can simplify schema management and enhance performance in distributed systems. 
The SCIM server can be configured to connect seamlessly with MongoDB by installing the corresponding NuGet package and updating your application’s startup configuration. With a few lines of code, you can connect to MongoDB, set up your collections, and adjust transaction settings as needed.

To integrate MongoDB with your SCIM server, begin by installing the NuGet package `SimpleIdServer.Scim.Persistence.MongoDB`. This package provides all the necessary functionality to handle the persistence of SCIM data using MongoDB as the storage engine.

```batch title="cmd.exe"
dotnet add package SimpleIdServer.Scim.Persistence.MongoDB
```

Next, update your `Program.cs` file to leverage the MongoDB storage by calling the `UseMongodbStorage` method. 
The configuration process involves specifying key parameters such as the connection string, database name, and various collection names.

The following code snippet demonstrates a sample configuration in your Program.cs file:

```csharp  title="Program.cs"
const string connectionstring = "";
builder.Services.AddScim()
    .UseMongodbStorage(d =>
    {
        d.ConnectionString = connectionstring;
        d.Database = "scim";
        d.CollectionMappings = "mappings";
        d.CollectionRepresentations = "representations";
        d.CollectionSchemas = "schemas";
        d.SupportTransaction = false;
    });
var app = builder.Build();
app.UseScim();
app.Run();
```

Several properties can be customized during the configuration process. Understanding these properties is crucial for tailoring your SCIM server to meet your organizational needs:

| Property | Description |
| -------- | ----------- |
| ConnectionString | This is the connection URI used to establish a connection to your MongoDB instance. An example value is `mongodb://localhost:27017`. |
| Database | The name of the database where your SCIM data will be stored. |
| CollectionRepresentations | Specifies the collection used to store core representations such as users, groups, or other entities. |
| CollectionRepresentationAttributes | Defines the collection for storing specific attributes of the representations—for instance, user first names or other properties. |
| CollectionSchemas | Indicates the collection where schemas are kept. Schemas define the structure and validation rules for the stored data. |
| CollectionRealms | Used for storing realms if your SCIM server segregates data based on different domains or contexts. |
| SupportTransaction | Boolean flag indicating if your MongoDB deployment supports transactions. When set to true, the application can take advantage of atomic operations, which are essential in environments requiring strong consistency. |

For more detailed reference, you can check out the example project available on GitHub: [SimpleIdServer Sample](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ScimMongodb).

## Creating MongoDB Collections

Once your SCIM server is configured and ready to use MongoDB, you can create the necessary collections by calling the `EnsureMongoStoreDataMigrated` function in your `Program.cs` file.

```csharp title="Program.cs"
var app = builder.Build();
app.Services.EnsureMongoStoreDataMigrated(new List<SCIMSchema>(), new List<SCIMAttributeMapping>(), new List<Realm>());;
```

This function takes three input parameters:

1. **List of SCIM Schemas** : You can use the `SCIMSchemaExtractor` class to extract a SCIM schema from a JSON file. For example:

```
var userSchema = SimpleIdServer.Scim.SCIMSchemaExtractor.Extract(Path.Combine(basePath, "UserSchema.json"), SCIMResourceTypes.User, true);
```

2. **List of attribute mappings** : These mappings define parent-child relationships between different representation types. For instance, between a `Group` and a `User`.

The following code snippet defines the relationship from a `Group` to a `User`. The `members` property of a `group` contains a list of `users`:

```csharp
new SCIMAttributeMapping
{
    Id = Guid.NewGuid().ToString(),
    SourceAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id,
    SourceResourceType = StandardSchemas.GroupSchema.ResourceType,
    SourceAttributeSelector = "members",
    TargetResourceType = StandardSchemas.UserSchema.ResourceType,
    TargetAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
    Mode = Mode.PROPAGATE_INHERITANCE
}
```

The next snippet defines the reverse relationship, from a `User` to a `Group`. The `groups` property of a `user` contains a list of `groups`:

```
new SCIMAttributeMapping
{
    Id = Guid.NewGuid().ToString(),
    SourceAttributeId = userSchema.Attributes.First(a => a.Name == "groups").Id,
    SourceResourceType = StandardSchemas.UserSchema.ResourceType,
    SourceAttributeSelector = "groups",
    TargetResourceType = StandardSchemas.GroupSchema.ResourceType,
    TargetAttributeId = groupSchema.Attributes.First(a => a.Name == "members").Id
}
```

3. **List of realms** : The third parameter specifies the list of realms to initialize. For more details, refer to the dedicated chapter on realms.