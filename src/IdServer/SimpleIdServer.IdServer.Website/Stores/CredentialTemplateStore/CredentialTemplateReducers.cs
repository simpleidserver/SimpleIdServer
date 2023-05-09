// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.Vc.Models;
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

        [ReducerMethod]
        public static UpdateCredentialTemplateState ReduceRemoveSelectedCredentialSubjectsAction(UpdateCredentialTemplateState state, RemoveSelectedCredentialSubjectsAction act) => new(true, null);

        [ReducerMethod]
        public static UpdateCredentialTemplateState ReduceRemoveSelectedCredentialSubjectsSuccessAction(UpdateCredentialTemplateState state, RemoveSelectedCredentialSubjectsSuccessAction act) => new(false, null);

        [ReducerMethod]
        public static UpdateCredentialTemplateState ReduceAddW3CCredentialTemplateCredentialSubjectAction(UpdateCredentialTemplateState state, AddW3CCredentialTemplateCredentialSubjectAction act) => new(true, null);

        [ReducerMethod]
        public static UpdateCredentialTemplateState ReduceAddW3CCredentialTemplateCredentialSubjectSuccessAction(UpdateCredentialTemplateState state, AddW3CCredentialTemplateCredentialSubjectAction act) => new(false, null);

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
            var displays = act.CredentialTemplate.DisplayLst.Select(d => new SelectableCredentialTemplateDisplay(new CredentialTemplateDisplay
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

        #region W3CCredentialTemplateState


        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceGetCredentialTemplateAction(W3CCredentialTemplateState state, GetCredentialTemplateAction act)
        {
            return state with
            {
                IsLoading = true,
                CredentialSubjects = new List<SelectableW3CCredentialSubjectRecord>(),
                Count = 0,
                Types = new List<string>()
            };
        }

        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceGetCredentialTemplateSuccessAction(W3CCredentialTemplateState state, GetCredentialTemplateSuccessAction act)
        {
            if (act.CredentialTemplate.Format != Vc.Constants.CredentialTemplateProfiles.W3CVerifiableCredentials) return state with
            {
                IsLoading = false
            };

            var w3cCredentialTemplate = new W3CCredentialTemplate(act.CredentialTemplate);
            var credentialSubjects = w3cCredentialTemplate.GetSubjects().Select(r => new SelectableW3CCredentialSubjectRecord(r.ParameterId, r.ClaimName, r.Subject));
            return state with
            {
                IsLoading = false,
                TechnicalId = act.CredentialTemplate.TechnicalId,
                CredentialSubjects = credentialSubjects.ToList(),
                Count = credentialSubjects.Count(),
                Types = w3cCredentialTemplate.GetTypes()
            };
        }

        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceToggleAllCredentialSubjectsAction(W3CCredentialTemplateState state, ToggleAllCredentialSubjectsAction act)
        {
            var subjects = state.CredentialSubjects.ToList();
            foreach (var subject in subjects) subject.IsSelected = true;
            return state with
            {
                CredentialSubjects = subjects
            };
        }

        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceToggleCredentialSubjectAction(W3CCredentialTemplateState state, ToggleCredentialSubjectAction act)
        {
            var subjects = state.CredentialSubjects.ToList();
            var subject = subjects.Single(s => s.ClaimName == act.ClaimName);
            subject.IsSelected = act.IsSelected;
            return state with
            {
                CredentialSubjects = subjects
            };
        }

        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceRemoveSelectedCredentialSubjectsAction(W3CCredentialTemplateState state, RemoveSelectedCredentialSubjectsAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceRemoveSelectedCredentialSubjectsSuccessAction(W3CCredentialTemplateState state, RemoveSelectedCredentialSubjectsSuccessAction act)
        {
            var subjects = state.CredentialSubjects.ToList();
            subjects = subjects.Where(s => !act.ParameterIds.Contains(s.ParameterId)).ToList();
            return state with
            {
                IsLoading = false,
                Count = subjects.Count(),
                CredentialSubjects = subjects
            };
        }

        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceUpdateW3CCredentialTemplateTypesAction(W3CCredentialTemplateState state, UpdateW3CCredentialTemplateTypesAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceUpdateW3CCredentialTemplateTypesSuccessAction(W3CCredentialTemplateState state, UpdateW3CCredentialTemplateTypesSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                Types = act.ConcatenatedTypes.Split(';')
            };
        }

        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceAddW3CCredentialTemplateCredentialSubjectAction(W3CCredentialTemplateState state, AddW3CCredentialTemplateCredentialSubjectAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static W3CCredentialTemplateState ReduceAddW3CCredentialTemplateCredentialSubjectSuccessAction(W3CCredentialTemplateState state, AddW3CCredentialTemplateCredentialSubjectSuccessAction act)
        {
            var credentialSubjects = state.CredentialSubjects.ToList();
            credentialSubjects.Add(new SelectableW3CCredentialSubjectRecord(act.ParameterId, act.ClaimName, act.Subject)
            {
                IsNew = true
            });
            return state with
            {
                IsLoading = false,
                Count = credentialSubjects.Count,
                CredentialSubjects = credentialSubjects
            };
        }

        #endregion
    }
}
