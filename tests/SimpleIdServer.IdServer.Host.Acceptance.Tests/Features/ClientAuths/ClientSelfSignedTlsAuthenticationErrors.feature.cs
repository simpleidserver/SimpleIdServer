﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Features.ClientAuths
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class ClientSelfSignedTlsAuthenticationErrorsFeature : object, Xunit.IClassFixture<ClientSelfSignedTlsAuthenticationErrorsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "ClientSelfSignedTlsAuthenticationErrors.feature"
#line hidden
        
        public ClientSelfSignedTlsAuthenticationErrorsFeature(ClientSelfSignedTlsAuthenticationErrorsFeature.FixtureData fixtureData, SimpleIdServer_IdServer_Host_Acceptance_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/ClientAuths", "ClientSelfSignedTlsAuthenticationErrors", "\tCheck errors returned during the \'self_signed_tls_client_auth\' authentication", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public void TestInitialize()
        {
        }
        
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Error is returned when there is no certificate")]
        [Xunit.TraitAttribute("FeatureTitle", "ClientSelfSignedTlsAuthenticationErrors")]
        [Xunit.TraitAttribute("Description", "Error is returned when there is no certificate")]
        public void ErrorIsReturnedWhenThereIsNoCertificate()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Error is returned when there is no certificate", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 4
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table112 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table112.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
                table112.AddRow(new string[] {
                            "scope",
                            "scope"});
                table112.AddRow(new string[] {
                            "client_id",
                            "elevenClient"});
#line 5
 testRunner.When("execute HTTP POST request \'https://localhost:8080/mtls/token\'", ((string)(null)), table112, "When ");
#line hidden
#line 11
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 12
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 13
 testRunner.And("JSON \'$.error\'=\'invalid_request\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 14
 testRunner.And("JSON \'$.error_description\'=\'certificate is required\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Error is returned when certificate is not correct")]
        [Xunit.TraitAttribute("FeatureTitle", "ClientSelfSignedTlsAuthenticationErrors")]
        [Xunit.TraitAttribute("Description", "Error is returned when certificate is not correct")]
        public void ErrorIsReturnedWhenCertificateIsNotCorrect()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Error is returned when certificate is not correct", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 16
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 17
 testRunner.Given("build random X509Certificate2 and store into \'clientCertificate\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table113 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table113.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
                table113.AddRow(new string[] {
                            "scope",
                            "scope"});
                table113.AddRow(new string[] {
                            "client_id",
                            "elevenClient"});
                table113.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "clientCertificate"});
#line 19
 testRunner.When("execute HTTP POST request \'https://localhost:8080/mtls/token\'", ((string)(null)), table113, "When ");
#line hidden
#line 26
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 27
 testRunner.Then("HTTP status code equals to \'401\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 28
 testRunner.And("JSON \'$.error\'=\'invalid_client\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 29
 testRunner.And("JSON \'$.error_description\'=\'the certificate is not correct\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                ClientSelfSignedTlsAuthenticationErrorsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                ClientSelfSignedTlsAuthenticationErrorsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
