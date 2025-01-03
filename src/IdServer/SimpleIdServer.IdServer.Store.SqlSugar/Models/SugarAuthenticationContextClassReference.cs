// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models
{
    [SugarTable("Acrs")]
    public class SugarAuthenticationContextClassReference
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null;
        public string DisplayName { get; set; } = null;
        public string AuthenticationMethodReferences { get; set; } = null!;
        public string AuthenticationWorkflow { get; set; } = null;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string? RegistrationWorkflowId { get; set; }
        [Navigate(NavigateType.ManyToOne, nameof(RegistrationWorkflowId))]
        public SugarRegistrationWorkflow? RegistrationWorkflow { get; set; }
        [Navigate(typeof(SugarAuthenticationContextClassReferenceRealm), nameof(SugarAuthenticationContextClassReferenceRealm.AuthenticationContextClassReferencesId), nameof(SugarAuthenticationContextClassReferenceRealm.RealmsName))]
        public List<SugarRealm> Realms { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(SugarClient.AuthenticationContextClassReferenceId))]
        public List<SugarClient> Clients { get; set; }

        public static SugarAuthenticationContextClassReference Transform(AuthenticationContextClassReference record)
        {
            return new SugarAuthenticationContextClassReference
            {
                Id = record.Id,
                CreateDateTime = record.CreateDateTime,
                DisplayName = record.DisplayName,
                Name = record.Name,
                RegistrationWorkflowId = record.RegistrationWorkflowId,
                UpdateDateTime = record.UpdateDateTime,
                Realms = record.Realms.Select(r => new SugarRealm
                {
                    RealmsName = r.Name
                }).ToList()
            };
        }

        public AuthenticationContextClassReference ToDomain()
        {
            return new AuthenticationContextClassReference
            {
                Id = Id,
                Name = Name,
                DisplayName = DisplayName,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                RegistrationWorkflowId = RegistrationWorkflowId,
                Realms = Realms == null ? new List<Realm>() : Realms.Select(r => r.ToDomain()).ToList(),
                Clients = Clients == null ? new List<Client>() : Clients.Select(c => c.ToDomain()).ToList(),
                RegistrationWorkflow = RegistrationWorkflow?.ToDomain()
            };
        }
    }
}
