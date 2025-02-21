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
    public partial class ClientPrivateKeyJwtAuthenticationErrorsFeature : object, Xunit.IClassFixture<ClientPrivateKeyJwtAuthenticationErrorsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "ClientPrivateKeyJwtAuthenticationErrors.feature"
#line hidden
        
        public ClientPrivateKeyJwtAuthenticationErrorsFeature(ClientPrivateKeyJwtAuthenticationErrorsFeature.FixtureData fixtureData, SimpleIdServer_IdServer_Host_Acceptance_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/ClientAuths", "ClientPrivateKeyJwtAuthenticationErrors", "\tCheck errors returned during the \'private_key_jwt\' authentication", ProgrammingLanguage.CSharp, featureTags);
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
        
        [Xunit.SkippableFactAttribute(DisplayName="Error is returned when client_assertion doesn\'t contain iss claim")]
        [Xunit.TraitAttribute("FeatureTitle", "ClientPrivateKeyJwtAuthenticationErrors")]
        [Xunit.TraitAttribute("Description", "Error is returned when client_assertion doesn\'t contain iss claim")]
        public void ErrorIsReturnedWhenClient_AssertionDoesntContainIssClaim()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Error is returned when client_assertion doesn\'t contain iss claim", null, tagsOfScenario, argumentsOfScenario, featureTags);
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
                TechTalk.SpecFlow.Table table101 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table101.AddRow(new string[] {
                            "user",
                            "user"});
#line 5
 testRunner.Given("build JWS by signing with a random RS256 algorithm and store the result into \'cli" +
                        "entAssertion\'", ((string)(null)), table101, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table102 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table102.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
                table102.AddRow(new string[] {
                            "scope",
                            "scope"});
                table102.AddRow(new string[] {
                            "client_assertion_type",
                            "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"});
                table102.AddRow(new string[] {
                            "client_assertion",
                            "$clientAssertion$"});
#line 9
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table102, "When ");
#line hidden
#line 16
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 17
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 18
 testRunner.And("JSON \'$.error\'=\'invalid_request\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 19
 testRunner.And("JSON \'$.error_description\'=\'client_id cannot be extracted from client_assertion\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Error is returned when issuer present in the client_assertion is not a valid clie" +
            "nt")]
        [Xunit.TraitAttribute("FeatureTitle", "ClientPrivateKeyJwtAuthenticationErrors")]
        [Xunit.TraitAttribute("Description", "Error is returned when issuer present in the client_assertion is not a valid clie" +
            "nt")]
        public void ErrorIsReturnedWhenIssuerPresentInTheClient_AssertionIsNotAValidClient()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Error is returned when issuer present in the client_assertion is not a valid clie" +
                    "nt", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 21
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table103 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table103.AddRow(new string[] {
                            "iss",
                            "bad"});
#line 22
 testRunner.Given("build JWS by signing with a random RS256 algorithm and store the result into \'cli" +
                        "entAssertion\'", ((string)(null)), table103, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table104 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table104.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
                table104.AddRow(new string[] {
                            "scope",
                            "scope"});
                table104.AddRow(new string[] {
                            "client_assertion_type",
                            "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"});
                table104.AddRow(new string[] {
                            "client_assertion",
                            "$clientAssertion$"});
#line 26
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table104, "When ");
#line hidden
#line 33
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 34
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 35
 testRunner.And("JSON \'$.error\'=\'invalid_client\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 36
 testRunner.And("JSON \'$.error_description\'=\'unknown client bad\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Error is returned when client_assertion is not signed by a known json web key (JW" +
            "K)")]
        [Xunit.TraitAttribute("FeatureTitle", "ClientPrivateKeyJwtAuthenticationErrors")]
        [Xunit.TraitAttribute("Description", "Error is returned when client_assertion is not signed by a known json web key (JW" +
            "K)")]
        public void ErrorIsReturnedWhenClient_AssertionIsNotSignedByAKnownJsonWebKeyJWK()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Error is returned when client_assertion is not signed by a known json web key (JW" +
                    "K)", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 38
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table105 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table105.AddRow(new string[] {
                            "iss",
                            "sevenClient"});
#line 39
 testRunner.Given("build JWS by signing with a random RS256 algorithm and store the result into \'cli" +
                        "entAssertion\'", ((string)(null)), table105, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table106 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table106.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
                table106.AddRow(new string[] {
                            "scope",
                            "scope"});
                table106.AddRow(new string[] {
                            "client_assertion_type",
                            "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"});
                table106.AddRow(new string[] {
                            "client_assertion",
                            "$clientAssertion$"});
#line 43
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table106, "When ");
#line hidden
#line 50
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 51
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 52
 testRunner.And("JSON \'$.error\'=\'invalid_client\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 53
 testRunner.And("JSON \'$.error_description\'=\'client assertion is not signed by a known Json Web Ke" +
                        "y\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Error is returned when iss != sub")]
        [Xunit.TraitAttribute("FeatureTitle", "ClientPrivateKeyJwtAuthenticationErrors")]
        [Xunit.TraitAttribute("Description", "Error is returned when iss != sub")]
        public void ErrorIsReturnedWhenIssSub()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Error is returned when iss != sub", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 55
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table107 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table107.AddRow(new string[] {
                            "iss",
                            "sevenClient"});
                table107.AddRow(new string[] {
                            "sub",
                            "sub"});
#line 56
 testRunner.Given("build JWS by signing with the key \'seventClientKeyId\' coming from the client \'sev" +
                        "enClient\' and store the result into \'clientAssertion\'", ((string)(null)), table107, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table108 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table108.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
                table108.AddRow(new string[] {
                            "scope",
                            "scope"});
                table108.AddRow(new string[] {
                            "client_assertion_type",
                            "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"});
                table108.AddRow(new string[] {
                            "client_assertion",
                            "$clientAssertion$"});
#line 61
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table108, "When ");
#line hidden
#line 68
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 69
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 70
 testRunner.And("JSON \'$.error\'=\'invalid_client\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 71
 testRunner.And("JSON \'$.error_description\'=\'bad client assertion issuer\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Error is returned when audience is invalid")]
        [Xunit.TraitAttribute("FeatureTitle", "ClientPrivateKeyJwtAuthenticationErrors")]
        [Xunit.TraitAttribute("Description", "Error is returned when audience is invalid")]
        public void ErrorIsReturnedWhenAudienceIsInvalid()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Error is returned when audience is invalid", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 73
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table109 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table109.AddRow(new string[] {
                            "iss",
                            "sevenClient"});
                table109.AddRow(new string[] {
                            "sub",
                            "sevenClient"});
                table109.AddRow(new string[] {
                            "aud",
                            "invalid"});
#line 74
 testRunner.Given("build JWS by signing with the key \'seventClientKeyId\' coming from the client \'sev" +
                        "enClient\' and store the result into \'clientAssertion\'", ((string)(null)), table109, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table110 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table110.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
                table110.AddRow(new string[] {
                            "scope",
                            "scope"});
                table110.AddRow(new string[] {
                            "client_assertion_type",
                            "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"});
                table110.AddRow(new string[] {
                            "client_assertion",
                            "$clientAssertion$"});
#line 80
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table110, "When ");
#line hidden
#line 87
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 88
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 89
 testRunner.And("JSON \'$.error\'=\'invalid_client\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 90
 testRunner.And("JSON \'$.error_description\'=\'bad client assertion audiences\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Error is returned when client_assertion is expired")]
        [Xunit.TraitAttribute("FeatureTitle", "ClientPrivateKeyJwtAuthenticationErrors")]
        [Xunit.TraitAttribute("Description", "Error is returned when client_assertion is expired")]
        public void ErrorIsReturnedWhenClient_AssertionIsExpired()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Error is returned when client_assertion is expired", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 92
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table111 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table111.AddRow(new string[] {
                            "iss",
                            "sevenClient"});
                table111.AddRow(new string[] {
                            "sub",
                            "sevenClient"});
                table111.AddRow(new string[] {
                            "aud",
                            "https://localhost:8080/token"});
#line 93
 testRunner.Given("build expired JWS by signing with the key \'seventClientKeyId\' coming from the cli" +
                        "ent \'sevenClient\' and store the result into \'clientAssertion\'", ((string)(null)), table111, "Given ");
#line hidden
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
                            "client_assertion_type",
                            "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"});
                table112.AddRow(new string[] {
                            "client_assertion",
                            "$clientAssertion$"});
#line 99
 testRunner.When("execute HTTP POST request \'https://localhost:8080/token\'", ((string)(null)), table112, "When ");
#line hidden
#line 106
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 107
 testRunner.Then("HTTP status code equals to \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 108
 testRunner.And("JSON \'$.error\'=\'invalid_client\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 109
 testRunner.And("JSON \'$.error_description\'=\'client assertion is expired\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
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
                ClientPrivateKeyJwtAuthenticationErrorsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                ClientPrivateKeyJwtAuthenticationErrorsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
