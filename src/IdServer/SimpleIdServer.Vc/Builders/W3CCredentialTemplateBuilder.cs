using SimpleIdServer.Vc.Models;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Vc.Builders
{
    public class W3CCredentialTemplateBuilder : BaseCredentialTemplateBuilder<W3CCredentialTemplate>
    {
        protected W3CCredentialTemplateBuilder(W3CCredentialTemplate credentialTemplate) : base(credentialTemplate) { }

        public static W3CCredentialTemplateBuilder New(string name, string logoUrl, string type)
        {
            var result = new W3CCredentialTemplate
            {
                TechnicalId = Guid.NewGuid().ToString(),
                Id = $"{name}_JWT",
                Format = Vc.Constants.CredentialTemplateProfiles.W3CVerifiableCredentials,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                DisplayLst = new List<CredentialTemplateDisplay>
                {
                    new CredentialTemplateDisplay
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = name,
                        LogoUrl = logoUrl,
                        Locale = "en-US"
                    }
                }
            };
            result.AddType(Constants.DefaultVerifiableCredentialType);
            result.AddType(type);
            return new W3CCredentialTemplateBuilder(result);
        }

        public W3CCredentialTemplateBuilder AddStringCredentialSubject(string claimName, bool mandatory, ICollection<W3CCredentialSubjectDisplay> display) => AddCredentialSubject(claimName, mandatory, Constants.CredentialSubjectDisplayTypes.String, display);

        public W3CCredentialTemplateBuilder AddCredentialSubject(string claimName, bool mandatory, string valueType, ICollection<W3CCredentialSubjectDisplay> display)
        {
            CredentialTemplate.AddCredentialSubject(claimName, new W3CCredentialSubject
            {
                Mandatory = mandatory,
                ValueType = valueType,
                Display = display
            });
            return this;
        }
    }
}
