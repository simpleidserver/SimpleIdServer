// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public class ProvisioningConfiguration: ICloneable
    {
        public ProvisioningConfiguration()
        {
            Records = new List<ProvisioningConfigurationRecord>();
            HistoryLst = new List<ProvisioningConfigurationHistory>();
        }

        public string Id { get; set; }
        public ProvisioningConfigurationTypes Type { get; set; }
        public string ResourceType { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public virtual ICollection<ProvisioningConfigurationRecord> Records { get; set; }
        public virtual ICollection<ProvisioningConfigurationHistory> HistoryLst { get; set; } 

        public void Update(IEnumerable<ProvisioningConfigurationRecord> records)
        {
            Records.Clear();
            foreach(var record in records)
            {
                Records.Add(record);
            }

            UpdateDateTime = DateTime.UtcNow;
        }

        public void Complete(string representationId, int version)
        {
            UpdateDateTime = DateTime.UtcNow;
            HistoryLst.Add(ProvisioningConfigurationHistory.Complete(representationId, version));
        }

        public void Error(string representationId, int version, string exception)
        {
            UpdateDateTime = DateTime.UtcNow;
            HistoryLst.Add(ProvisioningConfigurationHistory.Error(representationId, version, exception));
        }

        public bool IsRepresentationProvisioned(string representationId, int version)
        {
            return HistoryLst.Any(r => r.RepresentationId == representationId && r.RepresentationVersion == version && r.Status == ProvisioningConfigurationHistoryStatus.FINISHED);
        }

        public object Clone()
        {
            return new ProvisioningConfiguration
            {
                Type = Type,
                Records = Records.Select(r => (ProvisioningConfigurationRecord)r.Clone()).ToList(),
                HistoryLst = HistoryLst.Select(r => (ProvisioningConfigurationHistory)r.Clone()).ToList()
            };
        }
    }
}
