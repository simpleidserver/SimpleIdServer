using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Uma.Domains
{
    public class UMAResourcePermission : ICloneable, IEquatable<UMAResourcePermission>
    {
        public UMAResourcePermission(string id, DateTime createDateTime)
        {
            Id = id;
            CreateDateTime = createDateTime;
            Claims = new List<UMAResourcePermissionClaim>();
            Scopes = new List<string>();
        }

        public UMAResourcePermission(string id, DateTime createDatTime, ICollection<string> scopes)  : this(id, createDatTime)
        {
            Scopes = scopes;
        }
        
        public string Id { get; set; }
        public ICollection<UMAResourcePermissionClaim> Claims { get; set; }
        public ICollection<string> Scopes { get; set; }
        public DateTime CreateDateTime { get; set; }

        public object Clone()
        {
            return new UMAResourcePermission(Id, CreateDateTime)
            {
                Claims = Claims.Select(c => (UMAResourcePermissionClaim)c.Clone()).ToList(),
                Scopes = Scopes.ToList()
            };
        }

        public bool Equals(UMAResourcePermission other)
        {
            if (other == null)
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
