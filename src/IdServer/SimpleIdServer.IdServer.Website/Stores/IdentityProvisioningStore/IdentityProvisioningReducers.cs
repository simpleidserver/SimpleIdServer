// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.IdentityProvisioningStore
{
    public class IdentityProvisioningReducers
    {
        #region SearchIdentityProvisioningState

        [ReducerMethod]
        public static SearchIdentityProvisioningState ReduceSearchIdentityProvisioningAction(SearchIdentityProvisioningState state, SearchIdentityProvisioningAction act) => new(true, new List<IdentityProvisioning>(), 0);

        [ReducerMethod]
        public static SearchIdentityProvisioningState ReduceSearchIdentityProvisioningSuccessAction(SearchIdentityProvisioningState state, SearchIdentityProvisioningSuccessAction act) => new(false, act.IdentityProvisioningLst, act.Count);

        [ReducerMethod]
        public static SearchIdentityProvisioningState ReduceToggleAllIdentityProvisioningAction(SearchIdentityProvisioningState state, ToggleAllIdentityProvisioningAction act)
        {
            var values = state.Values.ToList();
            foreach (var val in values)
                val.IsSelected = act.IsSelected;
            return state with
            {
                Values = values
            };
        }

        [ReducerMethod]
        public static SearchIdentityProvisioningState ReduceToggleIdentityProvisioningSelectionAction(SearchIdentityProvisioningState state, ToggleIdentityProvisioningSelectionAction act)
        {
            var values = state.Values.ToList();
            var value = values.First(v => v.Value.Id == act.IdentityProvisioningId);
            value.IsSelected = act.IsSelected;
            return state with
            {
                Values = values
            };
        }

        [ReducerMethod]
        public static SearchIdentityProvisioningState ReduceRemoveSelectedIdentityProvisioningAction(SearchIdentityProvisioningState state, RemoveSelectedIdentityProvisioningAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchIdentityProvisioningState ReduceRemoveSelectedIdentityProvisioningSuccessAction(SearchIdentityProvisioningState state, RemoveSelectedIdentityProvisioningSuccessAction act)
        {
            var values = state.Values.ToList();
            values = values.Where(v => !act.Ids.Contains(v.Value.Id)).ToList();
            return state with
            {
                IsLoading = false,
                Values = values,
                Count = values.Count
            };
        }

        [ReducerMethod]
        public static SearchIdentityProvisioningState ReduceUpdateIdProvisioningDetailSuccessAction(SearchIdentityProvisioningState state, UpdateIdProvisioningDetailsSuccessAction act)
        {
            var idProvisioningLst = state.Values.ToList();
            var idProvisioning = idProvisioningLst.SingleOrDefault(i => i.Value.Id == act.Id);
            if(idProvisioning != null)
            {
                idProvisioning.Value.Description = act.Description;
                idProvisioning.Value.UpdateDateTime = DateTime.UtcNow;
            }

            return state with
            {
                Values = idProvisioningLst,
                IsLoading = false
            };
        }

        #endregion

        #region IdentityProvisioningState

        [ReducerMethod]
        public static IdentityProvisioningState ReduceUpdateIdProvisioningPropertiesAction(IdentityProvisioningState state, UpdateIdProvisioningPropertiesAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static IdentityProvisioningState ReduceUpdateIdProvisioningPropertiesSuccessAction(IdentityProvisioningState state, UpdateIdProvisioningPropertiesSuccessAction act)
        {
            var idProvisioning = state.IdentityProvisioning;
            idProvisioning.Properties = act.Properties.ToList();
            return state with
            {
                IsLoading = false,
                IdentityProvisioning = idProvisioning
            };
        }

        [ReducerMethod]
        public static IdentityProvisioningState ReduceGetIdentityProvisioningAction(IdentityProvisioningState state, GetIdentityProvisioningAction act) => new(true, null);

        [ReducerMethod]
        public static IdentityProvisioningState ReduceGetIdentityProvisioningFailureAction(IdentityProvisioningState state, GetIdentityProvisioningSuccessAction act) => new(false, null);

        [ReducerMethod]
        public static IdentityProvisioningState ReduceGetIdentityProvisioningSuccessAction(IdentityProvisioningState state, GetIdentityProvisioningSuccessAction act) => new(false, act.IdentityProvisioning);

        [ReducerMethod]
        public static IdentityProvisioningState ReduceLaunchIdentityProvisioningAction(IdentityProvisioningState state, LaunchIdentityProvisioningAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static IdentityProvisioningState ReduceLaunchIdentityProvisioningSuccessAction(IdentityProvisioningState state, LaunchIdentityProvisioningSuccessAction act)
        {
            var idProvisioning = state.IdentityProvisioning;
            var histories = state.IdentityProvisioning.Histories.ToList();
            histories.Add(new IdentityProvisioningHistory
            {
                StartDateTime = DateTime.UtcNow,
                NbRepresentations = 0,
                Status = IdentityProvisioningHistoryStatus.START
            });
            idProvisioning.Histories = histories;
            return state with
            {
                IsLoading = false,
                IdentityProvisioning = idProvisioning
            };
        }

        [ReducerMethod]
        public static IdentityProvisioningState ReduceUpdateIdProvisioningDetailsAction(IdentityProvisioningState state, UpdateIdProvisioningDetailsAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static IdentityProvisioningState ReduceUpdateIdProvisioningDetailSuccessAction(IdentityProvisioningState state, UpdateIdProvisioningDetailsSuccessAction act)
        {
            var idProvisioning = state.IdentityProvisioning;
            idProvisioning.Description = act.Description;
            idProvisioning.UpdateDateTime = DateTime.UtcNow;
            return state with
            {
                IsLoading = false,
                IdentityProvisioning = idProvisioning
            };
        }

        #endregion
    }
}
