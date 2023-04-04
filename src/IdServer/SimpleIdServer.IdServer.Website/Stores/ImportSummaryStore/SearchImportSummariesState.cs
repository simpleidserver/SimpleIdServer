// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ImportSummaryStore
{
    [FeatureState]
    public record SearchImportSummariesState
    {
        public SearchImportSummariesState() { }

        public SearchImportSummariesState(bool isLoading, IEnumerable<ImportSummary> importSummaries)
        {
            ImportSummaries = importSummaries;
            Count = importSummaries.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<ImportSummary>? ImportSummaries { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }
}
