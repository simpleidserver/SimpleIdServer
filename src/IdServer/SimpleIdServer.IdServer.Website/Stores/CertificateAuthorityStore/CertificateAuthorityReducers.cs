// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;
using System.Security.Cryptography.X509Certificates;

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

        [ReducerMethod]
        public static SearchCertificateAuthoritiesState ReduceToggleCertificateAuthoritySelectionAction(SearchCertificateAuthoritiesState state, ToggleCertificateAuthoritySelectionAction act)
        {
            var result = state.CertificateAuthorities.ToList();
            result.First(r => r.Value.Id == act.Id).IsSelected = act.IsSelected;
            return state with
            {
                CertificateAuthorities = result
            };
        }

        [ReducerMethod]
        public static SearchCertificateAuthoritiesState ReduceToggleAllCertificateAuthoritySelectionAction(SearchCertificateAuthoritiesState state, ToggleAllCertificateAuthoritySelectionAction act)
        {
            var result = state.CertificateAuthorities.ToList();
            foreach (var r in result)
                r.IsSelected = act.IsSelected;
            return state with
            {
                CertificateAuthorities = result
            };
        }

        [ReducerMethod]
        public static SearchCertificateAuthoritiesState ReduceRemoveSelectedCertificateAuthoritiesSuccessAction(SearchCertificateAuthoritiesState state, RemoveSelectedCertificateAuthoritiesSuccessAction act)
        {
            var result = state.CertificateAuthorities.ToList();
            result = result.Where(r => !act.Ids.Contains(r.Value.Id)).ToList();
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
        public static UpdateCertificateAuthorityState ReduceImportCertificateAuthorityAction(UpdateCertificateAuthorityState state, ImportCertificateAuthorityAction action)
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
                CertificateAuthority = action.CertificateAuthority,
                ErrorMessage = null
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

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceSaveCertificateAuthorityFailureAction(UpdateCertificateAuthorityState state, SaveCertificateAuthorityFailureAction action)
        {
            return state with
            {
                IsUpdating = false
            };
        }

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceGenerateCertificateAuthorityFailureAction(UpdateCertificateAuthorityState state, GenerateCertificateAuthorityFailureAction action)
        {
            return state with
            {
                IsUpdating = false,
                CertificateAuthority = null,
                ErrorMessage = action.ErrorMessage
            };
        }

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceSelectCertificateAuthoritySourceAction(UpdateCertificateAuthorityState state, SelectCertificateAuthoritySourceAction action)
        {
            if (state.CertificateAuthority == null || state.CertificateAuthority.Source == action.Source) return state;
            return state with
            {
                CertificateAuthority = null
            };
        }

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceAddClientCertificateAction(UpdateCertificateAuthorityState state, AddClientCertificateAction action)
        {
            return state with
            {
                IsUpdating = true
            };
        }

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceAddClientCertificateSuccessAction(UpdateCertificateAuthorityState state, AddClientCertificateSuccessAction action)
        {
            return state with
            {
                IsUpdating = false
            };
        }


        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceAddClientCertificateFailureAction(UpdateCertificateAuthorityState state, AddClientCertificateFailureAction action)
        {
            return state with
            {
                IsUpdating = false
            };
        }

        [ReducerMethod]
        public static UpdateCertificateAuthorityState ReduceStartAddCertificateAuthorityAction(UpdateCertificateAuthorityState state, StartAddCertificateAuthorityAction action)
        {
            return state with
            {
                IsUpdating = false,
                ErrorMessage = null,
                CertificateAuthority = null
            };
        }

        #endregion

        #region CertificateAuthorityState

        [ReducerMethod]
        public static CertificateAuthorityState ReduceGetCertificateAuthorityAction(CertificateAuthorityState state, GetCertificateAuthorityAction act) => new(null, null, true);

        [ReducerMethod]
        public static CertificateAuthorityState ReduceGetCertificateAuthoritySuccessAction(CertificateAuthorityState state, GetCertificateAuthoritySuccessAction act) => new(act.CertificateAuthority, act.Certificate, false);

        #endregion

        #region SearchCertificateClientsState

        [ReducerMethod]
        public static SearchCertificateClientsState ReduceGetCertificateAuthorityAction(SearchCertificateClientsState state, GetCertificateAuthorityAction act) => new(true, 0, new List<ClientCertificate>());

        [ReducerMethod]
        public static SearchCertificateClientsState ReduceGetCertificateAuthoritySuccessAction(SearchCertificateClientsState state, GetCertificateAuthoritySuccessAction act) => new(false, act.CertificateAuthority.ClientCertificates.Count(), act.CertificateAuthority.ClientCertificates.ToList());

        [ReducerMethod]
        public static SearchCertificateClientsState ReduceAddClientCertificateAction(SearchCertificateClientsState state, AddClientCertificateAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchCertificateClientsState ReduceAddClientCertificatSuccessAction(SearchCertificateClientsState state, AddClientCertificateSuccessAction act)
        {
            var clients = state.ClientCertificates.ToList();
            clients.Add(new SelectableClientCertificate(act.ClientCertificate, X509Certificate2.CreateFromPem(act.ClientCertificate.PublicKey, act.ClientCertificate.PrivateKey))
            {
                IsNew = true
            });
            return state with
            {
                IsLoading = false,
                ClientCertificates = clients,
                Count = clients.Count()
            };
        }

        [ReducerMethod]
        public static SearchCertificateClientsState ReduceToggleClientCertificateSelectionAction(SearchCertificateClientsState state, ToggleClientCertificateSelectionAction act)
        {
            var clients = state.ClientCertificates.ToList();
            clients.First(c => c.Value.Id == act.Id).IsSelected = act.IsSelected;
            return state with
            {
                ClientCertificates = clients
            };
        }

        [ReducerMethod]
        public static SearchCertificateClientsState ReduceToggleAllClientCertificatesSelectionAction(SearchCertificateClientsState state, ToggleAllClientCertificatesSelectionAction act)
        {
            var clients = state.ClientCertificates.ToList();
            foreach (var client in clients) client.IsSelected = act.IsSelected;
            return state with
            {
                ClientCertificates = clients
            };
        }


        [ReducerMethod]
        public static SearchCertificateClientsState ReduceRemoveSelectedClientCertificatesAction(SearchCertificateClientsState state, RemoveSelectedClientCertificatesAction act)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static SearchCertificateClientsState ReduceRemoveSelectedClientCertificatesSuccessAction(SearchCertificateClientsState state, RemoveSelectedClientCertificatesSuccessAction act)
        {
            var clients = state.ClientCertificates.ToList();
            clients = clients.Where(c => !act.CertificateClientIds.Contains(c.Value.Id)).ToList();
            return state with
            {
                ClientCertificates = clients,
                IsLoading = false,
                Count = clients.Count
            };
        }

        #endregion
    }
}
