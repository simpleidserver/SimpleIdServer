// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Did.Events
{
    public class ServiceAdded : IEvent
    {
        public const string DEFAULT_NAME = "ServiceAdded";
        public string Name => DEFAULT_NAME;
        public string Id { get; set; }
        public string ServiceEndpoint { get; set; }
        public string Type { get; set; }
    }
}
