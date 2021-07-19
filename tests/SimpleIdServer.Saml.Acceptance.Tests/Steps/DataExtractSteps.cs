// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using TechTalk.SpecFlow;

namespace SimpleIdServer.Saml.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class DataExtractSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public DataExtractSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When("extract XML from body")]
        public async Task GivenExtractFromBody()
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            var xml = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            _scenarioContext.Set(doc, "xmlHttpBody");
        }
    }
}
