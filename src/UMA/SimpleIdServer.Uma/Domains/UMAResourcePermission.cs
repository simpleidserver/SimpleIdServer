using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Uma.Domains
{
    public class UMAResourcePermission : ICloneable, IEquatable<UMAResourcePermission>
    {
        public UMAResourcePermission()
        {
            Scopes = new List<string>();
        }

        public UMAResourcePermission(string subject, ICollection<string> scopes)
        {
            Subject = subject;
            Scopes = scopes;
        }

        public string Subject { get; set; }
        public ICollection<string> Scopes { get; set; }

        public object Clone()
        {
            return new UMAResourcePermission
            {
                Subject = Subject,
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
            return Subject.GetHashCode();
        }
    }
}
