using System;

namespace SimpleIdServer.Uma.Domains
{
    public class UMAResourcePermissionClaim : ICloneable
    {
        public string ClaimType { get; set; }
        public string FriendlyName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public object Clone()
        {
            return new UMAResourcePermissionClaim
            {
                ClaimType = ClaimType,
                FriendlyName = FriendlyName,
                Name = Name,
                Value = Value
            };
        }
    }
}
