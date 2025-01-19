// Copyright (c) SimpleIdServer. All rights reserved.
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Store.EF;

namespace SimpleIdServer.IdServer.Startup.Conf;

public class MigrationService
{
    const string SQLServerCreateTableFormat = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DistributedCache' and xtype='U') " +
        "CREATE TABLE [dbo].[DistributedCache] (" +
        "Id nvarchar(449) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL, " +
        "Value varbinary(MAX) NOT NULL, " +
        "ExpiresAtTime datetimeoffset NOT NULL, " +
        "SlidingExpirationInSeconds bigint NULL," +
        "AbsoluteExpiration datetimeoffset NULL, " +
        "PRIMARY KEY (Id))";


    const string MYSQLCreateTableFormat =
                "CREATE TABLE IF NOT EXISTS DistributedCache (" +
                    "`Id` varchar(449) CHARACTER SET ascii COLLATE ascii_bin NOT NULL," +
                    "`AbsoluteExpiration` datetime(6) DEFAULT NULL," +
                    "`ExpiresAtTime` datetime(6) NOT NULL," +
                    "`SlidingExpirationInSeconds` bigint(20) DEFAULT NULL," +
                    "`Value` longblob NOT NULL," +
                    "PRIMARY KEY(`Id`)," +
                    "KEY `Index_ExpiresAtTime` (`ExpiresAtTime`)" +
    ")";

    const string PostgreCreateSchemaAndTableSql =
        $"""
        CREATE SCHEMA IF NOT EXISTS "public";
        CREATE TABLE IF NOT EXISTS "public"."DistributedCache"
        (
            "Id" text COLLATE pg_catalog."default" NOT NULL,
            "Value" bytea,
            "ExpiresAtTime" timestamp with time zone,
            "SlidingExpirationInSeconds" double precision,
            "AbsoluteExpiration" timestamp with time zone,
            CONSTRAINT "DistCache_pkey" PRIMARY KEY ("Id")
        )
        """;

    public static void EnableIsolationLevel(StoreDbContext dbContext)
    {
        if (dbContext.Database.IsInMemory()) return;
        EnableSqlServer(dbContext);
        EnableMysql(dbContext);
        EnablePostgre(dbContext);
    }

    private static void EnableSqlServer(StoreDbContext dbContext)
    {
        var dbConnection = dbContext.Database.GetDbConnection();
        var sqlConnection = dbConnection as SqlConnection;
        if (sqlConnection != null)
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open) sqlConnection.Open();
            var cmd = sqlConnection.CreateCommand();
            cmd.CommandText = "ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON";
            cmd.ExecuteNonQuery();
            cmd = sqlConnection.CreateCommand();
            cmd.CommandText = SQLServerCreateTableFormat;
            cmd.ExecuteNonQuery();
        }
    }

    private static void EnableMysql(StoreDbContext dbContext)
    {
        var dbConnection = dbContext.Database.GetDbConnection();
        var mysqlConnection = dbConnection as MySqlConnector.MySqlConnection;
        if (mysqlConnection != null)
        {
            if (mysqlConnection.State != System.Data.ConnectionState.Open) mysqlConnection.Open();
            var cmd = mysqlConnection.CreateCommand();
            cmd.CommandText = MYSQLCreateTableFormat;
            cmd.ExecuteNonQuery();
        }
    }

    private static void EnablePostgre(StoreDbContext dbContext)
    {
        var dbConnection = dbContext.Database.GetDbConnection();
        var postgreconnection = dbConnection as Npgsql.NpgsqlConnection;
        if (postgreconnection != null)
        {
            if (postgreconnection.State != System.Data.ConnectionState.Open) postgreconnection.Open();
            var cmd = postgreconnection.CreateCommand();
            cmd.CommandText = PostgreCreateSchemaAndTableSql;
            cmd.ExecuteNonQuery();
        }
    }
}
