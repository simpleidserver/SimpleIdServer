using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenID.Domains
{
    public class OpenIdScope : OAuthScope
    {
        public OpenIdScope()
        {
            Claims = new List<string>();
        }

        public OpenIdScope(string name) : base(name)
        {
            Claims = new List<string>();
        }

        public ICollection<string> Claims { get; set; }

        public override object Clone()
        {
            return new OpenIdScope
            {
                Claims = Claims.ToList(),
                CreateDateTime = CreateDateTime,
                IsExposedInConfigurationEdp = IsExposedInConfigurationEdp,
                Name = Name,
                UpdateDateTime = UpdateDateTime
            };
        }
    }
}
