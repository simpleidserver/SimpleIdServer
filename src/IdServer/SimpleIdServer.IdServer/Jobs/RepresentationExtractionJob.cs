// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Jobs
{
    public interface IRepresentationExtractionJob
    {
        public string Name { get; }
        public Task Execute(string instanceId, string prefix);
    }

    public abstract class RepresentationExtractionJob : IRepresentationExtractionJob
    {
        public static string SEPARATOR = ";";

        public abstract string Name { get; }

        public abstract Task Execute(string instanceId, string prefix);

        protected string BuildFileColumns(IdentityProvisioningDefinition definition) => $"Id;Version;{string.Join(";", definition.MappingRules.Select(r => r.Id))}";
    }
}
