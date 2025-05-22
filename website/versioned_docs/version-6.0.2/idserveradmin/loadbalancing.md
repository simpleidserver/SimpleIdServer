# Load balancing

When an administration site is deployed across multiple instances behind a load balancer, it is crucial that all instances share the same cryptographic keys. 
Without a consistent key store, users might experience failed authentications or session inconsistencies because each server could potentially use different keys to encrypt and decrypt data. Persisting these keys in a centralized location ensures that every instance of the administration site can access the necessary keys to operate seamlessly.

## Using a Shared Directory for Cryptographic Keys

The current best practice involves storing the cryptographic keys in a shared directory that is accessible to all instances of the administration site. 
This approach not only simplifies key management but also guarantees consistency across servers. 
By persisting the keys in a single, common location, the risk of key mismatch is minimized, and overall system reliability is enhanced.

A practical way to achieve this in a .NET environment is to edit the `Program.cs` file of your application. 
The key step involves calling the `PersistDataprotection` method from the fluent API provided by your development framework. This method takes as its parameter the path to the shared directory where the keys will be stored.

Here’s a simple example of how you might implement this in C#:

```csharp  title="Program.cs"
adminBuilder.PersistDataprotection("C:\");
```

In this code snippet, the `PersistDataprotection` method is invoked with the `DataProtectionPath` property, ensuring that the cryptographic keys are stored in a location accessible to all instances of the administration site.