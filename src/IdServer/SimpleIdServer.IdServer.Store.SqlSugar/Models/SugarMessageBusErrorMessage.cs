// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("MessageBusErrorMessages")]
public class SugarMessageBusErrorMessage
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string? ExternalId { get; set; } = null;
    public string Name { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Exceptions { get; set; } = null!;
    public DateTime ReceivedDateTime { get; set; }
    public string QueueName { get; set; } = null!;

    public static SugarMessageBusErrorMessage Transform(MessageBusErrorMessage messageBusError)
    {
        return new SugarMessageBusErrorMessage
        {
            ReceivedDateTime = messageBusError.ReceivedDateTime,
            QueueName = messageBusError.QueueName,
            Name = messageBusError.Name,
            Id = messageBusError.Id,
            FullName = messageBusError.FullName,
            Exceptions = messageBusError.Exceptions == null ? string.Empty : string.Join(",", messageBusError.Exceptions),
            Content = messageBusError.Content,
            ExternalId = messageBusError.ExternalId
        };
    }


    public MessageBusErrorMessage ToDomain()
    {
        return new MessageBusErrorMessage
        {
            ExternalId = ExternalId,
            Content = Content,
            Exceptions = string.IsNullOrWhiteSpace(Exceptions) ? new List<string>() : Exceptions.Split(',').ToList(),
            FullName = FullName,
            Id = Id,
            Name = Name,
            QueueName = QueueName,
            ReceivedDateTime = ReceivedDateTime
        };
    }
}
