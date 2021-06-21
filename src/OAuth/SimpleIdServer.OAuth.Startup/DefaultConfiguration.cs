// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Startup
{
    public class DefaultConfiguration
    {
        public static OAuthScope QueryScimResource = new OAuthScope
        {
            Name = "query_scim_resource",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope AddScimResource = new OAuthScope
        {
            Name = "add_scim_resource",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope DeleteScimResource = new OAuthScope
        {
            Name = "delete_scim_resource",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope UpdateScimResource = new OAuthScope
        {
            Name = "update_scim_resource",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope BulkScimResource = new OAuthScope
        {
            Name = "bulk_scim_resource",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope ManageClients = new OAuthScope
        {
            Name = "manage_clients",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope ManageScopes = new OAuthScope
        {
            Name = "manage_scopes",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope GetStatistic = new OAuthScope
        {
            Name = "get_statistic",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope GetPerformance = new OAuthScope
        {
            Name = "get_performance",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope GetCasePlan = new OAuthScope
        {
            Name = "get_caseplan",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope AddCaseFile = new OAuthScope
        {
            Name = "add_casefile",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope UpdateCaseFile = new OAuthScope
        {
            Name = "update_casefile",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope PublishCaseFile = new OAuthScope
        {
            Name = "publish_casefile",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope AddCaseInstance = new OAuthScope
        {
            Name = "add_case_instance",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope LaunchCaseInstance = new OAuthScope
        {
            Name = "launch_case_intance",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope GetCaseFile = new OAuthScope
        {
            Name = "get_casefile",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope SearchCasePlanInstance = new OAuthScope
        {
            Name = "search_caseplaninstance",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope LaunchCasePlanInstance = new OAuthScope
        {
            Name = "launch_caseplaninstance",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope GetFormInstances = new OAuthScope
        {
            Name = "get_forminstances",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope GetCaseWorkerTasks = new OAuthScope
        {
            Name = "get_caseworkertasks",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope GetCasePlanInstance = new OAuthScope
        {
            Name = "get_caseplaninstance",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope ActiveCasePlanInstance = new OAuthScope
        {
            Name = "activate_caseplaninstance",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope GetForm = new OAuthScope
        {
            Name = "get_form",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope SearchForm = new OAuthScope
        {
            Name = "search_form",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope ConfirmCasePlanInstance = new OAuthScope
        {
            Name = "confirm_caseplaninstance",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope DeleteRole = new OAuthScope
        {
            Name = "delete_role",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope UpdateRole = new OAuthScope
        {
            Name = "update_role",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope GetRole = new OAuthScope
        {
            Name = "get_role",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope SearchRole = new OAuthScope
        {
            Name = "search_role",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope AddRole = new OAuthScope
        {
            Name = "add_role",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope CreateHumanTaskInstance = new OAuthScope
        {
            Name = "create_humantaskinstance",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope CompleteHumanTask = new OAuthScope
        {
            Name = "complete_humantask",
            IsExposedInConfigurationEdp = true
        };
        public static OAuthScope AddScope = new OAuthScope
        {
            Name = "add_scope",
            IsExposedInConfigurationEdp = true
        };

        public static List<OAuthScope> Scopes => new List<OAuthScope>
        {
            QueryScimResource,
            AddScimResource,
            DeleteScimResource,
            UpdateScimResource,
            BulkScimResource,
            ManageClients,
            ManageScopes,
            GetStatistic,
            GetPerformance,
            GetCasePlan,
            AddCaseFile,
            UpdateCaseFile,
            PublishCaseFile,
            AddCaseInstance,
            LaunchCaseInstance,
            GetCaseFile,
            SearchCasePlanInstance,
            LaunchCasePlanInstance,
            GetFormInstances,
            GetCaseWorkerTasks,
            GetCasePlanInstance,
            ActiveCasePlanInstance,
            GetForm,
            SearchForm,
            ConfirmCasePlanInstance,
            DeleteRole,
            UpdateRole,
            GetRole,
            SearchRole,
            AddRole,
            CreateHumanTaskInstance,
            CompleteHumanTask,
            AddScope
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
                    CreateHumanTaskInstance
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
                    CreateHumanTaskInstance
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
                    CompleteHumanTask
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
                    QueryScimResource,
                    AddScimResource,
                    DeleteScimResource,
                    UpdateScimResource,
                    BulkScimResource
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
                    ManageClients,
                    ManageScopes
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
                        Translation =  new OAuthTranslation("websiteGateway_client_name", "Website gateway", "fr")
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
                    GetStatistic,
                    GetPerformance,
                    GetCasePlan,
                    AddCaseFile,
                    UpdateCaseFile,
                    PublishCaseFile,
                    AddCaseInstance,
                    LaunchCaseInstance,
                    GetCaseFile,
                    SearchCasePlanInstance,
                    LaunchCaseInstance,
                    GetFormInstances,
                    GetCaseWorkerTasks,
                    GetCasePlanInstance,
                    ActiveCasePlanInstance,
                    GetForm,
                    SearchForm,
                    ConfirmCasePlanInstance,
                    DeleteRole,
                    UpdateRole,
                    GetRole,
                    SearchRole,
                    AddScope
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