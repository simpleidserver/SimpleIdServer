// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class AuthenticationContextClassReference : IEquatable<AuthenticationContextClassReference>
    {
        [JsonPropertyName(AuthenticationContextClassReferenceNames.Id)]
        public string Id { get; set; } = null!;
        /// <summary>
        /// Name of the Authentication Context Reference.
        /// </summary>
        [JsonPropertyName(AuthenticationContextClassReferenceNames.Name)]
        public string Name { get; set; }
        /// <summary>
        /// Human-readable Authentication Context Reference.
        /// </summary>
        [JsonPropertyName(AuthenticationContextClassReferenceNames.DisplayName)]
        public string DisplayName { get; set; }
        [JsonPropertyName(AuthenticationContextClassReferenceNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(AuthenticationContextClassReferenceNames.UpdateDateTime)]
        public DateTime UpdateDateTime { get; set; }
        [JsonPropertyName(AuthenticationContextClassReferenceNames.WorkflowId)]
        public string? RegistrationWorkflowId { get; set; }
        [JsonPropertyName(AuthenticationContextClassReferenceNames.RegistrationWorkflow)]
        public RegistrationWorkflow? RegistrationWorkflow { get; set; }
        [JsonPropertyName(AuthenticationContextClassReferenceNames.AuthenticationWorkflow)]
        public string? AuthenticationWorkflow { get; set; }
        [JsonIgnore]
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
        [JsonIgnore]
        public ICollection<Client> Clients { get; set; } = new List<Client>();

        public bool Equals(AuthenticationContextClassReference other)
        {
            if (other == null)
            {
                return false;
            }

            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
