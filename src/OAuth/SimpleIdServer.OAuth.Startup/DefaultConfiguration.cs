// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Startup
{
    public class DefaultConfiguration
    {
        public static List<OAuthScope> Scopes => new List<OAuthScope>
        {
            new OAuthScope
            {
                Name = "query_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "add_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "delete_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "update_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "bulk_scim_resource",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "manage_clients",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "manage_scopes",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "get_statistic",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "get_performance",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "get_caseplan",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "add_casefile",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "update_casefile",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "publish_casefile",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "add_case_instance",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "launch_case_intance",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "get_casefile",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "search_caseplaninstance",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "add_case_instance",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "launch_caseplaninstance",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "get_forminstances",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "get_caseworkertasks",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "get_caseplaninstance",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "activate_caseplaninstance",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "get_form",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "search_form",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "confirm_caseplaninstance",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "delete_role",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "update_role",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "get_role",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "search_role",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "add_role",
                IsExposedInConfigurationEdp = true
            },
            new OAuthScope
            {
                Name = "create_humantaskinstance",
                IsExposedInConfigurationEdp = true
            }
        };

        public static List<OAuthClient> Clients => new List<OAuthClient>
        {
            new OAuthClient
            {
                ClientId = "bpmnClient",
                ClientSecret = "bpmnClientSecret",
                Translations = new List<OAuthClientTranslation>
                {
                    new OAuthClientTranslation
                    {
                        Translation = new OAuthTranslation("bpmnClient_client_name", "BPMN Client", "fr")
                        {
                            Type = "client_name"
                        }
                    }
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        Name = "create_humantaskinstance"
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                PreferredTokenProfile = "Bearer"
            },
            new OAuthClient
            {
                ClientId = "cmmnClient",
                ClientSecret = "cmmnClientSecret",
                Translations = new List<OAuthClientTranslation>
                {
                    new OAuthClientTranslation
                    {
                        Translation = new OAuthTranslation("cmmnClient_client_name", "CMMN Client", "fr")
                        {
                            Type = "client_name"
                        }
                    }
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        Name = "create_humantaskinstance"
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                PreferredTokenProfile = "Bearer"
            },
            new OAuthClient
            {
                ClientId = "humanTaskClient",
                ClientSecret = "humanTaskClientSecret",
                Translations = new List<OAuthClientTranslation>
                {
                    new OAuthClientTranslation
                    {
                        Translation = new OAuthTranslation("humanTaskClient_client_name", "HumanTask Client", "fr")
                        {
                            Type = "client_name"
                        }
                    }
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        Name = "complete_humantask"
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                PreferredTokenProfile = "Bearer"
            },
            new OAuthClient
            {
                ClientId = "scimClient",
                ClientSecret = "scimClientSecret",
                Translations = new List<OAuthClientTranslation>
                {
                    new OAuthClientTranslation
                    {
                        Translation = new OAuthTranslation("scimClient_client_name", "SCIMClient", "fr")
                        {
                            Type = "client_name"
                        }
                    }
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        Name = "query_scim_resource"
                    },
                    new OAuthScope
                    {
                        Name = "add_scim_resource"
                    },
                    new OAuthScope
                    {
                        Name = "delete_scim_resource"
                    },
                    new OAuthScope
                    {
                        Name = "update_scim_resource"
                    },
                    new OAuthScope
                    {
                        Name = "bulk_scim_resource"
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                PreferredTokenProfile = "Bearer"
            },
            new OAuthClient
            {
                ClientId = "gatewayClient",
                ClientSecret = "gatewayClientPassword",
                Translations = new List<OAuthClientTranslation>
                {
                    new OAuthClientTranslation
                    {
                        Translation = new OAuthTranslation("gatewayClient_client_name", "SCIMClient", "fr")
                        {
                            Type = "client_name"
                        }
                    }
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        Name = "manage_clients"
                    },
                    new OAuthScope
                    {
                        Name = "manage_scopes"
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                PreferredTokenProfile = "Bearer"
            },
            new OAuthClient
            {
                ClientId = "websiteGateway",
                ClientSecret = "websiteGatewaySecret",
                Translations = new List<OAuthClientTranslation>
                {
                    new OAuthClientTranslation
                    {
                        Translation = new OAuthTranslation("websiteGateway_client_name", "Website gateway", "fr")
                        {
                            Type = "client_name"
                        }
                    }
                },
                TokenEndPointAuthMethod = "client_secret_post",
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                TokenExpirationTimeInSeconds = 60 * 30,
                RefreshTokenExpirationTimeInSeconds = 60 * 30,
                TokenSignedResponseAlg = "RS256",
                AllowedScopes = new List<OAuthScope>
                {
                    new OAuthScope
                    {
                        Name = "get_statistic"
                    },
                    new OAuthScope
                    {
                        Name = "get_performance"
                    },
                    new OAuthScope
                    {
                        Name = "get_caseplan"
                    },
                    new OAuthScope
                    {
                        Name = "add_casefile"
                    },
                    new OAuthScope
                    {
                        Name = "update_casefile"
                    },
                    new OAuthScope
                    {
                        Name = "publish_casefile"
                    },
                    new OAuthScope
                    {
                        Name = "add_case_instance"
                    },
                    new OAuthScope
                    {
                        Name = "launch_case_intance"
                    },
                    new OAuthScope
                    {
                        Name = "get_casefile"
                    },
                    new OAuthScope
                    {
                        Name = "search_caseplaninstance"
                    },
                    new OAuthScope
                    {
                        Name = "add_case_instance"
                    },
                    new OAuthScope
                    {
                        Name = "launch_caseplaninstance"
                    },
                    new OAuthScope
                    {
                        Name = "get_forminstances"
                    },
                    new OAuthScope
                    {
                        Name = "get_caseworkertasks"
                    },
                    new OAuthScope
                    {
                        Name = "get_caseplaninstance"
                    },
                    new OAuthScope
                    {
                        Name = "activate_caseplaninstance"
                    },
                    new OAuthScope
                    {
                        Name = "get_form"
                    },
                    new OAuthScope
                    {
                        Name = "search_form"
                    },
                    new OAuthScope
                    {
                        Name = "confirm_caseplaninstance"
                    },
                    new OAuthScope
                    {
                        Name = "delete_role",
                    },
                    new OAuthScope
                    {
                        Name = "update_role"
                    },
                    new OAuthScope
                    {
                        Name = "get_role"
                    },
                    new OAuthScope
                    {
                        Name = "search_role"
                    },
                    new OAuthScope
                    {
                        Name = "add_scope"
                    }
                },
                GrantTypes = new List<string>
                {
                    "client_credentials"
                },
                PreferredTokenProfile = "Bearer"
            }
        };
    }
}