// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.CertificateAuthorityStore
{
    public class CertificateAuthorityReducers
    {
        #region SearchCertificateAuthoritiesState

        [ReducerMethod]
        public static SearchCertificateAuthoritiesState ReduceSearchCertificateAuthoritiesAction(SearchCertificateAuthoritiesState state, SearchCertificateAuthoritiesAction act) => new(isLoading: true, 0, new List<CertificateAuthority>());

        [ReducerMethod]
        public static SearchCertificateAuthoritiesState ReduceSearchCertificateAuthoritiesSuccessAction(SearchCertificateAuthoritiesState state, SearchCertificateAuthoritiesSuccessAction act)
        {
            return state with
            {
                IsLoading = false,
                CertificateAuthorities = act.CertificateAuthorities.Select(c => new SelectableCertificateAuthority(c)),
                Count = act.Count
            };
        }

        [ReducerMethod]
        public static SearchCertificateAuthoritiesState ReduceSaveCertificateAuthoritySuccessAction(SearchCertificateAuthoritiesState state, SaveCertificateAuthoritySuccessAction action)
        {
            var result = state.CertificateAuthorities.ToList();
            result.Add(new SelectableCertificateAuthority(action.CertificateAuthority)
            {
                IsNew = true
            });
            return state with
            {
                CertificateAuthorities = result,
                Count = result.Count
            };
        }

        #endregion

        #region UpdateCertificateAuthorityState

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceGenerateCertificateAuthorityAction(UpdateCertificateAuthorityState state, GenerateCertificateAuthorityAction action)
        {
            return state with
            {
                IsUpdating = true
            };
        }

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceGenerateCertificateAuthoritySuccessAction(UpdateCertificateAuthorityState state, GenerateCertificateAuthoritySuccessAction action)
        {
            return state with
            {
                IsUpdating = false,
                CertificateAuthority = action.CertificateAuthority
            };
        }

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceSaveCertificateAuthorityAction(UpdateCertificateAuthorityState state, SaveCertificateAuthorityAction action)
        {
            return state with
            {
                IsUpdating = true
            };
        }

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceSaveCertificateAuthoritySuccessAction(UpdateCertificateAuthorityState state, SaveCertificateAuthoritySuccessAction action)
        {
            return state with
            {
                IsUpdating = false,
                CertificateAuthority = null
            };
        }

        #endregion
    }
}
