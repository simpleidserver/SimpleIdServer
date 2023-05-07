// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using System.Data;

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

        [ReducerMethod]
        public static UpdateCredentialTemplateState ReduceAddCredentialTemplateDisplayAction(UpdateCredentialTemplateState state, AddCredentialTemplateDisplayAction act) => new(true, null);

        [ReducerMethod]
        public static UpdateCredentialTemplateState ReduceAddCredentialTemplateDisplaySuccessAction(UpdateCredentialTemplateState state, AddCredentialTemplateDisplaySuccessAction act) => new(false, null);

        #endregion

        #region CredentialTemplateState

        [ReducerMethod]
        public static CredentialTemplateState ReduceGetCredentialTemplateAction(CredentialTemplateState state, GetCredentialTemplateAction act) => new(true, null);

        [ReducerMethod]
        public static CredentialTemplateState ReduceGetCredentialTemplateSuccessAction(CredentialTemplateState state, GetCredentialTemplateSuccessAction act) => new(false, act.CredentialTemplate);

        #endregion

        #region CredentialTemplateDisplayState

        [ReducerMethod]
        public static CredentialTemplateDisplayState ReduceGetCredentialTemplateAction(CredentialTemplateDisplayState state, GetCredentialTemplateAction act)
        {
            return state with
            {
                IsLoading = true,
                CredentialTemplateDisplays = new List<SelectableCredentialTemplateDisplay>(),
                Count = 0
            };
        }

        [ReducerMethod]
        public static CredentialTemplateDisplayState ReduceGetCredentialTemplateSuccessAction(CredentialTemplateDisplayState state, GetCredentialTemplateSuccessAction act)
        {
            var displays = act.CredentialTemplate.DisplayLst.Select(d => new SelectableCredentialTemplateDisplay(new Domains.CredentialTemplateDisplay
            {
                BackgroundColor = d.BackgroundColor,
                Description = d.Description,
                Id = d.Id,
                Locale = d.Locale,
                LogoUrl = d.LogoUrl,
                LogoAltText = d.LogoAltText,
                Name = d.Name,
                TextColor = d.TextColor
            }));
            return state with
            {
                CredentialTemplateDisplays = displays,
                Count = displays.Count(),
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static CredentialTemplateDisplayState ReduceToggleAllCredentialTemplateDisplayAction(CredentialTemplateDisplayState state, ToggleAllCredentialTemplateDisplayAction act)
        {
            var displays = state.CredentialTemplateDisplays.ToList();
            foreach (var display in displays)
                display.IsSelected = act.IsSelected;
            return state with
            {
                CredentialTemplateDisplays = displays
            };
        }

        [ReducerMethod]
        public static CredentialTemplateDisplayState ReduceToggleCredentialTemplateDisplayAction(CredentialTemplateDisplayState state, ToggleCredentialTemplateDisplayAction act)
        {
            var displays = state.CredentialTemplateDisplays.ToList();
            var display = displays.First(d => d.Value.Id == act.DisplayId);
            display.IsSelected = act.IsSelected;
            return state with
            {
                CredentialTemplateDisplays = displays
            };
        }

        [ReducerMethod]
        public static CredentialTemplateDisplayState ReduceRemoveSelectedCredentialTemplateDisplayAction(CredentialTemplateDisplayState state, RemoveSelectedCredentialTemplateDisplayAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static CredentialTemplateDisplayState ReduceRemoveSelectedCredentialTemplateDisplaySuccessAction(CredentialTemplateDisplayState state, RemoveSelectedCredentialTemplateDisplaySuccessAction act)
        {
            var displays = state.CredentialTemplateDisplays.ToList();
            displays = displays.Where(d => !act.DisplayIds.Contains(d.Value.Id)).ToList();
            return state with
            {
                IsLoading = false,
                CredentialTemplateDisplays = displays,
                Count = displays.Count
            };
        }

        [ReducerMethod]
        public static CredentialTemplateDisplayState ReduceAddCredentialTemplateDisplayAction(CredentialTemplateDisplayState state, AddCredentialTemplateDisplayAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static CredentialTemplateDisplayState ReduceAddCredentialTemplateDisplaySuccessAction(CredentialTemplateDisplayState state, AddCredentialTemplateDisplaySuccessAction act)
        {
            var displays = state.CredentialTemplateDisplays.ToList();
            displays.Add(new SelectableCredentialTemplateDisplay(act.Display)
            {
                IsNew = true
            });
            return state with
            {
                IsLoading = false,
                CredentialTemplateDisplays = displays,
                Count = displays.Count
            };
        }

        #endregion
    }
}
