using System;
using System.Collections.Generic;

namespace SimpleIdServer.FastFed.ApplicationProvider.Models;

public class IdentityProviderFederation
{
    public string EntityId { get; set; } = null!;
    public string JwksUri { get; set; } = null!;
    public List<string> AuthenticationProfiles { get; set; }
    public List<string> ProvisioningProfiles { get; set; }
    /// <summary>
    /// After which the whitelisting will be considered expired. I
    /// </summary>
    public DateTime ExpirationDateTime { get; set; }
    public bool IsActive { get; set; } = false;
    public bool IsConfirmedByAdministrator { get; set; } = false;
    public DateTime CreateDateTime { get; set; }
}