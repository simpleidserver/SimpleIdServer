// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace TechTalk.SpecFlow
{
    public static class ScenarioContextExtensions
    {
        private static string IsUserAuthenticationEnabledName = "IsUserAuthenticationEnabled";

        public static void EnableUserAuthentication(this ScenarioContext scenarioContext) => scenarioContext.Set(true, IsUserAuthenticationEnabledName);

        public static void DisableUserAuthentication(this ScenarioContext scenarioContext) => scenarioContext.Set(false, IsUserAuthenticationEnabledName);

        public static bool IsUserAuthenticationEnabled(this ScenarioContext scenarioContext)
        {
            if (!scenarioContext.ContainsKey(IsUserAuthenticationEnabledName)) return false;
            return scenarioContext.Get<bool>(IsUserAuthenticationEnabledName);
        }
    }
}
