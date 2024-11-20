﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains;

public class MessageBusErrorMessage
{
    public string Id { get; set; } = null!;
    public string? ExternalId { get; set; } = null;
    public string Name { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Content { get; set; } = null!;
    public List<string> Exceptions { get; set; } = new List<string>();
    public DateTime ReceivedDateTime { get; set; }
    public string QueueName { get; set; } = null!;
}