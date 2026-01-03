// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Template.Startup.Configurations;

public class MessageBrokerOptions
{
    public TransportTypes Transport { get; set; }
    public string ConnectionString { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
}

public enum TransportTypes
{
    INMEMORY = 0,
    SQLSERVER = 1
}