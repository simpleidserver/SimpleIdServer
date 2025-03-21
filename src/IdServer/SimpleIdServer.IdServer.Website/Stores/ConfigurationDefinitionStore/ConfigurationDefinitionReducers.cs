// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.ConfDefs;
using SimpleIdServer.IdServer.Website.Stores.ConfigurationDefinitionStore;

namespace SimpleIdServer.IdServer.Website.Stores.AcrsStore
{
    public static class ConfigurationDefinitionReducers
    {
        #region ConfigurationDefinitionsState

        [ReducerMethod]
        public static ConfigurationDefinitionsState ReduceGetAllConfigurationDefinitionsAction(ConfigurationDefinitionsState state, GetAllConfigurationDefinitionsAction action) => new(new List<ConfigurationDefResult>(), true);

        [ReducerMethod]
        public static ConfigurationDefinitionsState ReduceGetAllConfigurationDefinitionsSuccessAction(ConfigurationDefinitionsState state, GetAllConfigurationDefinitionsSuccessAction action)
        {
            return state with
            {
                IsLoading = false,
                ConfigurationDefs = action.Content
            };
        }

        #endregion
    }
}
