// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class ImportSummary
    {
        public string Id { get; set; } = null!;
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int NbRepresentations { get; set; }
        public string? ErrorMessage { get; set; } = null;
        public ImportStatus Status { get; set; }
        public string RealmName { get; set; } = null!;
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
