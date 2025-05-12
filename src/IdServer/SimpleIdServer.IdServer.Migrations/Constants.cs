// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Migrations;

public static class Constants
{
    public static class Endpoints
    {
        public const string Migrations = "migrations";
        public const string MigDefinitions = $"{Migrations}/definitions";
        public const string MigExecutions = $"{Migrations}/executions";
        public const string LaunchMigration = Migrations + "/{name}/launch";
    }
}
