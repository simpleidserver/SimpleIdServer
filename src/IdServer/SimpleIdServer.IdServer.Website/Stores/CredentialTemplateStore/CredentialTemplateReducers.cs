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

        [ReducerMethod]
        public static SearchCredentialTemplatesState ReduceAddCredentialTemplateSuccessAction(SearchCredentialTemplatesState state, AddCredentialTemplateSuccessAction act)
        {
            var records = state.CredentialTemplates.ToList();
            records.Add(new SelectableCredentialTemplate(act.Credential)
            {
                IsNew = true
            });
            return state with
            {
                CredentialTemplates = records,
                Count = records.Count()
            };
        }

        #endregion

        #region UpdateCredentialTemplateState

        [ReducerMethod]
        public static UpdateCredentialTemplateState ReduceAddW3CCredentialTemplateAction(UpdateCredentialTemplateState state, AddW3CCredentialTemplateAction act) => new(true, null);

        [ReducerMethod]
        public static UpdateCredentialTemplateState ReduceAddCredentialTemplateSuccessAction(UpdateCredentialTemplateState state, AddCredentialTemplateSuccessAction act) => new(false, null);

        [ReducerMethod]
        public static UpdateCredentialTemplateState ReduceAddCredentialTemplateErrorAction(UpdateCredentialTemplateState state, AddCredentialTemplateErrorAction act) => new(false, act.ErrorMessage);

        #endregion

        #region CredentialTemplateState

        [ReducerMethod]
        public static CredentialTemplateState ReduceGetCredentialTemplateAction(CredentialTemplateState state, GetCredentialTemplateAction act) => new(true, null);

        [ReducerMethod]
        public static CredentialTemplateState ReduceGetCredentialTemplateSuccessAction(CredentialTemplateState state, GetCredentialTemplateSuccessAction act) => new(false, act.CredentialTemplate);

        #endregion
    }
}
