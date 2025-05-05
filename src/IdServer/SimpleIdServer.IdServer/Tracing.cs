// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Diagnostics;

namespace SimpleIdServer.IdServer;

public static class Tracing
{
    public static ActivitySource UserActivitySource = new(Names.User);

    public static ActivitySource ApiResourceActivitySource = new(Names.ApiResource);

    public static ActivitySource AcrActivitySource = new(Names.Acr);

    public static ActivitySource CaActivitySource = new(Names.Ca);

    public static ActivitySource ClientActivitySource = new(Names.Client);

    public static ActivitySource GroupActivitySource = new(Names.Group);

    public static ActivitySource RealmActivitySource = new(Names.Realm);

    public static ActivitySource ScopeActivitySource = new(Names.Scope);

    public static ActivitySource IdserverActivitySource = new(Names.Idserver);

    internal static void Init()
    {
        UserActivitySource = new(Names.User);
        ApiResourceActivitySource = new(Names.ApiResource);
        AcrActivitySource = new(Names.Acr);
        CaActivitySource = new(Names.Ca);
        ClientActivitySource = new(Names.Client);
        GroupActivitySource = new(Names.Group);
        RealmActivitySource = new(Names.Realm);
        ScopeActivitySource = new(Names.Scope);
        IdserverActivitySource = new(Names.Idserver);
    }

    public static class CommonTagNames
    {
        public const string Realm  = "realm";
    }

    public static class UserTagNames
    {
        private const string root = "user";
        public const string Id = root + ".id";
        public const string GroupId = root + ".group.id";
        public const string ConsentId = root + ".consent.id";
        public const string SessionId = root + ".session.id";
    }

    public static class ApiResourceTagNames
    {
        private const string root = "apiresources";
        public const string Id = root + ".id";
    }
    
    public static class CaTagNames
    {
        private const string root = "ca";
        public const string Id = root + ".id";
        public const string ClientCertificateId = root + ".client.id";
    }

    public static class AcrTagNames
    {
        private const string root = "acr";
        public const string Id = root + ".id";
    }

    public static class ClientTagNames
    {
        private const string root = "client";
        public const string Id = root + ".id";
        public const string Scope = root + ".scope";
    }

    public static class GroupTagNames
    {
        private const string root = "group";
        public const string Id = root + ".id";
        public const string Path = root + ".path";
        public const string Role = root + ".role";
    }

    public static class RealmTagNames
    {
        private const string root = "realm";
        public const string Id = root + ".id";
    }

    public static class ScopeTagNames
    {
        private const string root = "scope";
        public const string Id = root + ".id";
    }

    public static class IdserverTagNames
    {
        private const string root = "oauth";
        public const string GrantType = root + ".grant_type";
        public const string ClientId = root + ".client_id";
    }

    public static class Names
    {
        public const string User = "User";
        public const string ApiResource = "ApiResource";
        public const string Acr = "Acr";
        public const string Ca = "Ca";
        public const string Client = "Client";
        public const string Group = "Group";
        public const string Realm = "Realm";
        public const string Scope = "Scope";
        public const string Idserver = "Idserver";
    }
}
