// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains.DTOs;

public static class AuditingEventNames
{
    public const string Id = "id";
    public const string EventName = "name";
    public const string Realm = "realm";
    public const string IsError = "is_error";
    public const string Description = "description";
    public const string ErrorMessage = "error_message";
    public const string CreateDateTime = "create_datetime";
    public const string ClientId = "client_id";
    public const string UserName = "username";
    public const string RequestJSON = "request_json";
    public const string RedirectUrl = "redirect_url";
    public const string AuthMethod = "auth_method";
    public const string Scopes = "scopes";
    public const string Claims = "claims";
}
