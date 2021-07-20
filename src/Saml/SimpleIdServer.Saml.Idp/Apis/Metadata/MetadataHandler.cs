// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Xsd;

namespace SimpleIdServer.Saml.Idp.Apis.Metadata
{
    public class MetadataHandler : IMetadataHandler
    {
        public EntityDescriptorType Get()
        {
            return new EntityDescriptorType
            {
                Items = new object[]
                {
                    new IDPSSODescriptorType
                    {
                        KeyDescriptor = new KeyDescriptorType[]
                        {

                        },
                        SingleSignOnService = new EndpointType[]
                        {
                            new EndpointType
                            {
                                Location = "http://localhost:7000/SSO/Login"
                            }
                        }
                    }
                }
            };
        }
    }
}
