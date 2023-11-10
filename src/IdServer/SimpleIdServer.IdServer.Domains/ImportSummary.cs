// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class ImportSummary
    {
        [JsonPropertyName(ImportSummaryNames.Id)]
        public string Id { get; set; } = null!;
        [JsonPropertyName(ImportSummaryNames.StartDateTime)]
        public DateTime StartDateTime { get; set; }
        [JsonPropertyName(ImportSummaryNames.EndDateTime)]
        public DateTime? EndDateTime { get; set; }
        [JsonPropertyName(ImportSummaryNames.NbRepresentations)]
        public int NbRepresentations { get; set; }
        [JsonPropertyName(ImportSummaryNames.ErrorMessage)]
        public string? ErrorMessage { get; set; } = null;
        [JsonPropertyName(ImportSummaryNames.Status)]
        public ImportStatus Status { get; set; }
        [JsonPropertyName(ImportSummaryNames.Realm)]
        public string RealmName { get; set; } = null!;
        [JsonIgnore]
        public Realm Realm { get; set; } = null!;

        public static ImportSummary Create(string id, string realm)
        {
            return new ImportSummary
            {
                Id = id,
                RealmName = realm,
                StartDateTime = DateTime.UtcNow,
                Status = ImportStatus.START
            };
        }

        public void Fail(string message) 
        {
            ErrorMessage = message;
            Status = ImportStatus.ERROR;
            EndDateTime = DateTime.UtcNow;
        }

        public void Success(int nbRepresentations)
        {
            NbRepresentations = nbRepresentations;
            Status = ImportStatus.SUCCESS;
            EndDateTime = DateTime.UtcNow;
        }
    }

    public enum ImportStatus
    {
        START = 0,
        SUCCESS = 1,
        ERROR = 2
    }
}
