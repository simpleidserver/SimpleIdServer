// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.CredentialTemplateStore
{
    public class CredentialTemplateReducers
    {
        #region SearchCredentialTemplatesState

        [ReducerMethod]
        public static SearchCredentialTemplatesState ReduceSearchCredentialTemplatesAction(SearchCredentialTemplatesState state, SearchCredentialTemplatesAction act) => new(isLoading: true, credentialTemplates: new List<Domains.CredentialTemplate>(), 0);

        [ReducerMethod]
        public static SearchCredentialTemplatesState ReduceSearchCredentialTemplatesSuccessAction(SearchCredentialTemplatesState state, SearchCredentialTemplatesSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                CredentialTemplates = act.CredentialTemplates.Select(c => new SelectableCredentialTemplate(c)),
                Count = act.Count
            };
        }

        [ReducerMethod]
        public static SearchCredentialTemplatesState ReduceRemoveSelectedCredentialTemplatesAction(SearchCredentialTemplatesState state, RemoveSelectedCredentialTemplatesAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchCredentialTemplatesState ReduceRemoveSelectedCredentialTemplatesSuccessAction(SearchCredentialTemplatesState state, RemoveSelectedCredentialTemplatesSuccessAction act)
        {
            var records = state.CredentialTemplates.ToList();
            records = records.Where(r => !act.CredentialTemplateIds.Contains(r.Value.Id)).ToList();
            return state with
            {
                IsLoading = false,
                Count = records.Count(),
                CredentialTemplates = records
            };
        }

        [ReducerMethod]
        public static SearchCredentialTemplatesState ReduceToggleAllCredentialTemplatesAction(SearchCredentialTemplatesState state, ToggleAllCredentialTemplatesAction act)
        {
            var records = state.CredentialTemplates.ToList();
            foreach (var record in records)
                record.IsSelected = act.IsSelected;
            return state with
            {
                CredentialTemplates = records
            };
        }

        [ReducerMethod]
        public static SearchCredentialTemplatesState ReduceToggleCredentialTemplateAction(SearchCredentialTemplatesState state, ToggleCredentialTemplateAction act)
        {
            var records = state.CredentialTemplates.ToList();
            var record = records.First(r => r.Value.Id == act.CredentialTemplateId);
            record.IsSelected = act.IsSelected;
            return state with
            {
                CredentialTemplates = records
            };
        }

        #endregion
    }
}
