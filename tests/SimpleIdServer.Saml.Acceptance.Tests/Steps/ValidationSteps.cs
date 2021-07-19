// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Xml;
using TechTalk.SpecFlow;
using Xunit;

namespace SimpleIdServer.Saml.Host.Acceptance.Tests.Steps
{
    [Binding]
    public class ValidationSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public ValidationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }


        [Then("XML exists '(.*)'")]
        public void ThenExists(string key)
        {
            var jsonHttpBody = _scenarioContext["jsonHttpBody"] as JObject;
            Assert.True(jsonHttpBody.ContainsKey(key));
        }

        [Then("XML element '(.*)'='(.*)'")]
        public void ThenXmlElementEqualsTo(string key, string value)
        {
            var xmlDocument = _scenarioContext["xmlHttpBody"] as XmlDocument;
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:protocol");
            Assert.Equal(value, xmlDocument.DocumentElement.SelectSingleNode(key, nsmgr).InnerText);
        }

        [Then("XML attribute '(.*)'='(.*)'")]
        public void ThenXmlAttributeEqualsTo(string key, string value)
        {
            var xmlDocument = _scenarioContext["xmlHttpBody"] as XmlDocument;
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:protocol");
            Assert.Equal(value, xmlDocument.DocumentElement.SelectSingleNode(key, nsmgr).Value);
        }

        [Then("HTTP status code equals to '(.*)'")]
        public void ThenCheckHttpStatusCode(int code)
        {
            var httpResponseMessage = _scenarioContext["httpResponseMessage"] as HttpResponseMessage;
            Assert.Equal(code, (int)httpResponseMessage.StatusCode);
        }
    }
}
