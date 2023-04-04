// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Website.Stores.IdentityProvisioningStore;

namespace SimpleIdServer.IdServer.Website.Stores.ImportSummaryStore
{
    public static class ImportSummaryReducers
    {
        #region SearchImportSummariesState

        [ReducerMethod]
        public static SearchImportSummariesState ReduceSearchImportSummariesAction(SearchImportSummariesState state, SearchImportSummariesAction act) => new(isLoading: true, importSummaries: new List<Domains.ImportSummary>());

        [ReducerMethod]
        public static SearchImportSummariesState ReduceSearchClientsSuccessAction(SearchImportSummariesState state, SearchImportSummariesSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                ImportSummaries = act.ImportSummaries,
                Count = act.Count
            };
        }

        [ReducerMethod]
        public static SearchImportSummariesState ReduceLaunchImportAction(SearchImportSummariesState state, LaunchImportAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchImportSummariesState ReduceLaunchImportSuccessAction(SearchImportSummariesState state, LaunchImportSuccessAction act)
        {
            var importSummaries = state.ImportSummaries.ToList();
            importSummaries.Insert(0, new Domains.ImportSummary
            {
                StartDateTime = DateTime.UtcNow,
                Status = Domains.ImportStatus.START,
                NbRepresentations = 0
            });
            return state with
            {
                IsLoading = false,
                ImportSummaries = importSummaries
            };
        }

        #endregion
    }
}
